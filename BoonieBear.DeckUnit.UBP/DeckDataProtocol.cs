using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using BoonieBear.DeckUnit.BaseType;
using BoonieBear.DeckUnit.DAL;
namespace BoonieBear.DeckUnit.UBP
{
    public interface BDObserver<in T>
    {
        void IUpdateTaskHandle(Object sender, T e);
    }

    //数据包类型
    public enum PackType
    {
        Task = 115,//任务包
        Ack = 0xbdac,//确认包
        Data = 0xbd01,//fsk数据包
        Ans = 0xbdcc,//查询回应包
    }
    //任务类型，相当于命令ID
    public enum TaskType
    {
        Normal = 1,
        FetchBetweenSpanTime = 2,
        Fetch=3,
        BackToWork=4,
        Retrieve = 5,

    }
    //任务确认包中的状态信息：0表示正在准备（接收中），1表示准备完毕，2预留文件不存在，3表示正在唤醒，4表示唤醒失败，5检索时间段内无数据，6表示数据接收出错,，7命令解析失败
    //8，SD卡故障
    public enum TaskState
    {
        RECV=0,//  
        OK =1,//
        NO_FILE =2,//
        WAKING=3,//
        WAKE_FAILED=4,//
        DATA_FAILED=5,
        RECV_FAILED=6,
        INVALID_CMD = 7,
        SD_ERR = 8,
    }
    //任务表中的阶段状态
    public enum TaskStage
    {
        Failed = -1,//任务失败TaskState.NO_FILE WAKE_FAILED DATA_FAILED RECV_FAILED
        UnStart = 0,// 任务未开始 
        Waiting = 1,//准备数据 TaskState.RECV
        Waking=2,//正在唤醒TaskState.WAKING
        OK=3,//数据准备完毕TaskState.OK
        Continue = 4,//持续传递数据中
        Pause = 5,//暂停，等待下次发起任务
        Finish = 6,//任务完成
        WaitForAns=7,//等待dsp响应
        RecvReply=8,//一组数据超时或者是接满了一组数据
    }
    public class DeckDataProtocol
    {
        private static event  EventHandler<EventArgs>TimeOutCallback;
        
        /// <summary>
        /// 数据库接口
        /// </summary>
        private static ISqlDAL _sqlite;
        /// <summary>
        /// 包类型
        /// </summary>
        private int PackageType = (int)PackType.Task;
        /// <summary>
        /// 临时文件夹路径
        /// </summary>
        private  static string TmpDataPath { get; set; }
        /// <summary>
        /// 数据库文件位置
        /// </summary>
        private  static string DBFile { get; set; }
        /// <summary>
        /// 本节点ID
        /// </summary>
        private static int ID { get; set; }
        /// <summary>
        /// 任务运行时间
        /// </summary>
        public static int SecondTicks { get; set; }
        /// <summary>
        /// 任务出错错误码
        /// </summary>
        public static int ErrorCode { get; set; }
        /// <summary>
        /// 任务运行时间定时器
        /// </summary>
        private static Timer totalTimer;
        /// <summary>
        /// 数据接收定时器
        /// </summary>
        private static Timer DataRecvTimer;
        /// <summary>
        /// 当前工作的任务
        /// </summary>
        public static BDTask WorkingBdTask { get; set; }

        /// <summary>
        /// 从每包数据发送时间
        /// </summary>
        public static int SendRecvPeriod { get; set; }
        /// <summary>
        /// 当前任务接收到的包数
        /// </summary>
        public static int RecvPkg { get; set; }
        /// <summary>
        /// 上一次接收最大包号
        /// </summary>
        public static int LastRecvPkgId { get; set; }
        public static int LastRecvUnitId { get; set; }
        /// <summary>
        /// 下一次期待收到的包号，用来判断哪些包没有收到或校验出错
        /// </summary>
        public static int[] ExpectPkgList = null;
        
        /// <summary>
        /// 协议初始化
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="dbPath">数据库文件位置</param>
        /// <returns>true成功，false失败</returns>
        public  static bool Init(int id,string dbPath)
        {
            if (System.IO.File.Exists(dbPath))
            {
                ID = id;
                if (_sqlite != null)
                    return true;
                TmpDataPath = @".\TmpDir\";
                Directory.CreateDirectory(TmpDataPath);
                DBFile = dbPath;
                SendRecvPeriod = 40;//32.88
                WorkingBdTask = null;
                SecondTicks = 0;
                DALFactory.Connectstring = "Data Source=" + DBFile + ";Pooling=True";
                _sqlite = DALFactory.CreateDAL(DBType.Sqlite);
                return true;
            }
            return false;
        }

        public static void AddCallBack(BDObserver<EventArgs> observer)
        {
            TimeOutCallback -= observer.IUpdateTaskHandle;
            TimeOutCallback += observer.IUpdateTaskHandle;

        }
        private static void TaskTimerTick(object state)
        {
            SecondTicks ++;
        }


        /// <summary>
        /// 停止当前任务，不关闭数据库
        /// </summary>
        public static void Stop()
        {
            TmpDataPath = @".\TmpDir\";
            if (WorkingBdTask != null)//还没有完成任务
            {
                if (WorkingBdTask.TaskStage != (int) TaskStage.Finish)
                {
                    WorkingBdTask.TaskStage = (int)TaskStage.Pause;
                    WorkingBdTask.TotolTime += SecondTicks;//加上运行时间
                    SecondTicks = 0;
                    //_sqlite.UpdateTask(WorkingBdTask);
                }
            }
            if (totalTimer != null)
                totalTimer.Dispose();
            if (DataRecvTimer != null)
            {
                DataRecvTimer.Dispose();
                //DataOutTimeTick(null);//调用超时程序更新重传包
                WorkingBdTask.TaskStage = (int)TaskStage.UnStart;
            }
                
            
        }
        /// <summary>
        /// 关闭协议类
        /// </summary>
        public static void Dispose()
        {
            if (WorkingBdTask != null)
                Stop();
            if (_sqlite != null)
                _sqlite.Close();
            WorkingBdTask = null;
        }
        /// <summary>
        /// 生成一个新任务
        /// </summary>
        /// <returns>任务ID号，出错了返回-1</returns>
        public static Int64 CreateNewTask(int destid, int destcomm, TaskType eTaskType, byte[] paraBytes)
        {
            TmpDataPath = @".\TmpDir\";
            WorkingBdTask = null;
            SecondTicks = 0;
            var id = CreateTaskID(destid);
            if (id> 0)
            {
                var task = new BDTask();
                task.TaskID = id;
                task.TaskStage = (int)TaskStage.UnStart;
                task.SourceID = ID;
                task.DestID = destid;
                task.DestPort = destcomm;
                task.CommID = (int)eTaskType;
                task.TotalPkg = 0;
                
                if(paraBytes!=null)
                {
                    task.HasPara = true;
                    task.ParaBytes = new byte[paraBytes.Length];
                    paraBytes.CopyTo(task.ParaBytes, 0);
                }
                else
                {
                    task.HasPara = false;
                    task.ParaBytes = null;
                }
                task.StarTime = DateTime.Now;
                task.TotolTime = 0;
                task.LastTime = DateTime.Now;
                task.RecvBytes = 0;
                TmpDataPath = Directory.CreateDirectory(TmpDataPath + id).FullName;
                task.FilePath = TmpDataPath;
                task.IsParsed = false;
                task.ErrIdxStr = "";
                var ret = _sqlite.AddTask(task);
                if (ret < 1)
                    return -2;
                WorkingBdTask = task;
                ExpectPkgList = new []{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14};
                
                Debug.WriteLine(TmpDataPath);
            }
            
            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialId"></param>
        /// <returns></returns>
        public static long StartTask(long serialId)
        {
            WorkingBdTask = null;
            if (Directory.Exists(TmpDataPath))
            {
                //TmpDataPath += serialId.ToString();
                var filename = Directory.GetFiles(TmpDataPath);
                if (filename.Count() == 0)
                    LastRecvPkgId = -1;
                else
                    LastRecvPkgId = filename.Max(s => int.Parse(s));
                var tsk = _sqlite.GetTask(serialId);
                if (tsk == null)
                    return -1;
                WorkingBdTask = tsk;
                WorkingBdTask.LastTime = DateTime.Now;
                WorkingBdTask.TaskStage = (int) TaskStage.WaitForAns;
                _sqlite.UpdateTask(WorkingBdTask);
                SecondTicks = 0;
                if (WorkingBdTask.ErrIdxStr == "") //没有重传
                {
                    ExpectPkgList = ExpectPkgList.Select(s => s += LastRecvPkgId + 1).ToArray();
                }
                else //有重传，LastRecvPkgId不会等于-1，因为第一次没收到的话不写重传包
                {
                    ReCalcRetryList();
                }
                totalTimer = new Timer(TaskTimerTick, null, 0, 1000);
                return serialId;
            }
            else
            {
                return -1;
            }
        }

        private static void ReCalcRetryList()
        {
            string[] RetryIds = WorkingBdTask.ErrIdxStr.Split(';');
            int MaxRetry = 0;
            if (WorkingBdTask.ErrIdxStr == "") //无erridx
            {
                for (int i = 0; i < 15; i++)
                {
                    ExpectPkgList[i] = LastRecvPkgId + i;
                    if (ExpectPkgList[i] > WorkingBdTask.TotalPkg - 1)
                        ExpectPkgList[i] = -1;
                }
                return;
            }
            else
                MaxRetry = int.Parse(RetryIds[RetryIds.Count() - 1]);
            if (LastRecvPkgId > MaxRetry) //收到包比重传的大
            {
                for (int i = 0; i < RetryIds.Count(); i++)
                {
                    ExpectPkgList[i] = int.Parse(RetryIds[i]);
                }
                for (int i = RetryIds.Count(), j = 0; i < 15; i++)
                {
                    j++;
                    ExpectPkgList[i] = LastRecvPkgId + j;
                    if (ExpectPkgList[i] > WorkingBdTask.TotalPkg - 1)
                        ExpectPkgList[i] = -1;
                    Debug.WriteLine("ExpectPkgList-{0}", ExpectPkgList[i]);
                }
            }
            else
            {
                for (int i = 0; i < RetryIds.Count(); i++)
                {
                    ExpectPkgList[i] = int.Parse(RetryIds[i]);
                }
                for (int i = RetryIds.Count(), j = 0; i < 15; i++)
                {
                    j++;
                    ExpectPkgList[i] = MaxRetry + j;
                    if (ExpectPkgList[i] > WorkingBdTask.TotalPkg - 1)
                        ExpectPkgList[i] = -1;
                    Debug.WriteLine("ExpectPkgList-{0}", ExpectPkgList[i]);
                }
            }
        }

        /// <summary>
        /// 下达任务后，接收数据超时定时器，这个定时器回调说明一组数据应该发完了，要计算重发和检查数据完整性
        /// </summary>
        /// <param name="state"></param>
        private static void DataOutTimeTick(object state)
        {
            WorkingBdTask.TotolTime += SecondTicks;
            SecondTicks = 0;
            if(LastRecvPkgId==-1)//还没有收到过正确数据
            {
                
            }
            else//收到过数据
            {
                if (ExpectPkgList==null)
                    ExpectPkgList = new int[15];
                WorkingBdTask.ErrIdxStr = "";
                foreach (var id in ExpectPkgList)
                {
                    if (id != -1) //未接收数据的包id
                    {
                        WorkingBdTask.ErrIdxStr += id.ToString()+";";
                    }
                }
                
                var last = WorkingBdTask.ErrIdxStr.LastIndexOf(';');
                if (last>0)
                    WorkingBdTask.ErrIdxStr = WorkingBdTask.ErrIdxStr.Remove(last);//1;2;3;4重新计算前需要移除最后一个;号
                ReCalcRetryList();
            }
            if(WorkingBdTask.TaskStage != (int)TaskStage.Pause)//stop状态下不设置为重传包状态
                WorkingBdTask.TaskStage = (int) TaskStage.RecvReply;
            _sqlite.UpdateTask(WorkingBdTask);
            if(TimeOutCallback!=null)
                TimeOutCallback.Invoke(null,null);
        }
        private static Int64 CreateTaskID(int destid)
        {
            var nowTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var strid =  nowTime + destid.ToString("D2");
            long id = 0;
            if (Int64.TryParse(strid, out id))
                return id;
            ErrorCode = -2;
            return -1;
        }

        /// <summary>
        /// 解析数据包并更新数据库任务包设置
        /// </summary>
        /// <param name="bytes">以包类型为开头的数据体</param>
        /// <param name="error">解析错误消息</param>
        /// <returns>返回任务状态</returns>
        public static TaskStage ParseData(byte[] bytes, out string error)
        {
            error = "";
            try
            {
                WorkingBdTask.TotolTime += SecondTicks;
                SecondTicks = 0;
                TaskStage ts = TaskStage.UnStart;
                if (BitConverter.ToUInt16(bytes, 0) == (int)PackType.Ack)
                {
                    switch ((TaskState)BitConverter.ToInt16(bytes,4))
                    {

                        case TaskState.OK://一般直接返回数据了，不返回这个包
                            WorkingBdTask.TaskStage = (int) TaskStage.Continue;
                            ErrorCode = 0;//没有错误
                            _sqlite.UpdateTask(WorkingBdTask);
                            ts = TaskStage.Continue;
                            break;
                        case TaskState.DATA_FAILED: 
                        case TaskState.NO_FILE:
                        case TaskState.RECV_FAILED: 
                        case TaskState.WAKE_FAILED:     
                        case TaskState.INVALID_CMD:
                            if ((TaskState)BitConverter.ToInt16(bytes, 4)==TaskState.DATA_FAILED)
                                error = "检索时间段内无数据";
                            if ((TaskState)BitConverter.ToInt16(bytes, 4)== TaskState.NO_FILE)
                                error = "该时段没有数据";
                            if ((TaskState)BitConverter.ToInt16(bytes, 4)== TaskState.RECV_FAILED)
                                error = "接收数据出错";
                            if ((TaskState)BitConverter.ToInt16(bytes, 4)== TaskState.WAKE_FAILED)
                                error = "唤醒失败";
                            if ((TaskState)BitConverter.ToInt16(bytes, 4)==TaskState.INVALID_CMD)
                                error = "命令解析失败";
                            if ((TaskState)BitConverter.ToInt16(bytes, 4)== TaskState.SD_ERR)
                                error = "SD卡故障";
                            DataRecvTimer.Dispose();
                            WorkingBdTask.TaskStage = (int)TaskStage.Failed;
                            ErrorCode = BitConverter.ToInt16(bytes,4);
                            _sqlite.UpdateTask(WorkingBdTask);
                            ts = TaskStage.Failed;
                            break;
                        case TaskState.RECV:
                            DataRecvTimer.Dispose();
                            WorkingBdTask.TaskStage = (int) TaskStage.Waiting;
                            _sqlite.UpdateTask(WorkingBdTask);
                            ts = TaskStage.Waiting;
                            break;
                        case TaskState.WAKING:
                            DataRecvTimer.Dispose();
                            WorkingBdTask.TaskStage = (int) TaskStage.Waking;
                            _sqlite.UpdateTask(WorkingBdTask);
                            ts = TaskStage.Waking;
                            break;
                        default:
                            DataRecvTimer.Dispose();
                            error = "未定义的包类型";
                            ts = TaskStage.Failed;
                            break;
                    }
                    
                }
                else if (BitConverter.ToUInt16(bytes, 0) == (int)PackType.Data)
                {
                    var ret = StoreData(bytes,15);
                    if (DataRecvTimer!=null)
                        DataRecvTimer.Dispose();
                    if (ret == 0)
                    {
                        WorkingBdTask.TaskStage = (int)TaskStage.Continue;
                        _sqlite.UpdateTask(WorkingBdTask);
                        int unitidx = bytes[17];
                        int unitnum = bytes[16];
                        if (unitidx == unitnum - 1 && unitidx != LastRecvUnitId)//每组最后一包不能连续处理
                        {
                            DataOutTimeTick(null);
                        }
                        else
                        {
                            DataRecvTimer = new Timer(DataOutTimeTick, null, ((unitnum - 1 - unitidx) * SendRecvPeriod) * 1000, 0);
                            ts = TaskStage.Continue;
                        }
                        LastRecvUnitId = unitidx;

                    }        
                    else if (ret == 1)
                    {
                        WorkingBdTask.TaskStage = (int)TaskStage.Finish;
                        _sqlite.UpdateTask(WorkingBdTask);
                        ts = TaskStage.Finish;  
                    }
                    else
                    {
                        WorkingBdTask.TaskStage = (int)TaskStage.Failed;
                        _sqlite.UpdateTask(WorkingBdTask);
                        ts = TaskStage.Failed;
                    }
                }
                else
                {
                    WorkingBdTask.TaskStage = (int)TaskStage.Failed;
                    _sqlite.UpdateTask(WorkingBdTask);
                    error = "未定义的包类型";
                    ts = TaskStage.Failed;
                }
                if (TimeOutCallback != null)
                    TimeOutCallback.Invoke(null, null);
                return ts;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                error = e.Message;
                ErrorCode = -2;
                WorkingBdTask.TaskStage = (int)TaskStage.Failed;
                _sqlite.UpdateTask(WorkingBdTask);
                if (TimeOutCallback != null)
                    TimeOutCallback.Invoke(null, null);
                return TaskStage.Failed;
            }
        }

        /// <summary>
        /// 存储收到的数据
        /// </summary>
        /// <param name="bytes">数据包</param>
        /// <param name="pkg">分组包数</param>
        /// <returns>0,正常，1完成，-1错误</returns>
        private static int StoreData(byte[] bytes,int pkg)
        {
            int ret = 0;
            var packageid = BitConverter.ToInt16(bytes, 14);
            
            WorkingBdTask.TotalPkg = BitConverter.ToInt16(bytes, 12);
            if (ExpectPkgList == null)
            {
                ExpectPkgList = new int[pkg];
                for (int i = 0; i < pkg; i++)
                {
                   ExpectPkgList[i] = i;
                }
            }
            for (int i = 0; i < pkg; i++)
            {
                if (ExpectPkgList[i] > WorkingBdTask.TotalPkg-1)
                    ExpectPkgList[i] = -1;
            }
            var packagelength = bytes.Length-18;//加上组内包数包号
            var stream = new FileStream(TmpDataPath + "\\" + packageid, FileMode.Create);
            stream.Write(bytes, 18, packagelength);
            stream.Flush();
            stream.Close();
            LastRecvPkgId = packageid;
            for (int i = 0; i < pkg; i++)
            {
                if (ExpectPkgList[i] == (int) packageid)
                    ExpectPkgList[i] = -1;
            }//在期望列表里去掉收到的节点
            var filename = Directory.GetFiles(TmpDataPath);
            RecvPkg = filename.Count();
            var totalbytes = filename.Select(s => new FileInfo(s)).Select(file => (int) file.Length).Sum();//LINQ
            WorkingBdTask.RecvBytes = totalbytes;
            if (filename.Count() == WorkingBdTask.TotalPkg && CheckFileIntegrity(WorkingBdTask.TaskID))
            {
                string err;
                ExpectPkgList = null;
                ret = -1;
                if(DeckDataProtocol.BuildFile(WorkingBdTask.TaskID,out err))
                    ret = 1;
                
            }
            return ret;

        }

        ///  <summary>
        ///  检查任务文件完整性,如果文件名是连续的则判断为完整的，本方法默认已收到
        ///  最后的数据文件，也可在后期加入校验码，但是如果
        /// 数据比较多的话，DSP无法一次性得出校验，则无法用校验方法。 
        ///  </summary>
        ///  <param name="id">任务ID</param>
        public static bool CheckFileIntegrity(long id)
        {
            var filename = Directory.GetFiles(TmpDataPath);
            List<int> fileList = new List<int>(15);
            foreach (var name in filename)
            {
                fileList.Add(int.Parse(name.Substring(name.LastIndexOf("\\")+1)));
            }
            fileList.Sort();
            for (int j = 0; j < fileList.Count; j++)
            {
                if(fileList.Exists((s) => s == j)==false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 组装数据包，成功则将文件路径存入数据库
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="err">错误消息字符串</param>
        /// <returns></returns>
        public static bool BuildFile(long id, out string err)
        {
            err = "";
            try
            {
                string TaskDIR = Directory.CreateDirectory(@".\TaskDir\").FullName;
                var stream = new FileStream(TaskDIR+id, FileMode.Create);
                var filename = Directory.GetFiles(TmpDataPath);
                foreach (var streams in filename.Select(s => new FileStream(s, FileMode.Open)))
                {
                    streams.CopyTo(stream);
                    streams.Close();
                    
                }
                stream.Close();
                WorkingBdTask.FilePath = TaskDIR;
                _sqlite.UpdateTask(WorkingBdTask);//存入数据路径
                return true;
            }
            catch (Exception exception)
            {
                ErrorCode = -1;
                err = exception.Message;
                return false;
            }
            
        }

        /// <summary>
        /// 分析数据并存入数据库中，暂时没有这个需求
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="err">错误消息</param>
        /// <returns>true成功，flase失败</returns>
        public static bool AnalyseData(long id, out string err)
        {
            err = "";
            try
            {
                return true;
            }
            catch (Exception exception)
            {

                err = exception.Message;
                return false;
            }
            
        }
    

    
    }
}

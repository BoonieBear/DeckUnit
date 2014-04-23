using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.DAL.DBModel;

namespace BoonieBear.DeckUnit.CommLib.Protocol
{
    //数据包类型
    public enum PackType
    {
        Task = 0,//任务包
        Ack = 1,//确认包
        Data = 2,//数据包

    }
    //任务类型，相当于命令ID
    public enum TaskType
    {
        ReadParameter = 0,
        Reset = 1,
    }
    //任务确认包中的状态信息
    public enum TaskState
    {
        OK=0,//  正常，后面紧跟数据
        Failed =1,//失败，需再发起任务
        Wait =2,//等待，准备数据中，准备成功，回复OK状态确认包
        End=3,//分组完成
        Success=4,//任务成功
    }
    //任务表中的阶段状态
    public enum TaskStage
    {
        Undefine,//未定义，解析出错
        Failed = -1,//任务失败
        Begin = 0,// 任务开始 
        Waiting =1,//等待DSP准备数据
        Continue = 2,//持续传递数据中
        Pause = 3,//暂停，等待下次发起任务
        Finish = 4,//任务完成
    }
    public class DeckDataProtocol
    {
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
        /// 任务运行时间定时器
        /// </summary>
        private static Timer totalTimer;
        /// <summary>
        /// 当前工作的任务
        /// </summary>
        private static Task WorkingTask { get; set; }
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
                if (_sqlite != null)
                    return true;
                ID = id;
                TmpDataPath = @"..\TmpDir\";
                Directory.CreateDirectory(TmpDataPath);
                DBFile = dbPath;
                SecondTicks = 0;
                DALFactory.connectstring = "Data Source=" + DBFile + ";Pooling=True";
                _sqlite = DALFactory.CreateDAL(DBType.Sqlite);
                
            }
            return false;
        }

        private static void TaskTimerTick(object state)
        {
            SecondTicks ++;
        }

        

        /// <summary>
        /// 停止当前任务，关闭数据库
        /// </summary>
        public static void Stop()
        {
            TmpDataPath = @"..\TmpDir\";
            if (WorkingTask != null)//还没有完成任务
            {
                if (WorkingTask.TaskState != (int) TaskStage.Finish)
                {
                    WorkingTask.TaskState = (int)TaskStage.Pause;
                    WorkingTask.TotolTime += SecondTicks;//加上运行时间
                    SecondTicks = 0;
                    _sqlite.UpdateTask(WorkingTask);
                }
            }
            if (totalTimer!=null)
                totalTimer.Dispose();
            WorkingTask = null;
            if (_sqlite!=null)
                _sqlite.Close();
        }
        /// <summary>
        /// 生成一个新任务
        /// </summary>
        /// <returns>任务ID号，出错了返回-1</returns>
        public static Int64 StartNewTask(int destid, int destcomm, TaskType eTaskType, byte[] paraBytes)
        {
            var id = CreateTaskID(destid);
            if (id> 0)
            {
                var task = new Task();
                task.TaskID = id;
                task.TaskState = (int)TaskStage.Begin;
                task.SourceID = ID;
                task.DestID = destid;
                task.DestPort = destcomm;
                task.CommID = (int)eTaskType;
                task.ErrIndex = new BitArray(32);
                
                if(paraBytes!=null)
                {
                        task.HasPara = true;
                        paraBytes.CopyTo(task.ParaBytes, 0);
                }
                else
                {
                    task.HasPara = false;
                    task.ParaBytes = new byte[0];
                }
                task.StarTime = DateTime.UtcNow;
                task.TotolTime = 0;
                task.LastTime = DateTime.UtcNow;
                task.RecvBytes = 0;
                task.FilePath = "";
                task.IsParsed = false;
                task.JSON = "";
                var ret = _sqlite.AddTask(task);
                if (ret < 1)
                    return -2;
                WorkingTask = task;
                totalTimer = new Timer(TaskTimerTick,null,0,1000);
         
                TmpDataPath = Directory.CreateDirectory(TmpDataPath + id).FullName;
                Debug.WriteLine(TmpDataPath);
            }
            
            return id;
        }
        public static Int64 ContinueTask(Int64 serialId)
        {
            if (Directory.Exists(TmpDataPath + serialId))
            {
                TmpDataPath += serialId.ToString();
                var tsk = _sqlite.GetTask(serialId);
                if (tsk == null)
                    return -1;
                WorkingTask = tsk;
                WorkingTask.LastTime = DateTime.UtcNow;
                WorkingTask.TaskState = (int) TaskStage.Begin;
                _sqlite.UpdateTask(WorkingTask);
                totalTimer = new Timer(TaskTimerTick, null, 0, 1000);
                return serialId;
            }
            else
            {
                return -1;
            }
        }
        private static Int64 CreateTaskID(int destid)
        {
            var nowTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var strid = nowTime + ID.ToString("D2") + destid.ToString("D2");
            long id = 0;
            if (Int64.TryParse(strid, out id))
                return id;
            return -1;
        }

        /// <summary>
        /// 根据任务ID读出任务包数据
        /// </summary>
        /// <param name="serialid">任务号</param>
        /// <returns>任务包</returns>
        public static byte[] TaskPackage(Int64 serialid)
        {
            var tsk = _sqlite.GetTask(serialid);
            if (tsk==null)
                return null;
            WorkingTask = tsk;
            int byteslength = 1+1+1+8 + 1 + 1 + 2 +4 + 4;
            if (tsk.HasPara)
                byteslength += tsk.ParaBytes.Length;
            var bytes = new byte[byteslength];
            UInt16 uid = 0xEE01;
            Buffer.BlockCopy(BitConverter.GetBytes(uid), 0, bytes, 0, 2);//头
            Buffer.BlockCopy(BitConverter.GetBytes(byteslength), 0, bytes, 2, 2);//长度
            Buffer.BlockCopy(BitConverter.GetBytes((int)PackType.Task), 0, bytes, 4, 1);//任务包类型
            Buffer.BlockCopy(BitConverter.GetBytes(ID), 0, bytes, 5, 1);//源ID
            Buffer.BlockCopy(BitConverter.GetBytes(tsk.DestID), 0, bytes, 6, 1);//目的id
            Buffer.BlockCopy(BitConverter.GetBytes(tsk.DestPort), 0, bytes, 7, 1);//设备端口
            Buffer.BlockCopy(BitConverter.GetBytes(tsk.TaskID), 0, bytes, 8, 8);//任务id
            Buffer.BlockCopy(BitConverter.GetBytes(tsk.CommID), 0, bytes, 16, 1);//命令ID
            Buffer.BlockCopy(BitConverter.GetBytes(tsk.ParaBytes.Length), 0, bytes, 17, 2);//参数长度
            if (tsk.ParaBytes.Length > 0)
            {
                Buffer.BlockCopy(tsk.ParaBytes, 0, bytes, 19, tsk.ParaBytes.Length);
            }
            var b = new[] { 0 };
            tsk.ErrIndex.CopyTo(b,0);
            Buffer.BlockCopy(BitConverter.GetBytes(b[0]), 0, bytes, byteslength-4, 4);
            return bytes;

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
                WorkingTask.TotolTime += SecondTicks;
                SecondTicks = 0;
                if (bytes[0]==(int)PackType.Ack)
                {
                    
                    switch ((TaskState)bytes[1])
                    {
                        case TaskState.OK:
                            WorkingTask.TaskState = (int) TaskStage.Continue;  

                            _sqlite.UpdateTask(WorkingTask);
                            return TaskStage.Continue;
                        case TaskState.Failed:
                            WorkingTask.TaskState = (int)TaskStage.Failed;

                            _sqlite.UpdateTask(WorkingTask);
                            return TaskStage.Failed;
                        case TaskState.Wait:
                            WorkingTask.TaskState = (int) TaskStage.Waiting;
                            _sqlite.UpdateTask(WorkingTask);
                            return TaskStage.Waiting;
                        case TaskState.End://更新数据库errorbit
                            WorkingTask.ErrIndex = new BitArray(new[] { BitConverter.ToInt32(bytes, 2) });
                            _sqlite.UpdateTask(WorkingTask);
                            return TaskStage.Continue;
                        case TaskState.Success:
                            if (BitConverter.ToInt32(bytes, 2) != 0) //有错误包
                            {
                                WorkingTask.ErrIndex = new BitArray(new[] { BitConverter.ToInt32(bytes, 2) });
                                _sqlite.UpdateTask(WorkingTask);
                                return TaskStage.Continue;
                            }
                            else//任务完成
                            {
                                WorkingTask.TaskState = (int)TaskStage.Finish;
                                WorkingTask.ErrIndex = new BitArray(new[] { 0 });
                                _sqlite.UpdateTask(WorkingTask);
                                totalTimer.Dispose();
                                return TaskStage.Finish;
                            }
                            
                        default:
                            error = "未定义的包类型";
                            return TaskStage.Undefine;
                    }
                    
                }
                if (bytes[0] == (int) PackType.Data)
                {
                    StoreData(bytes);
                    return TaskStage.Continue;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                error = e.Message;
                return TaskStage.Undefine;
            }
            error = "未定义的包类型";
            return TaskStage.Undefine;
        }

        /// <summary>
        /// 存储收到的数据
        /// </summary>
        /// <param name="bytes">数据包</param>
        /// <returns></returns>
        private static void StoreData(byte[] bytes)
        {
            var packageid = BitConverter.ToInt16(bytes, 1);
            var packagelength = BitConverter.ToInt16(bytes, 3);
            var stream = new FileStream(TmpDataPath + "\\" + packageid, FileMode.Create);
            stream.Write(bytes, 5, packagelength);
            stream.Flush();
            stream.Close();
            var filename = Directory.GetFiles(TmpDataPath);
            var totalbytes = filename.Select(s => new FileInfo(s)).Select(file => (int) file.Length).Sum();//LINQ
            WorkingTask.RecvBytes = totalbytes;
            _sqlite.UpdateTask(WorkingTask);
        }

        ///  <summary>
        ///  检查任务文件完整性,如果文件名是连续的则判断为完整的，本方法默认已收到
        ///  最后的数据文件，也可在后期加入校验码，但是如果
        /// 数据比较多的话，DSP无法一次性得出校验，则无法用校验方法。 
        ///  </summary>
        ///  <param name="id">任务ID</param>
        public static bool CheckFileIntegrity(long id)
        {
            var filename = Directory.GetFiles(TmpDataPath + "\\" + id);
            var i = 0;
            return filename.All(s => int.Parse((new FileInfo(s)).Name) == i++);
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
                Directory.CreateDirectory(@"..\TaskDir\");
                var stream = new FileStream(@"..\TaskDir\"  + id, FileMode.Create);
                var filename = Directory.GetFiles(TmpDataPath);
                foreach (var streams in filename.Select(s => new FileStream(s, FileMode.Open)))
                {
                    streams.CopyTo(stream);
                    streams.Close();
                    
                }
                foreach (var s in filename)
                {
                    File.Delete(s);
                }
                stream.Close();
                var filepath = @"..\TaskDir\" + id;
                WorkingTask.FilePath = filepath;
                _sqlite.UpdateTask(WorkingTask);//存入数据路径
                return true;
            }
            catch (Exception exception)
            {
                
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

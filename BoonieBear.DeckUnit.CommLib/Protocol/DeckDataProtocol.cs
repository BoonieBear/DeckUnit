using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
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
        Success=3,//任务成功
    }
    //任务表中的阶段状态
    public enum TaskStage
    {
        Undefine,//未定义，解析出错
        Failed = -1,//任务失败
        Begin = 0,//  
        Continue = 1,//持续传递数据中
        Pause = 2,//暂停，等待下次发起任务
        Finish = 3,//任务完成
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
        /// 当前任务ID号
        /// </summary>
        private static long WorkingTasKID { get; set; }//当前任务的ID号
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
                TmpDataPath = @"..\TmpDir\";
                DBFile = dbPath;
                DALFactory.connectstring = "Data Source=" + DBFile + ";Pooling=True";
                _sqlite = DALFactory.CreateDAL(DBType.Sqlite);
                if (_sqlite!= null)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 停止当前任务，关闭数据库
        /// </summary>
        public static void Stop()
        {
            TmpDataPath = @"..\TmpDir\";
            WorkingTasKID = 0;
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
                task.RecvUnit = 0;
                task.ErrIndex = new BitArray(16);
                
                if(paraBytes!=null)
                {
                        task.HasPara = true;
                        paraBytes.CopyTo(task.ParaBytes, 0);
                }
                else
                {
                    task.HasPara = false;
                    task.ParaBytes = null;
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
                TmpDataPath = Directory.CreateDirectory(TmpDataPath + id).FullName;
                Debug.WriteLine(TmpDataPath);
                WorkingTasKID = id;
            }
            
            return id;
        }

        public static Int64 ContinueTask(Int64 serialId)
        {
            if (Directory.Exists(TmpDataPath + serialId))
            {
                TmpDataPath += serialId.ToString();
                WorkingTasKID = serialId;
                var tsk = _sqlite.GetTask(WorkingTasKID);
                if (tsk == null)
                    return -1;
                return serialId;
            }
            else
            {
                return -1;
            }
        }
        private static Int64 CreateTaskID(int destid)
        {
            string nowTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string strid = nowTime + ID.ToString("D2") + destid.ToString("D2");
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
            Buffer.BlockCopy(BitConverter.GetBytes(tsk.ParaBytes.Length), 0, bytes, 17, 2);
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
        /// <returns>返回任务状态</returns>
        public static TaskStage ParseData(byte[] bytes)
        {
            try
            {
                if (bytes[0]==(int)PackType.Ack)
                {
                    return TaskStage.Continue;
                }
                if (bytes[0] == (int) PackType.Data)
                {
                    return TaskStage.Continue;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return TaskStage.Undefine;
            }
            return TaskStage.Undefine;
        }
    }
}

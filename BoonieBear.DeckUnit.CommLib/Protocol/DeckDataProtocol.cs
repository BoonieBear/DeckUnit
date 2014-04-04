using System;

namespace BoonieBear.DeckUnit.CommLib.Protocol
{
    public class DeckDataProtocol
    {
        private int PackageType = 7;
        private  static string TmpDataPath { get; set; }
        private  static string DBFile { get; set; }
        /// <summary>
        /// 本节点ID
        /// </summary>
        private static int ID { get; set; }

        public  static bool Init(int id,string dbPath, string tmpDataPath)
        {
            if (System.IO.Directory.Exists(tmpDataPath)&&System.IO.File.Exists(dbPath))
            {
                ID = id;
                TmpDataPath = tmpDataPath;
                DBFile = dbPath;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 生成一个新任务
        /// </summary>
        /// <returns>任务ID号，出错了返回-1</returns>
        public UInt64 CreateNewTask(int destid,int destcomm,TaskType eTaskType,byte[] paraBytes)
        {
            return 0;
        }

        private UInt64 CreateTaskID()
        {
            string nowTime = DateTime.Now.ToLongTimeString();
            ulong id = 0;
            if (UInt64.TryParse(nowTime, out id))
                return id;
            return 0;
        }
        public byte[] TaskPackage(int serialid)
        {
            return null;
        }

        public bool ParseData(byte[] bytes)
        {
            return true;
        }
    }
}

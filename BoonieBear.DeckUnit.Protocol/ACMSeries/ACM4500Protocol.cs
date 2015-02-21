using System;
using System.Collections;
using System.Threading.Tasks;

namespace BoonieBear.DeckUnit.Protocol.ACMSeries
{
    public class ACM4500Protocol
    {
        private static readonly Hashtable ACMCommandID = new Hashtable();
        private static byte[] dataBytes;//打包数据
        private static byte[] decodeBytes;//解调数据
        
        public static string Errormessage { get; private set; }

        //some initial para
        public class CommPara
        {
            public static DataType Type;
            public static bool LinkOrient;//true:uplink,false:downlink

            //初始化参数属性
            public static byte[] PARA 
            {
                get
                {
                    var bytes = new byte[MovGlobalVariables.MFSKSize];
                    //将成员变量照协议规定长度位置拷入bytes中
                    //
                    return bytes;
                }

            }
        }

        #region 成员函数

        public static void Init()
        {
            ACMCommandID.Add(124,(Action)Parse124);
            CommPara.Type = DataType.MPSK;
            CommPara.LinkOrient = true;

        }
        /// <summary>
        /// 得到解调数据
        /// </summary>
        /// <param name="bytes"></param>
        public static void GetBytes(byte[] bytes)
        {
            decodeBytes = bytes;
        }
        public static void PackData()
        {
            int byteslength = 0;
            switch (CommPara.Type)
            {
                case DataType.MPSK:
                    if (CommPara.LinkOrient)
                    {
                        
                    }
                    else
                    {
                        
                    }
                    break;
                case DataType.QPSK:
                    if (CommPara.LinkOrient)
                    {

                    }
                    else
                    {

                    }
                    break;
                default:
                    break;
            }
            dataBytes = new byte[byteslength];
        }
         
        #endregion

        #region 协议解析
        public static int Parse()
        {
            Errormessage = String.Empty;
            var id = 0;
            try
            {
                id = BitConverter.ToInt16(decodeBytes, 0);
                //命令列表中存在ID，则处理，不存在，则返回ID，外部处理
                if (ACMCommandID.ContainsKey(id))
                    Task.Factory.StartNew((Action)ACMCommandID[id]);
                return id;
            }
            catch (Exception e)
            {
                Errormessage = e.Message;
                id = 0;
                return id;
            }
            
        }
        #endregion

        #region 命令处理函数

        static  void Parse124()
        {
            
        }
        #endregion
    }
}

using System;
using System.Collections;
using System.Linq.Expressions;

namespace BoonieBear.DeckUnit.CommLib.Protocol
{
    public class ACMProtocol
    {
        private static readonly Hashtable ACMCommandID = new Hashtable();
        private static byte[] dataBytes;//打包数据
        private static byte[] decodeBytes;//解调数据
        private static int ParaLength = 135;
        public static string Errormessage { get; private set; }
        public enum CommType
        {
            MPSK=0,
            QPSK,
            QAM
        }
        //some initial para
        public class CommPara
        {
            public static CommType Type;
            public static bool LinkOrient;//true:uplink,false:downlink

            //初始化参数属性
            public static byte[] PARA 
            {
                get
                {
                    var bytes = new byte[ParaLength];
                    //将成员变量照协议规定长度位置拷入bytes中
                    //
                    return bytes;
                }

            }
        }

        #region 成员函数

        public static void Init()
        {
            ACMCommandID.Add(113,"命令");
            CommPara.Type = CommType.MPSK;
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
        public static void InitForPack()
        {
            int byteslength = 0;
            switch (CommPara.Type)
            {
                case CommType.MPSK:
                    if (CommPara.LinkOrient)
                    {
                        
                    }
                    else
                    {
                        
                    }
                    break;
                case CommType.QPSK:
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
        public static 
        #endregion

        #region 数据成员类

        class USBL
        {
            public static double Lat { get; private set; }
            public static double Lng { get; private set; }
            public static bool bFlag = false;
            public static bool Parse(byte[] data,int index)
            {
                //do the parse job
                try
                {
                    bFlag = true;
                }
                catch (Exception e)
                {

                    Errormessage += e.Message;
                    bFlag = false;
                }
                
                return bFlag;
                
            }
        }
        #endregion

        #region 协议解析
        bool Parse()
        {
            Errormessage = String.Empty;
            USBL.Parse(decodeBytes,12);
            return true;
        }
        #endregion
    }
}

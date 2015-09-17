using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoonieBear.DeckUnit.ACMP
{
    public class ACM4500Protocol
    {
        private List<byte[]> mpskpkg; 
        private static ShipGatherData _shipdataEngine;
        private static UWVGatherData _uwvdataEngine;
        public static string Errormessage { get; private set; }

        //some initial para
        public class CommPara
        {
            public static ModuleType Type;
            public static bool LinkOrient;//true:uplink,false:downlink

        }

        #region 成员函数

        public static void Init()
        {
            CommPara.Type = ModuleType.MFSK;
            CommPara.LinkOrient = true;
            _shipdataEngine = ShipGatherData.GetInstance();
            _uwvdataEngine = UWVGatherData.GetInstance();
        }

        public static void PackMixData(byte[] bytes)
        {
            int byteslength = 0;
            switch (CommPara.Type)
            {
                case ModuleType.MFSK:
                    if (CommPara.LinkOrient)
                    {
                        
                    }
                    else
                    {
                        
                    }
                    break;
                case ModuleType.MPSK:
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

        }
         
        #endregion

        #region 协议解析
        public static int ParseBP(byte[] decodeBytes)
        {
            Errormessage = String.Empty;
            var id = 0;
            try
            {
                id = BitConverter.ToInt16(decodeBytes, 0);

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


        public static void GetBytes(byte[] p)
        {
            throw new NotImplementedException();
        }

        public static object Parse()
        {
            throw new NotImplementedException();
        }
    }
}

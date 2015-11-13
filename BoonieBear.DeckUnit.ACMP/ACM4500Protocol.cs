using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BoonieBear.DeckUnit.ACMP
{
    public class ACM4500Protocol
    {
        private List<byte[]> mpskpkg; //mpsk包，3个凑齐一个完整的psk数据
        //母船或是潜器数据池，每次启动只会用其中一个
        private static ShipGatherData _shipdataPool;
        private static UWVGatherData _uwvdataPool;
        private static byte[] pkg;//指向需要解析的数据
        public static string Errormessage { get; private set; }
        private static Hashtable ParsedInfoHT = new Hashtable();
        private static MonitorMode Mode = MonitorMode.SUBMARINE;

        #region 成员函数

        public static void Init(MonitorMode mode = MonitorMode.SUBMARINE)
        {
            Mode = mode;
            _shipdataPool = ShipGatherData.GetInstance();
            _uwvdataPool = UWVGatherData.GetInstance();
            _shipdataPool.Clean();
            _uwvdataPool.Clean();
            ParsedInfoHT.Clear();
            Errormessage = string.Empty;
        }

        /// <summary>
        /// 打包协议数据，如果是FH或是SSB的话，使用dataOfFhorSSB作为源数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataOfFhorSSB"></param>
        /// <returns></returns>
        public static byte[] PackData(ModuleType type,byte[] dataOfFhorSSB = null)
        {
            switch (type)
            {
                case ModuleType.MFSK:
                    if (Mode == MonitorMode.SUBMARINE)
                    {
                        return _uwvdataPool.PackageMFSKBytes;
                    }
                    else
                    {
                        return _shipdataPool.PackageMFSKBytes;
                    }
                    break;
                case ModuleType.MPSK:
                    if (Mode == MonitorMode.SUBMARINE)
                    {
                        return _uwvdataPool.PackageMPSKBytes;
                    }
                    else
                    {
                        Errormessage = "不支持的调制类型";
                        return null;
                    }
                    break;
                case ModuleType.FH://上下行无区别
                    //暂无处理
                    return dataOfFhorSSB;
                case ModuleType.SSB:
                    return dataOfFhorSSB;
                case ModuleType.OFDM:
                    return null;
                default:
                    return null;
            }
            return null;
        }
         
        #endregion


        public static void GetBytes(byte[] pkgBytes)
        {
            pkg = pkgBytes;
        }

        public static bool Parse()
        {
            if (pkg != null)
            {
                if (Mode == MonitorMode.SHIP)
                {
                    
                }
                else
                {
                    Sysposition postion = new Sysposition();
                    postion.Parse(pkg);
                    ParsedInfoHT.Add((int)MovDataType.ALLPOST, postion);
                    string msg = Encoding.Default.GetString(pkg, 40, 40);
                }
                return true;
            }
            return false;
        }
        public static IEnumerator ReturnInfoPool()
        {
            if (ParsedInfoHT.Count > 0)
                return ParsedInfoHT.GetEnumerator();
            return null;

        }
    }
}

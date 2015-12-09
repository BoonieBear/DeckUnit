using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoonieBear.DeckUnit.ACMP
{
    public class ACM4500Protocol
    {
        private static  Dictionary<int,byte[]> mpskpkg; //mpsk包 72包
        private const int TotalPkg = 72;
        //母船或是潜器数据池，每次启动只会用其中一个
        public static ShipGatherData ShipdataPool;
        public static UWVGatherData UwvdataPool;
        public static string Errormessage { get; private set; }
        public static Hashtable Results = new Hashtable();
        private static MonitorMode Mode = MonitorMode.SUBMARINE;

        #region ACN protocol
        private enum ACNType
        {
            Router = 7,
            Data = 50
        }
        private static BitArray data;//将数据转换成bit数组，低位在前
        private static int index = 0;//累进解析器的下标位置。
        static private void GetDataForParse(byte[] d)//将byte[]->bit 为解析做准备
        {
            data = new BitArray(d);
            index = 0;
            mpskpkg.Clear();
        }
        static public int GetIntValueFromBit(int bitlen)
        {
            int[] value = new int[1];
            BitArray ba = new BitArray(bitlen);
            for (int i = 0; i < bitlen; i++)
            {
                ba[i] = data[index + i];
            }
            index += bitlen;

            ba.CopyTo(value, 0);
            return value[0];
        }
        static public Byte[] GetByteValueFromBit(int bitlen)
        {
            Byte[] value = new Byte[(int)Math.Ceiling((double)bitlen / 8)];
            Array.Clear(value, 0, (int)Math.Ceiling((double)bitlen / 8));
            BitArray ba = new BitArray(bitlen);
            for (int i = 0; i < bitlen; i++)
            {
                ba[i] = data[index + i];
            }
            index += bitlen;

            ba.CopyTo(value, 0);
            return value;
        }
        #endregion

        #region 成员函数

        public static void SetMode(MonitorMode mode)
        {
            Mode = mode;
            ShipdataPool.Clean();
            UwvdataPool.Clean();
            Results.Clear();
            Errormessage = string.Empty;
        }
        public static void Init(MonitorMode mode = MonitorMode.SUBMARINE)
        {

            ShipdataPool = ShipGatherData.GetInstance();
            UwvdataPool = UWVGatherData.GetInstance();
            SetMode(mode);
        }

        /// <summary>
        /// 打包协议数据，如果是FH或是SSB的话，直接将原数据编码
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte[] PackData(ModuleType type)
        {
            switch (type)
            {
                case ModuleType.MFSK:
                    if (Mode == MonitorMode.SUBMARINE)
                    {
                        return UwvdataPool.PackageMFSKBytes;
                    }
                    else
                    {
                        return ShipdataPool.PackageMFSKBytes;
                    }
                    break;
                case ModuleType.MPSK:
                    if (Mode == MonitorMode.SUBMARINE)
                    {
                        return UwvdataPool.PackageMPSKBytes;
                    }
                    else
                    {
                        Errormessage = "不支持的调制类型";
                        return null;
                    }
                    break;
                
                case ModuleType.OFDM:
                    return null;
                default:
                    return null;
            }
            return null;
        }
         
        #endregion

        /// <summary>
        /// 解码fsk，psk外的acn打包
        /// </summary>
        /// <returns></returns>
        public static byte[] DecodeACNData(byte[] encBytes)
        {
            index = 0;
            byte[] result = null;
            int blocks = 0;//总块数
            try
            {
                //解块
                int blocknum = GetIntValueFromBit(6);
                
                for (int i = 0; i < blocknum; i++)
                {

                    //块定义
                    int blockid = GetIntValueFromBit(10);
                    
                    int blocklen = GetIntValueFromBit(12);
                    
                    int StartId = GetIntValueFromBit(6);
                    
                    int EndId = GetIntValueFromBit(6);
                    
                    int j = 34; //长度加两个地址长度

                    while (j < blocklen)
                    {
                        //解析数据区
                        int sectorId = GetIntValueFromBit(8);
                        if (sectorId == 0) //结束标识
                        {
                            j += 8; //只有8bit长
                        }
                        else
                        { 
                            int len = GetIntValueFromBit(12);
                            j += len;
                            int key;
                            switch (sectorId)
                            {
                                case (int) ACNType.Router:
                                    int source = GetIntValueFromBit(8);
                                    break;
                                case (int) ACNType.Data:
                                    blocks  = GetIntValueFromBit(8);
                                    
                                    int current = GetIntValueFromBit(8);
                                    byte[] currentblock = GetByteValueFromBit(len - 36);
                                    if (blocks != 1) //psk
                                    {
                                        if(current==0)
                                            mpskpkg.Clear();
                                        mpskpkg.Add(current, currentblock);
                                    }
                                    if (result == null) //first block;
                                        result = currentblock;
                                    else
                                        result = result.Concat(currentblock).ToArray();
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Errormessage = e.Message + ":" + e.StackTrace;
                throw new Exception(Errormessage);
                return null;
            }
            return result;
        }

        //解析tcp回传数据，需要先调用decode解码fsk。psk，false表示出错
        public static bool ParseFSK(byte[] pkg)
        {
            if (pkg != null)
            {
                Results.Clear();

                if (Mode == MonitorMode.SHIP)
                {
                    var Sub = new Subposition();
                    byte[] data = new byte[26];
                    Buffer.BlockCopy(pkg, 0, data, 0, 26);
                    Sub.Parse(data);
                    Results.Add(MovDataType.SUBPOST, Sub);

                    var bp = new Bpdata();
                    data = new byte[18];
                    Buffer.BlockCopy(pkg, 26, data, 0, 18);
                    bp.Parse(data);
                    Results.Add(MovDataType.BP, bp);

                    var bsss = new Bsssdata();
                    data = new byte[6];
                    Buffer.BlockCopy(pkg, 44, data, 0, 6);
                    bsss.Parse(data);
                    Results.Add(MovDataType.BSSS, bsss);

                    var adcp = new Adcpdata();
                    data = new byte[34];
                    Buffer.BlockCopy(pkg, 50, data, 0, 34);
                    adcp.Parse(data);
                    Results.Add(MovDataType.ADCP, adcp);

                    var ctd = new Ctddata();
                    data = new byte[16];
                    Buffer.BlockCopy(pkg, 84, data, 0, 16);
                    ctd.Parse(data);
                    Results.Add(MovDataType.CTD, ctd);

                    var life = new Lifesupply();
                    data = new byte[14];
                    Buffer.BlockCopy(pkg, 100, data, 0, 14);
                    life.Parse(data);
                    Results.Add(MovDataType.LIFESUPPLY, life);

                    var eng = new Energysys();
                    data = new byte[34];
                    Buffer.BlockCopy(pkg, 114, data, 0, 34);
                    eng.Parse(data);
                    Results.Add(MovDataType.ENERGY, eng);

                    var alt = new Alertdata();
                    data = new byte[20];
                    Buffer.BlockCopy(pkg, 148, data, 0, 20);
                    alt.Parse(data);
                    Results.Add(MovDataType.ALERT, alt);
                    string msg = Encoding.Default.GetString(pkg, 168, 40);
                    Results.Add(MovDataType.WORD, msg);

                }
                else
                {
                    Sysposition postion = new Sysposition();
                    postion.Parse(pkg);
                    Results.Add(MovDataType.ALLPOST, postion);
                    string msg = Encoding.Default.GetString(pkg, 40, 40);
                    Results.Add(MovDataType.WORD, msg);
                }


                return true;
            }
            return false;
        }

        public static byte[] ParsePSK(byte[] pkg)
        {
            if (pkg != null)
            {
                Results.Clear();
                //only ship need to parse psk data
                if (mpskpkg.Count == TotalPkg) //full,pkg must be the last pkg
                {
                    byte[] dataBytes = null;
                    foreach (var block in mpskpkg)
                    {
                        if (dataBytes == null)
                            dataBytes = block.Value;
                        else
                            dataBytes = dataBytes.Concat(block.Value).ToArray();
                    }
                    var Sub = new Subposition();
                    byte[] data = new byte[26];
                    Buffer.BlockCopy(dataBytes, 0, data, 0, 26);
                    Sub.Parse(data);
                    Results.Add(MovDataType.SUBPOST, Sub);

                    var bp = new Bpdata();
                    data = new byte[18];
                    Buffer.BlockCopy(dataBytes, 26, data, 0, 18);
                    bp.Parse(data);
                    Results.Add(MovDataType.BP, bp);

                    var bsss = new Bsssdata();
                    data = new byte[6];
                    Buffer.BlockCopy(dataBytes, 44, data, 0, 6);
                    bsss.Parse(data);
                    Results.Add(MovDataType.BSSS, bsss);

                    var adcp = new Adcpdata();
                    data = new byte[34];
                    Buffer.BlockCopy(dataBytes, 50, data, 0, 34);
                    adcp.Parse(data);
                    Results.Add(MovDataType.ADCP, adcp);

                    var ctd = new Ctddata();
                    data = new byte[16];
                    Buffer.BlockCopy(dataBytes, 84, data, 0, 16);
                    ctd.Parse(data);
                    Results.Add(MovDataType.CTD, ctd);

                    var life = new Lifesupply();
                    data = new byte[14];
                    Buffer.BlockCopy(dataBytes, 100, data, 0, 14);
                    life.Parse(data);
                    Results.Add(MovDataType.LIFESUPPLY, life);

                    var eng = new Energysys();
                    data = new byte[34];
                    Buffer.BlockCopy(dataBytes, 114, data, 0, 34);
                    eng.Parse(data);
                    Results.Add(MovDataType.ENERGY, eng);

                    var alt = new Alertdata();
                    data = new byte[20];
                    Buffer.BlockCopy(dataBytes, 148, data, 0, 20);
                    alt.Parse(data);
                    Results.Add(MovDataType.ALERT, alt);
                    string msg = Encoding.Default.GetString(pkg, 168, 40);
                    Results.Add(MovDataType.WORD, msg);
                    byte[] img = new byte[MovGlobalVariables.ImgSize];
                    Buffer.BlockCopy(dataBytes, MovGlobalVariables.MFSKSize, img, 0, MovGlobalVariables.ImgSize);
                    return dataBytes;
                }
                
            }
            return null;
        }

        public static bool ParseFH(byte[] pkg)
        {
            string msg = Encoding.Default.GetString(pkg, 0, 8);
            Results.Clear();
            Results.Add(MovDataType.WORD, msg);
            return true;
        }

        public static bool ParseOFDM(byte[] pkg)
        {
            return true;
        }
        public static bool ParseSSB(byte[] pkg)//acoustic algorithm can be added
        {
            return true;
        }
    }
}

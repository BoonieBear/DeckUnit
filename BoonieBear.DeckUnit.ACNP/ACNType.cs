using System;
using System.Collections;
using System.Collections.Generic;
using TinyMetroWpfLibrary.Utility;
namespace BoonieBear.DeckUnit.ACNP
{
    #region 枚举成员
        public enum DeviceAddr
        {
            无设备 = 0,
            声学所浮标 = 1,
            SBE39_TD = 16,
            SBE37_CTD = 17,
            ZJU_AUV_CTD = 18,
            IOA_ADCP = 19,
            ZJU_AUV = 101,

        }
        public enum NodeType
        {
            声学所节点 = 0,
            _715通信机 = 4,
            哈工程通信机 = 8,

        }
        public enum EmitType
        {
            PWM发射机 = 0,
            线性发射机 = 1,
        }
        public enum RouteStatus
        {
            路由有效 = 0,
            路由无效 = 1,
            路由正在修复 = 2,
            非法状态标志 = 3,

        }
        public enum ChannlValue
        {
            良好 = 0,
            较好 = 1,
            较差 = 2,
            恶劣 = 3,

        }
        #endregion

    #region 协议成员类
        //{"结束标识","分段标识","邻节点表","网络表","网络简表","路由表","路径记录","路径安排",
        //                          "路径中断","转发失败"};
        //节点信息类
        //节点ID、节点能量、节点支持的通信制式、经纬度和深度
        public class Nodeinfo
        {
            public int NodeId;
            public int NodePower;
            public int CommType;
            public int Lang;
            public int Lat;
            public int depth;
            public int NodeType;
            public int RecvNum;
            public int Set1type;
            public int Set2type;
            public Nodeinfo(int id, int nodetype, int Num, int set1, int set2, int power, int type, int lang, int lat, int deep)
            {
                NodeId = id;
                NodeType = nodetype;
                RecvNum = Num;
                Set1type = set1;
                Set2type = set2;
                NodePower = power;
                CommType = type;
                Lang = lang;
                Lat = lat;
                depth = deep;
            }
            public string NodeID
            {
                get { return NodeId.ToString(); }

            }
            public string MoveType
            {
                get { return (NodeType == 1) ? "移动节点" : "静止节点"; }
            }
            public string Receiver
            {
                get { return RecvNum.ToString(); }
            }

            public string Set1Type
            {
                get
                {
                    return Enum.GetName(typeof(DeviceAddr), Set1type);
                }

            }
            public string Set2Type
            {
                get
                {
                    return Enum.GetName(typeof(DeviceAddr), Set2type);
                }
            }
            public string NodePW
            {
                get
                {
                    switch (NodePower)
                    {
                        case 0:
                            return @"<5%";
                        case 1:
                            return @"5%~20%";
                        case 2:
                            return @"20%~35%";
                        case 3:
                            return @"35%~50%";
                        case 4:
                            return @"50%~65%";
                        case 5:
                            return @"65%~80%";
                        case 6:
                            return @"80%~95%";
                        case 7:
                            return @">95%";
                    }
                    return @"NULL";

                }

            }
            public string Type
            {
                get
                {
                    string comstr = "";
                    byte[] b = BitConverter.GetBytes((short)CommType);
                    BitArray ba = new BitArray(b);
                    for (int i = 0; i < ba.Count; i++)
                    {
                        comstr += ba[i] ? "1" : "0";
                    }
                    return comstr.PadRight(16, '0');
                }

            }
            public string Langtude
            {

                get
                {
                    string str;
                    if (Lang >> 27 == 1)//西经
                    {
                        Lang &= 0x7ffffff;
                        double d = (double)Lang / 10000 / 60;
                        str = d.ToString() + "°W";
                    }
                    else//东经
                    {
                        Lang &= 0x7ffffff;
                        double d = (double)Lang / 10000 / 60;
                        str = d.ToString() + "°E";
                    }


                    return str;
                }

            }
            public string Latitude
            {
                get
                {
                    string str;
                    if (Lat >> 27 == 1)//南纬
                    {
                        Lat &= 0x7ffffff;
                        double d = (double)Lat / 10000 / 60;
                        str = d.ToString() + "°S";
                    }
                    else//北纬
                    {
                        Lat &= 0x7ffffff;
                        double d = (double)Lat / 10000 / 60;
                        str = d.ToString() + "°N";
                    }


                    return str;
                }

            }
            public string Depth
            {
                get
                {
                    return (depth * 0.5).ToString();
                }

            }


        }

        //邻节点信息类
        //邻节点ID、节点距离、信道评价
        public class NeiborNodeinfo
        {
            public int NodeId;
            public int Nodedist;
            public int ChannelEstimate;

            public NeiborNodeinfo(int id, int distance, int rate)
            {
                NodeId = id;
                Nodedist = distance;
                ChannelEstimate = rate;

            }
            public int NodeID
            {
                set { NodeId = value; }
                get { return NodeId; }

            }
            public double Distance
            {
                get
                {

                    return (double)Nodedist * 0.2;

                }

            }
            public string ChanEsti
            {
                get
                {
                    switch (ChannelEstimate)
                    {
                        case 0:
                            return @"信道条件良好，可支持高速通信";
                        case 1:
                            return @"信道条件较好，可支持中速通信";
                        case 2:
                            return @"信道条件较差，需降低通信速率";
                        case 3:
                            return @"信道条件恶劣，需采用高可靠性的通信";
                        default:
                            return @"错误的状态";
                    }
                }

            }


        }

        //网络表
        //节点的连接关系
        public class NetworkList
        {
            public int SourceNodeId;
            public int DestinNodeId;
            public int Nodedist;
            public int ChannelEstimate;
            public NetworkList(int sid, int did, int distance, int rate)
            {
                SourceNodeId = sid;
                DestinNodeId = did;
                Nodedist = distance;
                ChannelEstimate = rate;

            }
            public int SourceNodeID
            {
                set { SourceNodeId = value; }
                get { return SourceNodeId; }

            }
            public int DestinationNodeID
            {
                set { DestinNodeId = value; }
                get { return DestinNodeId; }

            }
            public double Distance
            {
                get
                {

                    return (double)Nodedist * 0.2;

                }

            }
            public string ChanEsti
            {
                get
                {
                    switch (ChannelEstimate)
                    {
                        case 0:
                            return @"信道条件良好，可支持高速通信";
                        case 1:
                            return @"信道条件较好，可支持中速通信";
                        case 2:
                            return @"信道条件较差，需降低通信速率";
                        case 3:
                            return @"信道条件恶劣，需采用高可靠性的通信";
                        default:
                            return @"错误的状态";
                    }
                }

            }
        }

        //路由表
        //源节点到各目标节点的路由，包括目标节点、下一跳地址和跳数
        public class RourteList
        {
            public int DNodeId;
            public int NextNodeId;
            public int Hops;
            public int DestSerial;
            public int RouteStatus;
            public RourteList(int did, int nid, int hop, int Serial, int Status)
            {
                DNodeId = did;
                NextNodeId = nid;
                Hops = hop;
                DestSerial = Serial;
                RouteStatus = Status;
            }
            public int DestinationNodeID
            {
                get { return DNodeId; }
            }
            public int NextNodeID
            {
                get { return NextNodeId; }
            }
            public int Hop
            {
                get { return Hops; }
            }
            public string Status
            {
                get
                {
                    switch (RouteStatus)
                    {
                        case 0:
                            return @"路由有效";
                        case 1:
                            return @"路由无效";
                        case 2:
                            return @"路由正在修复";
                        case 3:
                            return @"非法状态标志";
                        default:
                            return @"错误的状态";
                    }


                }
            }
            public int Serial
            {
                get { return DestSerial; }
            }

        }

        //路径记录
        //记录了所在数据块已经过的节点有哪些
        class RouterWriteLine
        {
            int StartId;
            List<int> IdWriteLine;
            public RouterWriteLine(int Startid, List<int> id)
            {
                StartId = Startid;
                IdWriteLine = id;
            }
            public int BeginID
            { get { return StartId; } }
            public List<int> ViaID
            { get { return IdWriteLine; } }
        }

        //路径安排
        //“路径安排”是“起始节点”对数据块按照什么样的路径进行传输的要求
        class RouterAssign
        {
            int StartId;
            List<int> IdWriteLine;
            int EndId;
            public RouterAssign(int Startid, List<int> id, int Endid)
            {
                StartId = Startid;
                IdWriteLine = id;
                EndId = Endid;
            }
            public int BeginID
            { get { return StartId; } }
            public List<int> ViaID
            { get { return IdWriteLine; } }
            public int EndID
            { get { return EndId; } }
        }

        //路径中断
        //“路径中断”是在路径中断后，判定路径中断的节点向网关发送的错误报告
        class RouterBroken
        {
            int StartId;
            //List<int> IdWriteLine;
            int EndId;
            public RouterBroken(int Startid, List<int> id, int Endid)
            {
                StartId = Startid;
                //IdWriteLine = id;
                EndId = Endid;
            }
            public int BeginID
            { get { return StartId; } }
            //public List<int> ViaID
            //{ get { return IdWriteLine; } }
            public int EndID
            { get { return EndId; } }
        }

        //转发失败
        //“转发失败”是节点在尝试各可选路径后均无法完成转发任务后，向中继链路的上一个节点的发送的错误报告
        class TransError
        {

            List<TransErrorBlock> ErrorBlock;

            public TransError(List<TransErrorBlock> Eb)
            {
                ErrorBlock = Eb;
            }
            public List<TransErrorBlock> ErrorReport
            { get { return ErrorBlock; } }

        }

        //转发失败数据体由成对的块标识和起始源地址组成
        class TransErrorBlock
        {
            int ErrNum;
            int StartId;
            public TransErrorBlock(int num, int sid)
            {
                ErrNum = num;
                StartId = sid;
            }
            public int ERRNO
            { get { return ErrNum; } }
            public int BeginID
            { get { return StartId; } }
        }
        #endregion

    #region 设备相关

    public interface IMSPState
    {
        string Contruct();
        bool Parse(string HexStr);
    }
    public struct Power48VState:IMSPState
    {
        private int _lowCurrentTime;
        private int _mediumCurrentTime;
        private int _highCurrentTime;
        public int LowCurrentTime
        {
            get { return _lowCurrentTime; }
            set { _lowCurrentTime = value; }
        }

        public int MediumCurrentTime
        {
            get { return _mediumCurrentTime; }
            set { _mediumCurrentTime = value; }
        }

        public int HighCurrentTime
        {
            get { return _highCurrentTime; }
            set { _highCurrentTime = value; }
        }

        public string Contruct()
        {
            return _lowCurrentTime.ToString("0000000000") + _mediumCurrentTime.ToString("0000000000") +
                   _highCurrentTime.ToString("0000000000");
        }

        public bool Parse(string HexStr)
        {
            if (HexStr.Length != 30)
                return false;
            _lowCurrentTime = Int32.Parse(HexStr.Substring(0, 10));
            _mediumCurrentTime = Int32.Parse(HexStr.Substring(10, 10));
            _highCurrentTime = Int32.Parse(HexStr.Substring(20, 10));
            return true;
        }

    }

    public struct MSPWorkState:IMSPState
    {
        private int sleeptime;
        private int worktime;

        public int Sleeptime
        {
            get { return sleeptime; }
            set { sleeptime = value; }
        }

        public int Worktime
        {
            get { return worktime; }
            set { worktime = value; }
        }

        public string Contruct()
        {
            return sleeptime.ToString("00000") + worktime.ToString("00000");
        }

        public bool Parse(string HexStr)
        {
            if (HexStr.Length != 10)
                return false;
            sleeptime = Int32.Parse(HexStr.Substring(0, 6));
            worktime = Int32.Parse(HexStr.Substring(6, 6));
            return true;
        }
        
    }
    
    public struct PWState : IMSPState
    {
        private double  _voltage33;
        private double _voltage48;
        private double _pw48Left;
        private double _pw48Consume;
        private double _pw33Left;
        private double _pw33Consume;

        public double Voltage33
        {
            get { return _voltage33; }
            set { _voltage33 = value; }
        }

        public double Voltage48
        {
            get { return _voltage48; }
            set { _voltage48 = value; }
        }

        public double Pw48Left
        {
            get { return _pw48Left; }
            set { _pw48Left = value; }
        }

        public double Pw48Consume
        {
            get { return _pw48Consume; }
            set { _pw48Consume = value; }
        }

        public double Pw33Left
        {
            get { return _pw33Left; }
            set { _pw33Left = value; }
        }

        public double Pw33Consume
        {
            get { return _pw33Consume; }
            set { _pw33Consume = value; }
        }

        
        public string Contruct()
        {
            string cmd;
            try
            {
                cmd = StringHexConverter.GetFormedString(Voltage33.ToString(), 1, 4) + StringHexConverter.GetFormedString(Voltage48.ToString(), 3, 6) + StringHexConverter.GetFormedString(Pw48Left.ToString(), 5, 8) +
                    StringHexConverter.GetFormedString(Pw48Consume.ToString(), 5, 8) + StringHexConverter.GetFormedString(Pw33Left.ToString(), 5, 8) + StringHexConverter.GetFormedString(Pw33Consume.ToString(), 5, 8);
            }
            catch (Exception e)
            {
                    
                throw e;
            }
            return cmd;
        }

        public bool Parse(string HexStr)
        {
            bool ret = false;
            try
            {
                Voltage33 = double.Parse(HexStr.Substring(0, 4)) / 1000;
                Voltage48 = double.Parse(HexStr.Substring(4, 6))/1000;
                _pw48Left = double.Parse(HexStr.Substring(10, 8))/1000;
                Pw48Consume = double.Parse(HexStr.Substring(18, 8))/1000;
                Pw33Left = double.Parse(HexStr.Substring(26, 8))/1000;
                Pw33Consume = double.Parse(HexStr.Substring(34, 8))/1000;
                ret = true;
            }
            catch (Exception)
            {
                ret = false;
            }
            return ret;
        }
    }

    public struct ModermConfig:IMSPState
    {
        private UInt32 buoyid;
        private UInt32 nodeid;
        private double lang;
        private double lati;
        private UInt32 com2device;
        private UInt32 com3device;
        private UInt32 emittype;
        private UInt32 netswitch;
        private UInt32 emitnum;
        private UInt32 nodetype;
        private UInt32 accessmode;
        public uint Buoyid
        {
            get { return buoyid; }
            set { buoyid = value; }
        }

        public uint Nodeid
        {
            get { return nodeid; }
            set { nodeid = value; }
        }

        public double Lang//120.123213°
        {
            get { return lang; }
            set { lang = value; }
        }

        public double Lati
        {
            get { return lati; }
            set { lati = value; }
        }

        public uint Com2Device
        {
            get { return com2device; }
            set { com2device = value; }
        }

        public uint Com3Device
        {
            get { return com3device; }
            set { com3device = value; }
        }

        public uint Emittype
        {
            get { return emittype; }
            set { emittype = value; }
        }

        public uint Netswitch
        {
            get { return netswitch; }
            set { netswitch = value; }
        }

        public uint Emitnum
        {
            get { return emitnum; }
            set { emitnum = value; }
        }

        public uint Nodetype
        {
            get { return nodetype; }
            set { nodetype = value; }
        }

        public uint Accessmode
        {
            get { return accessmode; }
            set { accessmode = value; }
        }

        //生成全部属性数据
        public string Contruct()
        {

            string strLang = Lang.ToString();
            string[] split = strLang.Split('.');
            strLang = split[0].PadLeft(4, '0');
            strLang += ((Lang - double.Parse(split[0])) * 60).ToString("F04").Replace(".", "").PadLeft(6, '0');

            string strLat = Lati.ToString();
            string[] splitlat = strLat.Split('.');
            strLat = splitlat[0].PadLeft(2, '0');
            strLat += ((Lati - double.Parse(splitlat[0])) * 60).ToString("F04").Replace(".", "").PadLeft(6, '0');
            if (Lang < 0)
                strLang = "01" +strLang;
            else
            {
                strLang = "00" +strLang;
            }
            if (Lati < 0) //南纬
                strLat = "01" + strLat;
            else
            {
                strLat = "00" + strLat;
            }
            return Buoyid.ToString("00") + Nodeid.ToString("00") + strLang + strLat + Com2Device.ToString("0000") +
                   Com3Device.ToString("0000") + Emittype.ToString("00") + Netswitch.ToString("00") +
                   Emitnum.ToString("00") + Nodetype.ToString("00") + Accessmode.ToString("00");
        }
        //解析,注：数据中没有网络开关和节点类型
        public bool Parse(string HexStr)
        {
            bool ret = true;
            try
            {
                Buoyid = UInt32.Parse(HexStr.Substring(0, 2));
                Nodeid = UInt32.Parse(HexStr.Substring(2, 4));
                string sn = HexStr.Substring(4, 2);
                if (sn == "00")
                    Lang = double.Parse(HexStr.Substring(6, 4)) +
                           double.Parse(HexStr.Substring(10, 2) + "." + HexStr.Substring(12, 4)) / 60;
                else if (sn == "01")
                    Lang = -double.Parse(HexStr.Substring(6, 4)) -
                           double.Parse(HexStr.Substring(10, 2) + "." + HexStr.Substring(12, 4)) / 60;
                else
                    Lang = 0;

                sn = HexStr.Substring(16, 2);
                if (sn == "00")
                    Lati = double.Parse(HexStr.Substring(18, 2)) +
                           double.Parse(HexStr.Substring(20, 2) + "." + HexStr.Substring(22, 4)) / 60;
                else if (sn == "01")
                    Lati = -double.Parse(HexStr.Substring(18, 2)) -
                           double.Parse(HexStr.Substring(20, 2) + "." + HexStr.Substring(22, 4)) / 60;
                else
                    Lati = 0;
                Com2Device = uint.Parse(HexStr.Substring(26, 4));
                Com3Device = uint.Parse(HexStr.Substring(30, 4));
                Emittype = uint.Parse(HexStr.Substring(34, 2));
                Emitnum = uint.Parse(HexStr.Substring(36, 2));
            }
            catch (Exception)
            {
                ret = false;
            }
            return ret;
            
        }
    }
    public struct RTCTime:IMSPState
    {
        public DateTime DtTime { get; set; }

        public string Contruct()
        {
            return DtTime.Year.ToString("0000") + DtTime.Month.ToString("00") + DtTime.Day.ToString("00") +
                   DtTime.Hour.ToString("00") + DtTime.Minute.ToString("00") + DtTime.Second.ToString("00");
        }

        public bool Parse(string HexStr)
        {
            bool ret = true;
            try
            {
                
                int Year = int.Parse(HexStr.Substring(0, 4));
                int Month = int.Parse(HexStr.Substring(4, 2));
                int Day = int.Parse(HexStr.Substring(6, 2));
                int Hour = int.Parse(HexStr.Substring(8, 2));
                int Minute = int.Parse(HexStr.Substring(10, 2));
                int Second = int.Parse(HexStr.Substring(12, 2));
                DtTime = new DateTime(Year, Month, Day, Hour, Minute, Second);
            }
            catch (Exception)
            {
                ret = false;
            }
            return ret;
        }
    }
    public struct ModermState:IMSPState
    {
        public RTCTime RtcTime { get; private set; }

        public ModermConfig Config { get; set; }

        public Power48VState Power48VState { get; private set; }

        public MSPWorkState MspWorkState { get; private set; }

        public PWState PwState { get; private set; }

        public double Temprature { get; private set; }

        //0:喂狗关闭
        public bool Bdog { get; private set; }

        public int AdLevel { get; private set; }

        public uint Com2Wakeuptime { get; private set; }

        public uint Com3Wakeuptime { get; private set; }

        public uint Reboottimes { get; private set; }

        //0:休眠
        public bool BbuoyOn { get; private set; }

        private string version;
        public string Version 
        {
            get
            {
                return (double.Parse(version.Substring(0, 4))/1000).ToString("F03") + " " +
                       version.Substring(4, 4) + "-" + version.Substring(8, 2) + "-"
                       + version.Substring(10, 2);
            }
            private set { version = value; } 
        }

        public string Contruct()//not used
        {
            throw new NotImplementedException();
        }

        public bool Parse(string HexStr)
        {
            bool ret = true;
            try
            {
                RtcTime.Parse(HexStr);
                Config.Parse(HexStr.Substring(14));
                Power48VState.Parse(HexStr.Substring(52));
                MspWorkState.Parse(HexStr.Substring(82));
                PwState.Parse(HexStr.Substring(94));
                int sgn = int.Parse(HexStr.Substring(136, 2));
                double temp = double.Parse(HexStr.Substring(138, 6))/1000;
                if (sgn == 1)
                    temp = -temp;
                Temprature = temp;
                Bdog = bool.Parse(HexStr.Substring(144, 2));
                AdLevel = int.Parse(HexStr.Substring(146, 2));
                Com2Wakeuptime = UInt32.Parse(HexStr.Substring(148, 10));
                Com3Wakeuptime = UInt32.Parse(HexStr.Substring(158, 10));
                Reboottimes = UInt32.Parse(HexStr.Substring(168, 4));
                BbuoyOn = bool.Parse(HexStr.Substring(172, 2));
                Version = HexStr.Substring(174, 12);
            }
            catch (Exception)
            {
                ret = false;
            }
            return ret;
        }
    }
   
    #endregion
}

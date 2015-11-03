using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace BoonieBear.DeckUnit.ACMP
{
    #region 枚举类型
     public enum ModuleType
        {
            MFSK = 0xda01,
            MPSK=0xda02,
            OFDM = 0xda03,
            SSB= 0xda04,
            FH=0xda05,
            END = 0xeded,
        }

    public enum MonitorMode
    {
        SHIP = 0,
        SUBMARINE = 1,
    }
    //数据类型，如果是潜器中交换数据，使用协议ID，其他的自定义
    public enum Mov4500DataType
    {
        ALLPOST = 0x2001,
        BP = 0x2002,
        SUBPOST = 0x1001,
        CTD = 0x1002,
        LIFESUPPLY = 0x1003,
        ENERGY = 0x1004,
        ALERT = 0x1005,
        ACUSTICALARM = 6,
        BSSS = 3,
        SUBALERT = 9,
        ADCP= 10,
        SWITCH = 11,
        SHIPPOST=12,
        SUBLASTEST5POST = 13,
        WORD = 14,
        IMAGE = 15,
        
    }
    #endregion

    public class MovGlobalVariables
    {
        public static int MFSKSize = 205;
        public static int ShipMFSKSize = 112;
        public static int MovMFSKSize = 131;
        public static int WordSize = 40;
        public static int ImgSize = 16560;
        public static int MPSKSize = 16560+282;
    }

    #region 调试数据结构
    class CommunicationParameter
    {
        private readonly int[] _gain;
        private readonly int[] _agcAmp;
        private readonly float[] _chirpCorrePeak;
        private readonly float[] _channCorreCoe;
        private readonly int[] _reserved;
        public  static UInt32 Length = 160;

        public CommunicationParameter()
        {
             _gain = new int[8];
            _agcAmp = new int[8];
            _chirpCorrePeak = new float[8];
            _channCorreCoe = new float[8];
            _reserved = new int[8];
        }
        public CommunicationParameter(byte[] databytes)
        {
            if (databytes.Length < Length)
            {
                throw new Exception("CommunicationParameter inital failed!");
            }
           
            Buffer.BlockCopy(databytes, 0, Gain, 0, 8 * sizeof(int));
            Buffer.BlockCopy(databytes, 8 * sizeof(int), AgcAmp, 0, 8 * sizeof(int));
            Buffer.BlockCopy(databytes, 2 * 8 * sizeof(int), ChirpCorrePeak, 0, 8 * sizeof(float));
            Buffer.BlockCopy(databytes, 2 * 8 * sizeof(int) + 8 * sizeof(float), ChannCorreCoe, 0, 8 * sizeof(float));
            Buffer.BlockCopy(databytes, 2 * 8 * sizeof(int) + 2 * 8 * sizeof(float), Reserved, 0, 8 * sizeof(int));

        }

        public int[] Gain
        {
            get { return _gain; }
        }

        public int[] AgcAmp
        {
            get { return _agcAmp; }
        }

        public float[] ChirpCorrePeak
        {
            get { return _chirpCorrePeak; }
        }

        public float[] ChannCorreCoe
        {
            get { return _channCorreCoe; }
        }

        public int[] Reserved
        {
            get { return _reserved; }
        }
    }


    #endregion 
 
    #region Mov4500设备数据
    //定位信息0x2001
    public class Sysposition 
    {
        private long _ltime;
        private float _shipLong;//母船经度
        private float _shipLat; //母船纬度
        private Int16 _shipvel;//母船速度
        private UInt16 _shipheading;//母船艏向角
        private Int16 _shippitch;//母船纵倾角
        private Int16 _shiproll;//母船横倾角
        private float _subLong; //潜水器经度
        private float _subLat;//潜水器纬度
        private UInt16 _subdepth;//潜水器深度
        private Int16 _relateX;//潜水器相对母船x轴位移
        private Int16 _relateY;//潜水器相对母船y轴位移
        private UInt16 _relateZ;//潜水器相对母船z轴位移	
        


        /// <summary>
        /// 按协议打包，无包头ID
        /// </summary>
        /// <returns></returns>
        public byte[] Pack()
        {
            byte[] storebyte = new byte[40];
            int offset = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(Ltime),0,storebyte,offset,sizeof(long));
            offset += sizeof (long);
            Buffer.BlockCopy(BitConverter.GetBytes(_shipLong),0,storebyte,offset,sizeof(float));
            offset += sizeof(float);
            Buffer.BlockCopy(BitConverter.GetBytes(_shipLat), 0, storebyte, offset, sizeof(float));
            offset += sizeof(float);
            Buffer.BlockCopy(BitConverter.GetBytes(_shipvel), 0, storebyte, offset, sizeof(Int16));
            offset += sizeof(Int16);
            Buffer.BlockCopy(BitConverter.GetBytes(_shipheading), 0, storebyte, offset, sizeof(UInt16));
            offset += sizeof(UInt16);
            Buffer.BlockCopy(BitConverter.GetBytes(_shippitch), 0, storebyte, offset, sizeof(Int16));
            offset += sizeof(Int16);
            Buffer.BlockCopy(BitConverter.GetBytes(_shiproll), 0, storebyte, offset, sizeof(Int16));
            offset += sizeof(Int16);
            Buffer.BlockCopy(BitConverter.GetBytes(_subLong), 0, storebyte, offset, sizeof(float));
            offset += sizeof(float);
            Buffer.BlockCopy(BitConverter.GetBytes(_subLat), 0, storebyte, offset, sizeof(float));
            offset += sizeof(float);
            Buffer.BlockCopy(BitConverter.GetBytes(_subdepth), 0, storebyte, offset, sizeof(UInt16));
            offset += sizeof(UInt16);
            Buffer.BlockCopy(BitConverter.GetBytes(_relateX), 0, storebyte, offset, sizeof(Int16));
            offset += sizeof(Int16);
            Buffer.BlockCopy(BitConverter.GetBytes(_relateY), 0, storebyte, offset, sizeof(Int16));
            offset += sizeof(Int16);
            Buffer.BlockCopy(BitConverter.GetBytes(_relateZ), 0, storebyte, offset, sizeof(UInt16));
            return storebyte;

        }


        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }
        public float ShipLong
        {
            get { return _shipLong; }
            set { _shipLong = value; }
        }
        public float ShipLat
        {
            get { return _shipLat; }
            set { _shipLat = value; }
        }
        public Int16 Shipvel
        {
            get { return _shipvel; }
            set { _shipvel = value; }
        }
        public UInt16 Shipheading
        {
            get { return _shipheading; }
            set { _shipheading = value; }
        }
        public Int16 Shippitch
        {
            get { return _shippitch; }
            set { _shippitch = value; }
        }
        public Int16 Shiproll
        {
            get { return _shiproll; }
            set { _shiproll = value; }
        }
        public float SubLong
        {
            get { return _subLong; }
            set { _subLong = value; }
        }
        public float SubLat
        {
            get { return _subLat; }
            set { _subLat = value; }
        }
        public UInt16 Subdepth
        {
            get { return _subdepth; }
            set { _subdepth = value; }
        }
        public Int16 RelateX
        {
            get { return _relateX; }
            set { _relateX = value; }
        }
        public Int16 RelateY
        {
            get { return _relateY; }
            set { _relateY = value; }
        }
        public UInt16 RelateZ
        {
            get { return _relateZ; }
            set { _relateZ = value; }
        }
        internal void Parse(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    };

    //避碰信息2002,
    public class Bpdata {
        private long _ltime;
        private UInt16		_frontup;//前上避碰声呐距离
        private UInt16		_front;//正前
        private UInt16		_frontdown;//前下
        private UInt16		_down;//正下
        private UInt16		_behinddown;//后下
        private UInt16		_left;//左下
        private UInt16		_right;//右下	
        private byte[] storebyte = new byte[22];

        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public ushort Frontup
        {
            get { return _frontup; }
            set { _frontup = value; }
        }

        public ushort Front
        {
            get { return _front; }
            set { _front = value; }
        }

        public ushort Frontdown
        {
            get { return _frontdown; }
            set { _frontdown = value; }
        }

        public ushort Down
        {
            get { return _down; }
            set { _down = value; }
        }

        public ushort Behinddown
        {
            get { return _behinddown; }
            set { _behinddown = value; }
        }

        public ushort Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public ushort Right
        {
            get { return _right; }
            set { _right = value; }
        }
        /// <summary>
        /// 用来打包数据给水声通信
        /// </summary>
        /// <returns></returns>
        public byte[] Pack()
        {
            return storebyte;
        }

        internal void Parse(byte[] bytes)
        {
            _ltime = BitConverter.ToInt64(bytes, 2);
            _frontup = BitConverter.ToUInt16(bytes, 10);
            _front = BitConverter.ToUInt16(bytes, 12);
            _frontdown = BitConverter.ToUInt16(bytes, 14);
            _down = BitConverter.ToUInt16(bytes, 16);
            _behinddown = BitConverter.ToUInt16(bytes, 18);
            _left = BitConverter.ToUInt16(bytes, 20);
            _right = BitConverter.ToUInt16(bytes, 22);
            Buffer.BlockCopy(bytes, 2, storebyte,0,22);
        }
    };

    //侧深侧扫信息
    public class Bsssdata {
        private long _ltime;
        private Int16 _depth;//距离海底高度
        byte[] storebyte = new byte[10];
        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public short Depth
        {
            get { return _depth; }
            set { _depth = value; }
        }

        public byte[] Pack()
        {
            return storebyte;
        }

        internal void Parse(byte[] bytes)
        {
            _ltime = BitConverter.ToInt64(bytes, 2);
            _depth = BitConverter.ToInt16(bytes, 10);
            Buffer.BlockCopy(bytes, 2, storebyte, 0,10);
        }
    };


    //CTD信息1002
    public class Ctddata {
        private long _ltime;
        private UInt16 _watertemp;//海水温度
        private UInt16 _vartlevel;//
        private UInt16 _watercond;//海水电导率
        private UInt16 _soundvec;//声速
        byte[] storebyte = new byte[16];
        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public ushort Watertemp
        {
            get { return _watertemp; }
            set { _watertemp = value; }
        }

        public ushort Vartlevel
        {
            get { return _vartlevel; }
            set { _vartlevel = value; }
        }

        public ushort Watercond
        {
            get { return _watercond; }
            set { _watercond = value; }
        }

        public ushort Soundvec
        {
            get { return _soundvec; }
            set { _soundvec = value; }
        }

        public void Parse(byte[] bytes)
        {
            _ltime = BitConverter.ToInt64(bytes, 2);
            _watertemp = BitConverter.ToUInt16(bytes, 10);
            _vartlevel = BitConverter.ToUInt16(bytes, 12);
            _watercond = BitConverter.ToUInt16(bytes, 14);
            _soundvec = BitConverter.ToUInt16(bytes, 16);
            
            Buffer.BlockCopy(bytes, 2, storebyte, 0, 16);
        }

        internal byte[] Pack()
        {
            return storebyte;
        }
    };

    //潜水器位姿信息0x1001
    public class Subposition 
    {
        private long _ltime;
        private float   _subLong; //潜水器经度
        private float   _subLat;//潜水器纬度
        private UInt16 _subheading;//潜水器艏向角
        private Int16 _subpitch;//潜水器纵倾角
        private Int16 _subroll;//潜水器横倾角
        private UInt16 _subdepth;//潜水器深度
        private UInt16 _subheight;//潜水器高度
        private byte[] storebyte = new byte[26];

        //
        public Subposition(long time = 0, float Long = 0, float Lat = 0, UInt16 heading = 0, short pitch = 0, short roll = 0, UInt16 depth = 0, UInt16 height = 0)
        {
            _ltime = time;
            _subLong = Long;
            _subLat = Lat;
            _subheading = heading;
            _subpitch = pitch;
            _subroll = roll;
            _subdepth = depth;
            _subheight = height;
        }
        public string Ltime
        {
            get { return DateTime.FromFileTime(_ltime).ToString("YYYY-MM-DD HH:mm:ss"); }
        }

        public string SubLong
        {
            get { return (_subLong/60).ToString()+"°"+(_subLong%60).ToString()+"'"; }
        }

        public string SubLat
        {
            get { return (_subLat / 60).ToString() + "°" + (_subLat % 60).ToString() + "'"; }

        }
        
        public float Subheading
        {
            get { return (float)_subheading*360/65536; }

        }

        public float Subpitch
        {
            get { return (float)_subpitch*180/32768; }

        }

        public float Subroll
        {
            get { return (float)_subroll*180/32768; }

        }

        public float Subdepth
        {
            get { return (float)_subdepth*5000/65536; }

        }

        public float Subheight
        {
            get { return (float)_subheight*256/65535; }
        }
    
        //前两个字节ID，parse完后数据存在成员里，属性表示真正的含义
        public void Parse(byte[] bytes)
        {
            _ltime = BitConverter.ToInt64(bytes, 2);
            _subLong = BitConverter.ToInt32(bytes, 10);
            _subLat = BitConverter.ToInt32(bytes, 14);
            _subheading = BitConverter.ToUInt16(bytes, 18);
            _subpitch = BitConverter.ToInt16(bytes, 20);
            _subroll = BitConverter.ToInt16(bytes, 22);
            _subdepth = BitConverter.ToUInt16(bytes, 24);
            _subheight = BitConverter.ToUInt16(bytes, 26);
            Buffer.BlockCopy(bytes,0,storebyte,0,26);
        }

        internal byte[] Pack()
        {
            return storebyte;
        }
    };


    //声学设备异常信息
    public class Abnormity{
        private long _ltime;
        private byte _equipNo;//设备号
        private byte	_abnorm;// oxff 表示正常

        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public byte EquipNo
        {
            get { return _equipNo; }
            set { _equipNo = value; }
        }

        public byte Abnorm
        {
            get { return _abnorm; }
            set { _abnorm = value; }
        }
    };

    //生命支持系统0x1003
    public class Lifesupply {
        private long _ltime;
        private byte  _oxygen;//氧气浓度
        private byte  _co2;//二氧化碳浓度
        private byte  _pressure;//舱内压力
        private byte  _temperature;//舱内温度
        private UInt16  _humidity; //舱内湿度		
        byte[] storebyte = new byte[14];
        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public byte Oxygen
        {
            get { return _oxygen; }
            set { _oxygen = value; }
        }

        public byte Co2
        {
            get { return _co2; }
            set { _co2 = value; }
        }

        public byte Pressure
        {
            get { return _pressure; }
            set { _pressure = value; }
        }

        public byte Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }

        public ushort Humidity
        {
            get { return _humidity; }
            set { _humidity = value; }
        }

        internal void Parse(byte[] bytes)
        {
            _ltime = BitConverter.ToInt64(bytes, 2);
            _oxygen = bytes[10];
            _co2 = bytes[11];
            _pressure = bytes[12];
            _temperature = bytes[13];
            _humidity = BitConverter.ToUInt16(bytes, 14);
            Buffer.BlockCopy(bytes, 2, storebyte, 0, 14);
        }

        internal byte[] Pack()
        {
            return storebyte;
        }
    };

    //能源系统信息
    public class Energysys {
        private long _ltime;
        private byte  _headmainV;//主电池电压
        private UInt16  _headmainI;//主电池电流
        private UInt16  _headmainconsume;//主电池能源消耗
        private byte _headmainMaxTemp;
        private byte _headmainMaxExpand;
        private byte _tailmainV;//尾主电池电压
        private UInt16 _tailmainI;//尾主电池电流
        private UInt16 _tailmainconsume;//尾主电池能源消耗
        private byte _tailmainMaxTemp;
        private byte _tailmainMaxExpand;
        private byte _leftsubV;//左副电池电压
        private byte _leftsubI;//左副电池电流
        private UInt16 _leftsubconsume;//左副电池能源消耗	
        private byte _leftsubMaxTemp;
        private byte _leftsubMaxExpand;
        private byte _rightsubV;//右副电池电压
        private byte _rightsubI;//右副电池电流
        private UInt16 _rightsubconsume;//右副电池能源消耗	
        private byte _rightsubMaxTemp;
        private byte _rightsubMaxExpand;
        byte[] storeBytes = new byte[34];
        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public byte HeadmainV
        {
            get { return _headmainV; }
        }

        public ushort HeadmainI
        {
            get { return _headmainI; }
        }

        public ushort Headmainconsume
        {
            get { return _headmainconsume; }
        }

        public byte HeadmainMaxTemp
        {
            get { return _headmainMaxTemp; }
        }

        public byte HeadmainMaxExpand
        {
            get { return _headmainMaxExpand; }
        }

        public byte TailmainV
        {
            get { return _tailmainV; }
        }

        public ushort TailmainI
        {
            get { return _tailmainI; }
        }

        public ushort Tailmainconsume
        {
            get { return _tailmainconsume; }
        }

        public byte TailmainMaxTemp
        {
            get { return _tailmainMaxTemp; }
        }

        public byte TailmainMaxExpand
        {
            get { return _tailmainMaxExpand; }
        }

        public byte LeftsubV
        {
            get { return _leftsubV; }
        }

        public byte LeftsubI
        {
            get { return _leftsubI; }
        }

        public ushort Leftsubconsume
        {
            get { return _leftsubconsume; }
        }

        public byte LeftsubMaxTemp
        {
            get { return _leftsubMaxTemp; }
        }

        public byte LeftsubMaxExpand
        {
            get { return _leftsubMaxExpand; }
        }

        public byte RightsubV
        {
            get { return _rightsubV; }
        }

        public byte RightsubI
        {
            get { return _rightsubI; }
        }

        public ushort Rightsubconsume
        {
            get { return _rightsubconsume; }
        }

        public byte RightsubMaxTemp
        {
            get { return _rightsubMaxTemp; }
        }

        public byte RightsubMaxExpand
        {
            get { return _rightsubMaxExpand; }
        }


        internal void Parse(byte[] bytes)
        {
            _ltime = BitConverter.ToInt64(bytes, 2);
            _headmainV = bytes[10];
            _headmainI = BitConverter.ToUInt16(bytes, 11);
           _headmainconsume = BitConverter.ToUInt16(bytes, 13);
            _headmainMaxTemp =bytes[15];
            _headmainMaxExpand=bytes[16];
            _tailmainV=bytes[17];
            _tailmainI = BitConverter.ToUInt16(bytes, 18);
            _tailmainconsume = BitConverter.ToUInt16(bytes, 20);
            _tailmainMaxTemp = bytes[22];
            _tailmainMaxExpand = bytes[23];
            _leftsubV = bytes[24];
            _leftsubI = bytes[25];
            _leftsubconsume = BitConverter.ToUInt16(bytes, 26);
            _leftsubMaxTemp = bytes[28];
            _leftsubMaxExpand = bytes[29];
            _rightsubV = bytes[30];
            _rightsubI = bytes[31];
            _rightsubconsume = BitConverter.ToUInt16(bytes, 32);	
            _rightsubMaxTemp = bytes[34];
            _rightsubMaxExpand = bytes[35];
            Buffer.BlockCopy(bytes, 0, storeBytes, 0, 26);
        }

        internal byte[] Pack()
        {
            return storeBytes;
        }
    };

    //报警信息1005
    public class Alertdata {
        private long _ltime;
        private UInt64  _alert;//报警0-64bit
        private byte	_leak;//载人舱漏水
        private UInt16	_cable;//压载水舱液位
        private byte    _temperature;//计算机罐温度
        byte[] storeBytes = new byte[20];
        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public UInt64 Alert
        {
            get { return _alert; }
            set { _alert = value; }
        }

        public byte Leak
        {
            get { return _leak; }
            set { _leak = value; }
        }

     
        public ushort Cable
        {
            get { return _cable; }
            set { _cable = value; }
        }

        public byte Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }

        internal void Parse(byte[] bytes)
        {
            _ltime = BitConverter.ToInt64(bytes, 2);
            _alert = BitConverter.ToUInt64(bytes, 10);
            _leak = bytes[18];
            _cable = BitConverter.ToUInt16(bytes, 19);
            _temperature = bytes[21];
            Buffer.BlockCopy(bytes, 2, storeBytes,0,20);
        }

        internal byte[] Pack()
        {
            return storeBytes;
        }
    };

    //开关信息
    public class Switchdata {
        private long _ltime;
        private byte  _equipNo;//设备号
        private byte  _state;//开关状态

        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public byte EquipNo
        {
            get { return _equipNo; }
            set { _equipNo = value; }
        }

        public byte State
        {
            get { return _state; }
            set { _state = value; }
        }
    };

    //母船定位信息
    public class Shippost {
    //	__int64 ltime;
        private long _ltime;
        private float _shipLong;//母船经度
        private float _shipLat; //母船纬度
        private float _shipheading;//母船艏向角

        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public float ShipLong
        {
            get { return _shipLong; }
            set { _shipLong = value; }
        }

        public float ShipLat
        {
            get { return _shipLat; }
            set { _shipLat = value; }
        }

        public float Shipheading
        {
            get { return _shipheading; }
            set { _shipheading = value; }
        }
    };

    public class SubLatestPost//近n次的潜器定位信息
    {
        private List<long> _ltime;
        private List<float> _subLong; //潜水器经度
        private List<float> _subLat;//潜水器纬度
        private int UpperLimit = 5;//存储上限

        public SubLatestPost(int num=5)
        {
            UpperLimit = num;
            _ltime = new List<long>(5);
            _ltime.ForEach((s) => s=0);
            _subLong.ForEach((s)=>s=0);
            _subLat.ForEach((s)=>s=0);
        }

        public void AddLongTime(long ltime)
        {
            if(Ltime.Count==UpperLimit)
                Ltime.RemoveAt(0);
            Ltime.Add(ltime);
        }

        public void AddSubLong(float sublong)
        {
            if(SubLong.Count==UpperLimit)
                SubLong.RemoveAt(0);
            SubLong.Add(sublong);
        }
        public void AddSubLat(float sublat)
        {
            if(SubLat.Count==UpperLimit)
                SubLat.RemoveAt(0);
            SubLat.Add(sublat);
        }
        public List<long> Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public List<float> SubLong
        {
            get { return _subLong; }
            set { _subLong = value; }
        }

        public List<float> SubLat
        {
            get { return _subLat; }
            set { _subLat = value; }
        }

        public bool Parse(byte[] bytes)
        {
            return false;
        }
    };

    public class Adcpdata
    {

        private sbyte[] _floorX;
        private sbyte[] _floorY;
        private sbyte[] _floorZ;
        byte[] storebyte = new byte[30];
        public Adcpdata()
        {
            _floorX = new sbyte[10];
            _floorY = new sbyte[10];
            _floorZ = new sbyte[10];
        }

        public void Clear()
        {
            Array.Clear(_floorX,0,10);
            Array.Clear(_floorY,0,10);
            Array.Clear(_floorZ,0,10);
        }
        public sbyte[] FloorX
        {
            get { return _floorX; }
            set { _floorX = value; }
        }
        public sbyte[] FloorY
        {
            get { return _floorY; }
            set { _floorY = value; }
        }
        public sbyte[] FloorZ
        {
            get { return _floorZ; }
            set { _floorZ = value; }
        }

        internal void Parse(byte[] bytes)
        {
            Buffer.BlockCopy(bytes, 2, _floorX, 0, 10);
            Buffer.BlockCopy(bytes, 12, _floorY, 0, 10);
            Buffer.BlockCopy(bytes, 22, _floorZ, 0, 10);
            Buffer.BlockCopy(bytes,2,storebyte,0,30);
        }

        internal byte[] Pack()
        {
            return storebyte;
        }
    }
    #endregion

    
}

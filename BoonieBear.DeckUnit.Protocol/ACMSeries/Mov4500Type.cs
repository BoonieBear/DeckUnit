using System;
using System.Collections.Generic;

namespace BoonieBear.DeckUnit.Protocol.ACMSeries
{
    #region 枚举类型
    enum ModulationType
    {
        MFSK = 0,
        MPSK = 2,
        OPSK = 3,
        FHSS = 4,
        VOICE = 6,
    }

    enum Mov4500Type
    {
        ALLPOST = 0,
        BP = 1,
        BSSS = 2,
        CTD = 3,
        SUBPOST = 4,
        ACUSTICALARM = 5,
        LIFESUPPLY = 6,
        ENERGY = 7,
        SUBALERT = 8,
        SWITCH = 9,
        SHIPPOST=10,
        SUBLASTEST5POST = 11,
    }
    #endregion

    public class GlobalVariables
    {
        public static int MFSKSize = 135;
        public static int ShipMFSKSize = 112;
        public static int MovMFSKSize = 131;
        public static int WordSize = 40;
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
    //定位信息
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
        public static UInt32 Length = 40;

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
    };

    //避碰信息
    public class Bpdata {
        private long _ltime;
        private UInt16		_frontup;//前上避碰声呐距离
        private UInt16		_front;//正前
        private UInt16		_frontdown;//前下
        private UInt16		_down;//正下
        private UInt16		_behinddown;//后下
        private UInt16		_left;//左下
        private UInt16		_right;//右下	

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
    };

    //侧深侧扫信息
    public class Bsssdata {
        private long _ltime;
        private Int16 _depth;//距离海底高度

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
    };


    //CTD信息
    public class Ctddata {
        private long _ltime;
        private UInt16 _watertemp;//海水温度
        private UInt16 _vartlevel;//
        private UInt16 _watercond;//海水电导率
        private UInt16 _soundvec;//声速

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
    };

    //潜水器位姿信息
    public class Subposition {
        private long _ltime;
        private float   _subLong; //潜水器经度
        private float   _subLat;//潜水器纬度
        private UInt16 _subheading;//潜水器艏向角
        private Int16 _subpitch;//潜水器纵倾角
        private Int16 _subroll;//潜水器横倾角
        private Int16 _subupdownvec;//潜水器升沉速度
        private Int16 _subpitchvec;//潜水器纵速度
        private Int16 _subrollvec;//潜水器横速度
        private UInt16 _subdepth;//潜水器深度
        private UInt16 _subheight;//潜水器高度

        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
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

        public ushort Subheading
        {
            get { return _subheading; }
            set { _subheading = value; }
        }

        public short Subpitch
        {
            get { return _subpitch; }
            set { _subpitch = value; }
        }

        public short Subroll
        {
            get { return _subroll; }
            set { _subroll = value; }
        }

        public short Subupdownvec
        {
            get { return _subupdownvec; }
            set { _subupdownvec = value; }
        }

        public short Subpitchvec
        {
            get { return _subpitchvec; }
            set { _subpitchvec = value; }
        }

        public short Subrollvec
        {
            get { return _subrollvec; }
            set { _subrollvec = value; }
        }

        public ushort Subdepth
        {
            get { return _subdepth; }
            set { _subdepth = value; }
        }

        public ushort Subheight
        {
            get { return _subheight; }
            set { _subheight = value; }
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

    //生命支持系统
    public class Lifesupply {
        private long _ltime;
        private byte  _oxygen;//氧气浓度
        private byte  _co2;//二氧化碳浓度
        private byte  _pressure;//舱内压力
        private byte  _temperature;//舱内温度
        private UInt16  _humidity; //舱内湿度		

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
    };

    //能源系统信息
    public class Energysys {
        private long _ltime;
        private UInt16  _mainV;//主电池电压
        private UInt16  _mainI;//主电池电流
        private UInt16  _mainconsume;//主电池能源消耗
        private UInt16  _subV;//副电池电压
        private UInt16  _subI;//副电池电流
        private UInt16  _subconsume;//副电池能源消耗	

        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public ushort MainV
        {
            get { return _mainV; }
            set { _mainV = value; }
        }

        public ushort MainI
        {
            get { return _mainI; }
            set { _mainI = value; }
        }

        public ushort Mainconsume
        {
            get { return _mainconsume; }
            set { _mainconsume = value; }
        }

        public ushort SubV
        {
            get { return _subV; }
            set { _subV = value; }
        }

        public ushort SubI
        {
            get { return _subI; }
            set { _subI = value; }
        }

        public ushort Subconsume
        {
            get { return _subconsume; }
            set { _subconsume = value; }
        }
    };

    //报警信息
    public class Alertdata {
        private long _ltime;
        private UInt32  _alert;//报警0-31bit
        private byte	_aleak;//载人舱漏水
        private UInt16	_pressure;//应急液压源压力
        private UInt16	_head;//艏纵倾液位检测
        private UInt16	_behind;//艉纵倾液位检测
        private UInt16	_cable;//压载水舱液位
        private byte    _temperature;//计算机罐温度

        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public uint Alert
        {
            get { return _alert; }
            set { _alert = value; }
        }

        public byte Aleak
        {
            get { return _aleak; }
            set { _aleak = value; }
        }

        public ushort Pressure
        {
            get { return _pressure; }
            set { _pressure = value; }
        }

        public ushort Head
        {
            get { return _head; }
            set { _head = value; }
        }

        public ushort Behind
        {
            get { return _behind; }
            set { _behind = value; }
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

    public class SubLatest5Post//近5次的潜器定位信息
    {
        private List<long> _ltime;
        private List<float> _subLong; //潜水器经度
        private List<float> _subLat;//潜水器纬度


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
    };
    #endregion

    #region 声学通信压缩数据结构
    //定位信息
     public class MFSKSysposition {
         private long _ltime;	//起始时间
         private byte[]   _interval;//4 interval time from ltime
	     float[]   subLONG; //潜水器经度 5
	     float[]   subLAT;//潜水器纬度 5
	     UInt16  subdepth;//潜水器深度
	     Int16    relateX;//潜水器相对母船x轴位移
	     Int16    relateY;//潜水器相对母船y轴位移
	     UInt16    relateZ;//潜水器相对母船z轴位移	
	     float shipLONG;//母船经度
	     float shipLAT; //母船纬度

         public MFSKSysposition()
         {
             _interval = new byte[4];
             Buffer.SetByte(_interval,0,0);
         }
         public long Ltime
         {
             get { return _ltime; }
             set { _ltime = value; }
         }

         public byte[] Interval
         {
             get { return _interval; }
             set { _interval = value; }
         }
     };

    //流速信息 ps自定义
    public class Adcpdata{
	    SByte[]	floorX;//10
	    SByte[]	floorY;//10
	    SByte[]	floorZ;//10
    };

    //ADL
    class Adldata {
	    float adl1;//
	    float adl2;//
    };

    //避碰信息
    public class MFSKBpdata {
	    byte	frontup;//前上避碰声呐距离
	    byte	front;//正前
	    byte	frontdown;//前下
	    byte	down;//正下
	    byte	behinddown;//后下
	    byte	left;//左下
	    byte	right;//右下	
    };
    //侧深侧扫信息
    public class MFSKBsssdata {
	    byte depth;//距离海底高度
    };



    //CTD信息
    public class MFSKCtddata {
	    UInt16 watertemp;//海水温度
	    UInt16 watercond;//海水电导率
	    byte vartlevel;//可变压载水舱液位
	    byte soundvec;//声速
    };

    //潜水器位姿信息
    public class MFSKSubposition {
	    float   subLONG; //潜水器经度
	    float   subLAT;//潜水器纬度
	    UInt16 subheading;//潜水器艏向角
	    Int16 subpitch;//潜水器纵倾角
	    Int16 subroll;//潜水器横倾角
	    UInt16 subdepth;//潜水器深度
	    char subupdownvec;//潜水器升沉速度
	    char subpitchvec;//潜水器纵速度
	    char subrollvec;//潜水器横速度
	    byte subheight;//潜水器高度
    };


    //生命支持系统
    public class MFSKLifesupply {
	    byte  oxygen;//氧气浓度
	    byte  CO2;//二氧化碳浓度
	    byte  pressure;//舱内压力
	    byte  temperature;//舱内温度
	    byte  humidity; //舱内湿度		
    };

    public class MFSKEnergysys {
	    byte  mainV;//主电池电压
	    byte  mainI;//主电池电流
	    byte  mainconsume;//主电池能源消耗
	    byte  subV;//副电池电压
	    byte  subI;//副电池电流
	    byte  subconsume;//副电池能源消耗	
    };//能源系统信息

    public class MFSKAlertdata {
	    byte[]	alert;//报警 4
	    byte	aleak;//载人舱漏水
	    byte	pressure;//应急液压源压力
	    byte	head;//艏纵倾液位检测
	    byte	behind;//艉纵倾液位检测
	    byte	cable;//压载水舱液位
	    byte    temperature;//计算机罐温度
    };//报警信息

    public class MFSKSwitchdata {
	    byte[]  state;//开关状态 2
    };//开关信息
    #endregion
}

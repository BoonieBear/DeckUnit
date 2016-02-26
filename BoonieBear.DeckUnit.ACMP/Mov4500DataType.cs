using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            PSKEND = 0xed02,//不会接收到
            SSBEND = 0xed04,//不会接收到
            Req = 0xdada,
            SSBNULL = 0xdaa4//发射时上传的空包，用于填充语音的空白处
        }

    public enum MonitorMode
    {
        SHIP = 0,
        SUBMARINE = 1,
    }
    //潜器中交换数据，只用于udp
    public enum ExchageType
    {
        ALLPOST = 0x2001,
        BP = 0x2002,
        //BSSS = 0x2005,//2003,2004预留
        ADCP = 0x2006,
        SUBPOST = 0x1001,
        CTD = 0x1002,
        LIFESUPPLY = 0x1003,
        ENERGY = 0x1004,
        ALERT = 0x1005,
    }
    //数据类型，程序中用到的所有数据类型定义
    public enum MovDataType
    {

        ALLPOST = 0x2001,//母船下发的定位数据
        BP=0x2002,//避碰声纳数据
        SUBPOST = 0x1001,//潜器上传的定位数据
        CTD = 0x1002,
        LIFESUPPLY = 0x1003,
        ENERGY = 0x1004,
        ALERT = 0x1005,
        //BSSS = 0x2005,//fsk
        ADCP = 0x2006,//fsk

        WORD = 13,//ui
        IMAGE = 14,//ui
        SOUND = 15,//ui
    }
    #endregion

    public class MovGlobalVariables
    {
        public static int MFSKSize = 210;
        public static int ShipMFSKSize = 80;
        public static int MovMFSKSize = 209;
        public static int WordSize = 40;
        public static int ImgSize = 16340;
        public static int MPSKSize = 16560;
    }

    public static class Alarm
    {
        public static Dictionary<int, string> Name = new Dictionary<int, string>(64);

        static Alarm()
        {
            Name.Add(0,"艏主蓄电池箱漏水");
            Name.Add(1,"艉主蓄电池箱漏水");
            Name.Add(2,"左副蓄电池箱漏水");
            Name.Add(3,"右副蓄电池箱漏水");
            Name.Add(4,"作业接线箱漏水");
            Name.Add(5,"动力接线箱漏水");
            Name.Add(6,"传感器接线箱漏水");
            Name.Add(7,"观察接线箱漏水");
            Name.Add(8,"声学主接线箱漏水");
            Name.Add(9,"声学副接线箱漏水");
            Name.Add(10,"纵倾调节油箱漏水");
            Name.Add(11,"配电罐 1 漏水");
            Name.Add(12,"配电罐 2 漏水");
            Name.Add(13,"计算机罐漏水");
            Name.Add(14,"SDI 光纤罐漏水");
            Name.Add(15,"水声通信机 1 漏水");
            Name.Add(16,"水声通信机 2 漏水");
            Name.Add(17,"测深测扫罐漏水");
            Name.Add(18,"水声电话罐漏水");
            Name.Add(19,"定位声呐罐漏水");
            Name.Add(20,"多普勒罐漏水");
            Name.Add(21,"避碰声纳罐漏水");
            Name.Add(22,"海水泵罐漏水");
            Name.Add(23,"左主推进器罐漏水");
            Name.Add(24,"右主推进器罐漏水");
            Name.Add(25,"左垂推进器罐漏水");
            Name.Add(26,"右垂推进器罐漏水");
            Name.Add(27,"艏侧推进器罐漏水");
            Name.Add(28,"艉垂推进器罐漏水");
            Name.Add(29,"艏主蓄电池箱油位补偿报警");
            Name.Add(30,"艉主蓄电池箱油位补偿报警");
            Name.Add(31,"左副蓄电池箱油位补偿报警");
            Name.Add(32,"右副蓄电池箱油位补偿报警");
            Name.Add(33,"作业接线箱油位补偿报警");
            Name.Add(34,"动力接线箱油位补偿报警");
            Name.Add(35,"传感器接线箱油位补偿报警");
            Name.Add(36,"观察接线箱油位补偿报警");
            Name.Add(37,"声学主接线箱油位补偿报警");
            Name.Add(38,"声学副接线箱油位补偿报警");
            Name.Add(39,"纵倾调节油箱油位补偿报警");
            Name.Add(40,"主液压源漏水");
            Name.Add(41,"主液压源油位报警");
            Name.Add(42,"主液压源温度报警");
            Name.Add(43,"主液压源驱动罐漏水");
            Name.Add(44,"主液压源阀箱漏水");
            Name.Add(45,"主液压源阀箱压力报警");
            Name.Add(46,"主液压源阀箱油位报警");
            Name.Add(47,"副液压源漏水");
            Name.Add(48,"副液压源油位报警");
            Name.Add(49,"副液压源温度报警");
            Name.Add(50,"副液压源驱动罐漏水");
            Name.Add(51,"副液压源阀箱漏水");
            Name.Add(52,"副液压源阀箱压力报警");
            Name.Add(53,"副液压源阀箱油位报警");
            Name.Add(54,"左机械手油位补偿报警");
            Name.Add(55,"右机械手油位补偿报警");
            Name.Add(56,"应急液压源漏水");
            Name.Add(57,"应急液压源油位报警");
            Name.Add(58,"应急液压源驱动罐漏水");
            Name.Add(59,"应急液压源阀箱漏水");
            Name.Add(60,"应急液压源阀箱油位报警");
            Name.Add(61,"应急液压源压力报警");
            Name.Add(62,"备用报警 1");
            Name.Add(63,"备用报警 2");
            
        }
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

    /// <summary>
    /// 接口
    /// </summary>
    public  interface IProtocol
    {
        byte[] Pack();
        void Parse(byte[] bytes);
    }
    //定位信息0x2001
    public class Sysposition : IProtocol
    {
        public long _ltime;
        public float _shipLong;//母船经度
        public float _shipLat; //母船纬度
        public Int16 _shipvel;//母船速度
        public UInt16 _shipheading;//母船艏向角
        public Int16 _shippitch;//母船纵倾角
        public Int16 _shiproll;//母船横倾角
        public float _subLong; //潜水器经度
        public float _subLat;//潜水器纬度
        public UInt16 _subdepth;//潜水器深度
        public Int16 _relateX;//潜水器相对母船x轴位移
        public Int16 _relateY;//潜水器相对母船y轴位移
        public UInt16 _relateZ;//潜水器相对母船z轴位移	
        private byte[] storebyte = new byte[40];

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
        public void Parse(byte[] bytes)
        {
            _ltime = BitConverter.ToInt64(bytes, 0);
            _shipLong = BitConverter.ToSingle(bytes, 8);
            _shipLat = BitConverter.ToSingle(bytes, 12);
            _shipvel = BitConverter.ToInt16(bytes, 16);
            _shipheading = BitConverter.ToUInt16(bytes, 18);
            _shippitch = BitConverter.ToInt16(bytes, 20);
            _shiproll = BitConverter.ToInt16(bytes, 22);
            _subLong = BitConverter.ToSingle(bytes, 24);
            _subLat = BitConverter.ToSingle(bytes, 28);
            _subdepth = BitConverter.ToUInt16(bytes, 32);
            _relateX = BitConverter.ToInt16(bytes, 34);
            _relateY = BitConverter.ToInt16(bytes, 36);
            _relateZ = BitConverter.ToUInt16(bytes, 38);
            Buffer.BlockCopy(bytes, 0, storebyte,0,40);
        }
        public string Time
        {
            get { return DateTime.FromFileTime(_ltime).ToString(); }

        }
        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }
        public string ShipLong
        {
            get
            {
                if (_shipLong >= 0)
                    return (_shipLong / 60).ToString() + "°" + (_shipLong % 60).ToString() + "'" + " E";
                else
                {
                    return (-_shipLong / 60).ToString() + "°" + (-_shipLong % 60).ToString() + "'" + " W";
                }
            }
        }
        public string ShipLat
        {
            get
            {
                if (_shipLong >= 0)
                    return (_shipLat / 60).ToString() + "°" + (_shipLat % 60).ToString() + "'"+" N";
                else
                {
                    return (-_shipLat / 60).ToString() + "°" + (-_shipLat % 60).ToString() + "'"+" S";
                }
            }
        }
        public float Shipvel
        {
            get { return (float)_shipvel*10000/32768; }
        }
        public float Shipheading
        {
            get { return (float)_shipheading*360/65536; }
        }
        public float Shippitch
        {
            get { return (float)_shippitch*180/32768; }
            
        }
        public float Shiproll
        {
            get { return (float)_shiproll*180/32768; }
        }
        public string SubLong
        {
            get
            {
                if (_subLong >= 0)
                    return (_subLong / 60).ToString() + "°" + (_subLong % 60).ToString() + "'" + " N";
                else
                {
                    return (-_subLong / 60).ToString() + "°" + (-_subLong % 60).ToString() + "'" + " S";
                }
            }
        }
        public string SubLat
        {
            get
            {
                if (_subLat >= 0)
                    return (_subLat / 60).ToString() + "°" + (_subLat % 60).ToString() + "'" + " N";
                else
                {
                    return (-_subLat / 60).ToString() + "°" + (-_subLat % 60).ToString() + "'" + " S";
                }
            }
        }
        public float Subdepth
        {
            get { return (float)_subdepth*5000/65536; }
            
        }
        public float RelateX
        {
            get { return (float)_relateX * 15000 / 32768;  }
            
        }
        public float RelateY
        {
            get { return (float)_relateY * 15000 / 32768; }
        }
        public float RelateZ
        {
            get { return (float)_relateZ * 5000 / 65536; }
        }
        
    };

    //避碰信息2002,
    public class Bpdata:IProtocol
    {
        private UInt32      _itime;//从2015年1/1/0:0:0开始的秒数
        private UInt16		_frontup;//前上避碰声呐距离
        private UInt16		_front;//正前
        private UInt16		_frontdown;//前下
        private UInt16		_down;//正下
        private UInt16		_behinddown;//后下
        private UInt16		_left;//左下
        private UInt16		_right;//右下	
        private byte[] storebyte = new byte[18];
        public string Time
        {
            get { return DateTime.FromFileTime(_itime).ToString(); }

        }
        public long Itime
        {
            get
            {
                DateTime starTime = new DateTime(2015, 1, 1, 0, 0, 0);
                DateTime newtTime = starTime.AddSeconds(_itime);
                return newtTime.ToFileTime();
            }
        }

        public float Frontup
        {
            get { return (float)_frontup*256/65536; }
            
        }

        public float Front
        {
            get { return (float)_front * 256 / 65536; }
        }

        public float Frontdown
        {
            get { return (float)_frontdown * 256 / 65536; }
        }

        public float Down
        {
            get { return (float)_down * 256 / 65536; }
        }

        public float Behinddown
        {
            get { return (float)_behinddown * 256 / 65536; }
        }

        public float Left
        {
            get { return (float)_left * 256 / 65536; }
        }

        public float Right
        {
            get { return (float)_right * 256 / 65536; }
        }
        /// <summary>
        /// 用来打包数据给水声通信
        /// </summary>
        /// <returns></returns>
        public byte[] Pack()
        {
            return storebyte;
        }

        /// <summary>
        /// 水下显控解析UDP中的bp信息，源数据是22字节的，时间是64位的，转换成32位，总数据打包成18字节
        /// </summary>
        /// <param name="bytes"></param>
        public void Parse(byte[] bytes)
        {
            long ftime = BitConverter.ToInt64(bytes, 0);
            DateTime starTime = new DateTime(2015, 1, 1, 0, 0, 0);
            _itime = (uint) DateTime.FromFileTime(ftime).Subtract(starTime).TotalSeconds;
            
            _frontup = BitConverter.ToUInt16(bytes, 8);
            _front = BitConverter.ToUInt16(bytes, 10);
            _frontdown = BitConverter.ToUInt16(bytes, 12);
            _down = BitConverter.ToUInt16(bytes, 14);
            _behinddown = BitConverter.ToUInt16(bytes, 16);
            _left = BitConverter.ToUInt16(bytes, 18);
            _right = BitConverter.ToUInt16(bytes, 20);
            Buffer.BlockCopy(bytes, 8, storebyte,4,14);
            Buffer.BlockCopy(BitConverter.GetBytes(_itime), 0, storebyte, 0, 4);
        }
        /// <summary>
        /// 水面解析水下数据中的bp信息，源数据应该是18字节的，将32位时间解出，转换成64位，不打包数据
        /// </summary>
        /// <param name="bytes"></param>
        public void ParseFSK(byte[] bytes)
        {
            _itime = BitConverter.ToUInt32(bytes, 0);
            _frontup = BitConverter.ToUInt16(bytes, 4);
            _front = BitConverter.ToUInt16(bytes, 6);
            _frontdown = BitConverter.ToUInt16(bytes, 8);
            _down = BitConverter.ToUInt16(bytes, 10);
            _behinddown = BitConverter.ToUInt16(bytes, 12);
            _left = BitConverter.ToUInt16(bytes, 14);
            _right = BitConverter.ToUInt16(bytes, 16);
        }
    };

    //侧深侧扫信息和ADCP的时间取定位数据的时间,不用
    /*
    public class Bsssdata :IProtocol
    {
        private UInt32 _itime;//从2015年1/1/0:0:0开始的秒数
        private UInt16 _height;//距离海底高度
        byte[] storebyte = new byte[6];
        public long Itime
        {
            get
            {
                DateTime starTime = new DateTime(2015, 1, 1, 0, 0, 0);
                DateTime newtTime = starTime.AddSeconds(_itime);
                return newtTime.ToFileTime();
            }
        }
        public ushort Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public byte[] Pack()
        {
            return storebyte;
        }

        public void Parse(byte[] bytes)
        {
            _itime = BitConverter.ToUInt32(bytes, 0);
            _height = BitConverter.ToUInt16(bytes, 4);
            Buffer.BlockCopy(bytes, 0, storebyte, 0,6);
        }
    };
    */

    //CTD信息1002
    public class Ctddata :IProtocol
    {
        private long _ltime;
        private UInt16 _watertemp;//海水温度
        private UInt16 _depth;//
        private UInt16 _watercond;//海水电导率
        private UInt16 _soundvec;//声速
        byte[] storebyte = new byte[16];
        public string Time
        {
            get { return DateTime.FromFileTime(_ltime).ToString(); }

        }
        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public float Watertemp
        {
            get { return (float)_watertemp*50/65536; }
            
        }

        public float Depth
        {
            get { return (float)_depth*5000/65536; }
        }

        public float Watercond
        {
            get { return (float)_watercond*5000/65536; }
            
        }

        public float Soundvec
        {
            get { return (float)_soundvec*200/65536+1400; }
            
        }

        public void Parse(byte[] bytes)
        {
            _ltime = BitConverter.ToInt64(bytes, 0);
            _watertemp = BitConverter.ToUInt16(bytes, 8);
            _depth = BitConverter.ToUInt16(bytes, 10);
            _watercond = BitConverter.ToUInt16(bytes, 12);
            _soundvec = BitConverter.ToUInt16(bytes, 14);
            
            Buffer.BlockCopy(bytes, 0, storebyte, 0, 16);
        }

        public byte[] Pack()
        {
            return storebyte;
        }
    };

    //潜水器位姿信息0x1001
    public class Subposition :IProtocol
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
        public string Time
        {
            get { return DateTime.FromFileTime(_ltime).ToString(); }

        }
        public long Ltime
        {
            get { return _ltime; }
        }

        public string SubLong
        {
            get
            {
                if (_subLong >= 0)
                    return (_subLong / 60).ToString() + "°" + (_subLong % 60).ToString() + "'" + " E";
                else
                {
                    return (-_subLong / 60).ToString() + "°" + (-_subLong % 60).ToString() + "'" + " W";
                }
            }
        }

        public string SubLat
        {

            get
            {
                if (_subLat >= 0)
                    return (_subLat / 60).ToString() + "°" + (_subLat % 60).ToString() + "'" + " N";
                else
                {
                    return (-_subLat / 60).ToString() + "°" + (-_subLat % 60).ToString() + "'" + " S";
                }
            }

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
            _ltime = BitConverter.ToInt64(bytes, 0);
            _subLong = BitConverter.ToInt32(bytes, 8);
            _subLat = BitConverter.ToInt32(bytes, 12);
            _subheading = BitConverter.ToUInt16(bytes, 16);
            _subpitch = BitConverter.ToInt16(bytes, 18);
            _subroll = BitConverter.ToInt16(bytes, 20);
            _subdepth = BitConverter.ToUInt16(bytes, 22);
            _subheight = BitConverter.ToUInt16(bytes, 24);
            Buffer.BlockCopy(bytes,0,storebyte,0,26);
        }

        public byte[] Pack()
        {
            return storebyte;
        }
    };


    //声学设备异常信息， 现在不用
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
    public class Lifesupply:IProtocol
    {
        private long _ltime;
        private byte  _oxygen;//氧气浓度
        private byte  _co2;//二氧化碳浓度
        private byte  _pressure;//舱内压力
        private byte  _temperature;//舱内温度
        private UInt16  _humidity; //舱内湿度		
        byte[] storebyte = new byte[14];
        public string Time
        {
            get { return DateTime.FromFileTime(_ltime).ToString(); }

        }
        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public float Oxygen
        {
            get { return (float)_oxygen*50/256; }
            
        }

        public float Co2
        {
            get { return (float)_co2*10/256; }
            
        }

        public float Pressure
        {
            get { return _pressure; }
        }

        public float Temperature
        {
            get { return (float)_temperature * 50 / 256; }
        }

        public float Humidity
        {
            get { return (float)_humidity*100/65536; }
        }

        public void Parse(byte[] bytes)
        {
            _ltime = BitConverter.ToInt64(bytes, 0);
            _oxygen = bytes[8];
            _co2 = bytes[9];
            _pressure = bytes[10];
            _temperature = bytes[11];
            _humidity = BitConverter.ToUInt16(bytes, 12);
            Buffer.BlockCopy(bytes, 0, storebyte, 0, 14);
        }

        public byte[] Pack()
        {
            return storebyte;
        }
    };

    //能源系统信息
    public class Energysys:IProtocol
    {
        private long _ltime;
        private byte _headmainV;//主电池电压
        private UInt16 _headmainI;//主电池电流
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

        public string Time
        {
            get { return DateTime.FromFileTime(_ltime).ToString(); }
            
        }
        public long Ltime
        {
            get { return _ltime; }
            set { _ltime = value; }
        }

        public float HeadmainV
        {
            get { return (float) _headmainV*130/256; }
        }

        public float HeadmainI
        {
            get { return (float)_headmainI*500/65536; }
        }

        public float Headmainconsume
        {
            get { return (float)_headmainconsume*200/65536; }
        }

        public float HeadmainMaxTemp
        {
            get { return (float)_headmainMaxTemp*80/256; }
        }

        public float HeadmainMaxExpand
        {
            get { return (float)_headmainMaxExpand*100/256; }
        }

        public float TailmainV
        {
            get { return (float)_tailmainV*130/256; }
        }

        public float TailmainI
        {
            get { return (float)_tailmainI*500/65536; }
        }

        public float Tailmainconsume
        {
            get { return (float)_tailmainconsume*200/65536; }
        }

        public float TailmainMaxTemp
        {
            get { return (float)_tailmainMaxTemp*80/256; }
        }

        public float TailmainMaxExpand
        {
            get { return (float)_tailmainMaxExpand*100/256; }
        }

        public float LeftsubV
        {
            get { return (float)_leftsubV*30/256; }
        }

        public float LeftsubI
        {
            get { return (float)_leftsubI*200/256; }
        }

        public float Leftsubconsume
        {
            get { return (float)_leftsubconsume*50/65536; }
        }

        public float LeftsubMaxTemp
        {
            get { return (float)_leftsubMaxTemp*80/256; }
        }

        public float LeftsubMaxExpand
        {
            get { return (float)_leftsubMaxExpand * 100 / 256; }
        }

        public float RightsubV
        {
            get { return (float)_rightsubV*30/256; }
        }

        public float RightsubI
        {
            get { return (float)_rightsubI*200/256; }
        }

        public float Rightsubconsume
        {
            get { return (float)_rightsubconsume*50/65536; }
        }

        public float RightsubMaxTemp
        {
            get { return (float)_rightsubMaxTemp*80/256; }
        }

        public float RightsubMaxExpand
        {
            get { return (float)_rightsubMaxExpand*100/256; }
        }


        public void Parse(byte[] bytes)
        {
            _ltime = BitConverter.ToInt64(bytes, 0);
            _headmainV = bytes[8];
            _headmainI = BitConverter.ToUInt16(bytes, 9);
           _headmainconsume = BitConverter.ToUInt16(bytes, 11);
            _headmainMaxTemp =bytes[13];
            _headmainMaxExpand=bytes[14];
            _tailmainV=bytes[15];
            _tailmainI = BitConverter.ToUInt16(bytes, 16);
            _tailmainconsume = BitConverter.ToUInt16(bytes, 18);
            _tailmainMaxTemp = bytes[20];
            _tailmainMaxExpand = bytes[21];
            _leftsubV = bytes[22];
            _leftsubI = bytes[23];
            _leftsubconsume = BitConverter.ToUInt16(bytes, 24);
            _leftsubMaxTemp = bytes[26];
            _leftsubMaxExpand = bytes[27];
            _rightsubV = bytes[28];
            _rightsubI = bytes[29];
            _rightsubconsume = BitConverter.ToUInt16(bytes, 30);	
            _rightsubMaxTemp = bytes[32];
            _rightsubMaxExpand = bytes[33];
            Buffer.BlockCopy(bytes, 0, storeBytes, 0, 34);
        }

        public byte[] Pack()
        {
            return storeBytes;
        }
    };

    //报警信息1005
    public class Alertdata:IProtocol
    {
        private long _ltime;
        private UInt64  _alert;//报警0-63bit
        private byte	_leak;//载人舱漏水
        private UInt16	_cable;//压载水舱液位
        private byte    _temperature;//计算机罐温度
        byte[] storeBytes = new byte[20];
        public string Time
        {
            get { return DateTime.FromFileTime(_ltime).ToString(); }

        }
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

        public float Leak
        {
            get { return (float)_leak*10/256; }
            
        }


        public float Cable
        {
            get { return (float)_cable*3000/65536; }
            
        }

        public float Temperature
        {
            get { return (float)_temperature*80/256; }
            
        }

        public void Parse(byte[] bytes)
        {
            _ltime = BitConverter.ToInt64(bytes, 0);
            _alert = BitConverter.ToUInt64(bytes, 8);
            _leak = bytes[16];
            _cable = BitConverter.ToUInt16(bytes, 17);
            _temperature = bytes[19];
            Buffer.BlockCopy(bytes, 0, storeBytes,0,20);
        }

        public byte[] Pack()
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

    public class Adcpdata:IProtocol
    {
        private long _itime;
        private sbyte[] _floorX;
        private sbyte[] _floorY;
        private sbyte[] _floorZ;
        private byte _bottomTrack;//cm
        private UInt16 _height;//cm
        byte[] storebyte = new byte[41];
        public Adcpdata()
        {
            _floorX = new sbyte[10];
            _floorY = new sbyte[10];
            _floorZ = new sbyte[10];
            _bottomTrack = 0;
            _height = 0;
        }

        public void Clear()
        {
            Array.Clear(_floorX,0,10);
            Array.Clear(_floorY,0,10);
            Array.Clear(_floorZ,0,10);
            _bottomTrack = 0;
            _height = 0;
        }
        public string Time
        {
            get { return DateTime.FromFileTime(_itime).ToString(); }

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

        public long Itime
        {

                get { return _itime; }
                set { _itime = value; }
        }

        public float BottomTrack
        {
            get { return _bottomTrack/100f; }
        }

        public float Height
        {
            get { return _height/100f; }
        }

        public void Parse(byte[] bytes)
        {
            _itime = BitConverter.ToInt64(bytes, 0);
            Buffer.BlockCopy(bytes, 8, _floorX, 0, 10);
            Buffer.BlockCopy(bytes, 18, _floorY, 0, 10);
            Buffer.BlockCopy(bytes, 28, _floorZ, 0, 10);
            _bottomTrack = bytes[38];
            _height = BitConverter.ToUInt16(bytes, 39);
            Buffer.BlockCopy(bytes,0,storebyte,0,41);
        }

        public byte[] Pack()
        {
            return storebyte;
        }

    }
    #endregion

    
}

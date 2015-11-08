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
        public static int MFSKSize = 211;
        public static int ShipMFSKSize = 80;
        public static int MovMFSKSize = 208;
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
        internal void Parse(byte[] bytes)
        {
            _ltime = BitConverter.ToInt64(bytes, 2);
            _shipLong = BitConverter.ToSingle(bytes, 10);
            _shipLat = BitConverter.ToSingle(bytes, 14);
            _shipvel = BitConverter.ToInt16(bytes, 18);
            _shipheading = BitConverter.ToUInt16(bytes, 20);
            _shippitch = BitConverter.ToInt16(bytes, 22);
            _shiproll = BitConverter.ToInt16(bytes, 24);
            _subLong = BitConverter.ToSingle(bytes, 26);
            _subLat = BitConverter.ToSingle(bytes, 30);
            _subdepth = BitConverter.ToUInt16(bytes, 34);
            _relateX = BitConverter.ToInt16(bytes, 36);
            _relateY = BitConverter.ToInt16(bytes, 38);
            _relateZ = BitConverter.ToUInt16(bytes, 40);
            Buffer.BlockCopy(bytes, 2, storebyte,0,40);
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
                    return "E " + (_shipLong / 60).ToString() + "°" + (_shipLong % 60).ToString() + "'";
                else
                {
                    return "W " + (-_shipLong / 60).ToString() + "°" + (-_shipLong % 60).ToString() + "'";
                }
            }
        }
        public string ShipLat
        {
            get
            {
                if (_shipLong >= 0)
                    return "N " + (_shipLat / 60).ToString() + "°" + (_shipLat % 60).ToString() + "'";
                else
                {
                    return "S " + (-_shipLat / 60).ToString() + "°" + (-_shipLat % 60).ToString() + "'";
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
                    return "N " + (_subLong / 60).ToString() + "°" + (_subLong % 60).ToString() + "'";
                else
                {
                    return "S " + (-_subLong / 60).ToString() + "°" + (-_subLong % 60).ToString() + "'";
                }
            }
        }
        public string SubLat
        {
            get
            {
                if (_subLat >= 0)
                    return "N " + (_subLat / 60).ToString() + "°" + (_subLat % 60).ToString() + "'";
                else
                {
                    return "S " + (-_subLat / 60).ToString() + "°" + (-_subLat % 60).ToString() + "'";
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
    public class Bpdata {
        private UInt32      _itime;//从2015年1/1/0:0:0开始的秒数
        private UInt16		_frontup;//前上避碰声呐距离
        private UInt16		_front;//正前
        private UInt16		_frontdown;//前下
        private UInt16		_down;//正下
        private UInt16		_behinddown;//后下
        private UInt16		_left;//左下
        private UInt16		_right;//右下	
        private byte[] storebyte = new byte[18];

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

        internal void Parse(byte[] bytes)
        {
            _itime = BitConverter.ToUInt32(bytes, 2);
            _frontup = BitConverter.ToUInt16(bytes, 6);
            _front = BitConverter.ToUInt16(bytes, 8);
            _frontdown = BitConverter.ToUInt16(bytes, 10);
            _down = BitConverter.ToUInt16(bytes, 12);
            _behinddown = BitConverter.ToUInt16(bytes, 14);
            _left = BitConverter.ToUInt16(bytes, 16);
            _right = BitConverter.ToUInt16(bytes, 18);
            Buffer.BlockCopy(bytes, 2, storebyte,0,18);
        }
    };

    //侧深侧扫信息和ADCP的时间取定位数据的时间
    public class Bsssdata {
        private UInt32 _itime;//从2015年1/1/0:0:0开始的秒数
        private UInt16 _depth;//距离海底高度
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
        public ushort Depth
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
            _itime = BitConverter.ToUInt32(bytes, 2);
            _depth = BitConverter.ToUInt16(bytes, 6);
            Buffer.BlockCopy(bytes, 2, storebyte, 0,6);
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

        public float Watertemp
        {
            get { return (float)_watertemp*50/65536; }
            
        }

        public float Vartlevel
        {
            get { return (float)_vartlevel*5000/65536; }
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
        public long Ltime
        {
            get { return _ltime; }
        }

        public string SubLong
        {
            get
            {
                if (_subLong >= 0)
                    return "E " + (_subLong/60).ToString() + "°" + (_subLong%60).ToString() + "'";
                else
                {
                    return "W " + (-_subLong/60).ToString() + "°" + (-_subLong%60).ToString() + "'";
                }
            }
        }

        public string SubLat
        {

            get
            {
                if (_subLat >= 0)
                    return "N " + (_subLat / 60).ToString() + "°" + (_subLat % 60).ToString() + "'";
                else
                {
                    return "S " + (-_subLat / 60).ToString() + "°" + (-_subLat % 60).ToString() + "'";
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
        private UInt64  _alert;//报警0-63bit
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

    public class Adcpdata
    {
        private UInt32 _itime;//从2015年1/1/0:0:0开始的秒数
        private sbyte[] _floorX;
        private sbyte[] _floorY;
        private sbyte[] _floorZ;
        byte[] storebyte = new byte[34];
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

        public long Itime
        {
            get
            {
                DateTime starTime = new DateTime(2015,1,1,0,0,0);
                DateTime newtTime = starTime.AddSeconds(_itime);
                return newtTime.ToFileTime();
            }
        }

        internal void Parse(byte[] bytes)
        {
            _itime = BitConverter.ToUInt32(bytes, 2);
            Buffer.BlockCopy(bytes, 6, _floorX, 0, 10);
            Buffer.BlockCopy(bytes, 16, _floorY, 0, 10);
            Buffer.BlockCopy(bytes, 26, _floorZ, 0, 10);
            Buffer.BlockCopy(bytes,2,storebyte,0,34);
        }

        internal byte[] Pack()
        {
            return storebyte;
        }
    }
    #endregion

    
}

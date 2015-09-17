using System;
using System.Collections.Generic;
using System.Text;

namespace BoonieBear.DeckUnit.ACMP
{
    /// <summary>
    /// 潜器
    /// </summary>
    public class UWVGatherData
    {
        private readonly static object SyncObject = new object();
        private static UWVGatherData _uwvGatherData;
        private byte[] _mfskBytes = new byte[MovGlobalVariables.ShipMFSKSize];
        private byte[] _mpskBytes = new byte[MovGlobalVariables.MPSKSize];
        private Bpdata _bpdata;
        private Bsssdata _bsssdata;
        private Subposition _subposition;
        private Ctddata _ctddata;
        private Lifesupply _lifesupply;
        private Energysys _energysys;
        private Alertdata _alertdata;
        private Switchdata _switchdata;
        private Adcpdata _adcpdata;
        private string _msg;
        private byte[] img = new byte[MovGlobalVariables.ImgSize];
        private UWVGatherData()
        {
            _bpdata = new Bpdata();
            _bsssdata = new Bsssdata();
            _ctddata = new Ctddata();
            _lifesupply = new Lifesupply();
            _energysys = new Energysys();
            _alertdata = new Alertdata();
            _subposition = new Subposition();
            _switchdata = new Switchdata();
            _adcpdata = new Adcpdata();
        }
        /// <summary>
        /// 将UDP接收到数据和文字，图像数据加入各个数据结构里
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="mDataType"></param>
        public void Add(byte[] bytes, Mov4500DataType mDataType)
        {
            switch (mDataType)
            {
                case Mov4500DataType.SUBPOST:
                    _subposition.Parse(bytes);
                    break;
                case Mov4500DataType.BP:
                    _bpdata.Parse(bytes);
                    break;
                case Mov4500DataType.BSSS:
                    _bsssdata.Parse(bytes);
                    break;
                case Mov4500DataType.ADCP:
                    _adcpdata.Parse(bytes);
                    break;
                case Mov4500DataType.CTD:
                    _ctddata.Parse(bytes);
                    break;
                case Mov4500DataType.LIFESUPPLY:
                    _lifesupply.Parse(bytes);
                    break;
                case Mov4500DataType.ENERGY:
                    _energysys.Parse(bytes);
                    break;
                case Mov4500DataType.ALERT:
                    _alertdata.Parse(bytes);
                    break;
                case Mov4500DataType.WORD://发一次要清空一次
                    var msg = Encoding.Default.GetString(bytes);
                    if (msg != null)
                    {
                        Msg = msg;
                    }
                    break;
                case Mov4500DataType.IMAGE://发一次要清空一次
                    Buffer.BlockCopy(bytes, 0, img, 0, bytes.Length);//bytes.Length要小于img尺寸
                    break;
                default:
                    throw new Exception("undefined data type!");
            }
        }
        internal byte[] Package(ModuleType mType)
        {
            Array.Clear(_mfskBytes,0,_mfskBytes.Length);
            Array.Clear(_mpskBytes,0,_mpskBytes.Length);
            switch (mType)
            {
                case ModuleType.MFSK:
                    return _mfskBytes;
                    break;
                case ModuleType.MPSK:
                    return _mpskBytes;
                    break;
                default:
                    throw new Exception("未定义的调制类型！");
            }
            return null;
        }
        public byte[] PackageMFSKBytes
        {
            get { return Package(ModuleType.MFSK); }
        }
        public byte[] PackageMPSKBytes
        {
            get { return Package(ModuleType.MPSK); }
        }

        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        public static UWVGatherData GetInstance()
        {
            lock (SyncObject)
            {
                return _uwvGatherData ?? (_uwvGatherData = new UWVGatherData());
            }
        }

        
    }
}

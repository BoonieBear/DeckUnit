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
        private Adcpdata _adcpdata;
        private string _msg;
        private byte[] img = new byte[MovGlobalVariables.ImgSize];
        private UWVGatherData()
        {
            Clean();
        }
        public void Clean()
        {
            _msg = string.Empty;
            _bpdata = new Bpdata();
            _bsssdata = new Bsssdata();
            _ctddata = new Ctddata();
            _lifesupply = new Lifesupply();
            _energysys = new Energysys();
            _alertdata = new Alertdata();
            _subposition = new Subposition();
            _adcpdata = new Adcpdata();
            Array.Clear(_mfskBytes, 0, MovGlobalVariables.ShipMFSKSize);
            Array.Clear(_mpskBytes, 0, MovGlobalVariables.MPSKSize);
            Array.Clear(img, 0, MovGlobalVariables.ImgSize);
        }
        /// <summary>
        /// 将UDP接收到数据和文字，图像数据加入各个数据结构里
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="mDataType"></param>
        public void Add(byte[] bytes, MovDataType mDataType)
        {
            switch (mDataType)
            {
                case MovDataType.SUBPOST:
                    _subposition.Parse(bytes);
                    break;
                case MovDataType.BP:
                    _bpdata.Parse(bytes);
                    break;
                case MovDataType.BSSS:
                    _bsssdata.Parse(bytes);
                    break;
                case MovDataType.ADCP:
                    _adcpdata.Parse(bytes);
                    break;
                case MovDataType.CTD:
                    _ctddata.Parse(bytes);
                    break;
                case MovDataType.LIFESUPPLY:
                    _lifesupply.Parse(bytes);
                    break;
                case MovDataType.ENERGY:
                    _energysys.Parse(bytes);
                    break;
                case MovDataType.ALERT:
                    _alertdata.Parse(bytes);
                    break;
                case MovDataType.WORD://发一次要清空一次
                    var msg = Encoding.Default.GetString(bytes);
                    if (msg != null)
                    {
                        Msg = msg;
                    }
                    break;
                case MovDataType.IMAGE://发一次要清空一次
                    Buffer.BlockCopy(bytes, 0, img, 0, bytes.Length);//bytes.Length要小于img尺寸
                    break;
                default:
                    throw new Exception("undefined data type!");
            }
        }
        private byte[] Package(ModuleType mType)
        {
            Array.Clear(_mfskBytes,0,_mfskBytes.Length);
            Array.Clear(_mpskBytes,0,_mpskBytes.Length);
            switch (mType)
            {
                case ModuleType.MFSK:
                    var bytes = _subposition.Pack();
                    Buffer.BlockCopy(bytes,0,_mfskBytes,0,26);
                    bytes = _bpdata.Pack();
                    Buffer.BlockCopy(bytes,0,_mfskBytes,26,18);
                    bytes = _bsssdata.Pack();
                    Buffer.BlockCopy(bytes,0,_mfskBytes,44,6);
                    bytes = _adcpdata.Pack();
                    Buffer.BlockCopy(bytes,0,_mfskBytes,50,34);
                    bytes = _ctddata.Pack();
                    Buffer.BlockCopy(bytes,0,_mfskBytes,84,16);
                    bytes = _lifesupply.Pack();
                    Buffer.BlockCopy(bytes,0,_mfskBytes,100,14);
                    bytes = _energysys.Pack();
                    Buffer.BlockCopy(bytes,0,_mfskBytes,114,34);
                    bytes = _alertdata.Pack();
                    Buffer.BlockCopy(bytes,0,_mfskBytes,148,20);
                    if (_msg != null)
                    {
                        bytes = Encoding.Default.GetBytes(_msg);
                    }
                    Buffer.BlockCopy(bytes,0,_mfskBytes,168,40);
                    _msg = null;
                    return _mfskBytes;
                case ModuleType.MPSK:
                    bytes = _subposition.Pack();
                    Buffer.BlockCopy(bytes,0,_mpskBytes,0,26);
                    bytes = _bpdata.Pack();
                    Buffer.BlockCopy(bytes,0,_mpskBytes,26,18);
                    bytes = _bsssdata.Pack();
                    Buffer.BlockCopy(bytes,0,_mpskBytes,44,6);
                    bytes = _adcpdata.Pack();
                    Buffer.BlockCopy(bytes,0,_mpskBytes,50,34);
                    bytes = _ctddata.Pack();
                    Buffer.BlockCopy(bytes,0,_mpskBytes,84,16);
                    bytes = _lifesupply.Pack();
                    Buffer.BlockCopy(bytes,0,_mpskBytes,100,14);
                    bytes = _energysys.Pack();
                    Buffer.BlockCopy(bytes,0,_mpskBytes,114,34);
                    bytes = _alertdata.Pack();
                    Buffer.BlockCopy(bytes,0,_mpskBytes,148,20);
                    if (_msg != null)
                    {
                        bytes = Encoding.Default.GetBytes(_msg);
                    }
                    _msg = null;
                    Buffer.BlockCopy(bytes,0,_mpskBytes,168,40);
                    Buffer.BlockCopy(img, 0, _mpskBytes, MovGlobalVariables.MFSKSize, MovGlobalVariables.ImgSize);
                    Array.Clear(img, 0, MovGlobalVariables.ImgSize);
                    return _mpskBytes;
                default:
                    throw new Exception("未定义的调制类型！");
            }

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

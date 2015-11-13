using System;
using System.Text;

namespace BoonieBear.DeckUnit.ACMP
{
    /// <summary>
    /// 用来管理母船各种数据的收集，打包，解析
    /// </summary>
    public class ShipGatherData
    {
        private static readonly object SyncObject = new object();
        private static ShipGatherData _shipGatherData;
        private Sysposition _sysposition; //最后一个位置信息,用来显示
        private string _msg;
        private byte[] _packageBytes = new byte[MovGlobalVariables.ShipMFSKSize];

        private ShipGatherData()
        {
            Clean();
        }

        public void Clean()
        {
            _msg = string.Empty;
            _sysposition = new Sysposition();
            Array.Clear(_packageBytes, 0, MovGlobalVariables.ShipMFSKSize);
        }
        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        public Sysposition Sysposition
        {
            get { return _sysposition; }
            set { _sysposition = value; }
        }

        public static ShipGatherData GetInstance()
        {
            lock (SyncObject)
            {
                return _shipGatherData ?? (_shipGatherData = new ShipGatherData());
            }
        }

        public void Add(byte[] bytes, MovDataType mDataType)
        {
            switch (mDataType)
            {
                case MovDataType.ALLPOST:
                    _sysposition.Parse(bytes);
                    break;
                case MovDataType.WORD:
                    var msg = Encoding.Default.GetString(bytes);
                    if (msg != null)
                    {
                        Msg = msg;
                    }
                    break;
                default:
                    throw new Exception("undefined data type!");
            }
        }

        private byte[] Package(ModuleType mType)
        {
            Array.Clear(_packageBytes, 0, _packageBytes.Length);
            switch (mType)
            {
                case ModuleType.MFSK:
                    var bytes = _sysposition.Pack();
                    Buffer.BlockCopy(bytes, 0, _packageBytes, 0, 40);
                    if (_msg != null)
                    {
                        bytes = Encoding.Default.GetBytes(_msg);
                    }
                    Buffer.BlockCopy(bytes, 0, _packageBytes, 40, 40);
                    _msg = null;
                    return _packageBytes;
                default:
                    return null;
            }
        }
        public byte[] PackageMFSKBytes
        {
            get { return Package(ModuleType.MFSK); }
        }
        
    }
}

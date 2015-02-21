using System;
using System.Collections.Generic;

namespace BoonieBear.DeckUnit.Protocol.ACMSeries
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
        private List<string> _msghistory =new List<string>();
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

        public void Add(object obj, Mov4500Type mType)
        {
            switch (mType)
            {
                case Mov4500Type.SUBPOST:
                    var subposition = obj as Subposition;
                    if (subposition != null)
                        _subposition = subposition;
                    break;
                case Mov4500Type.BP:
                    var bpdata = obj as Bpdata;
                    if (bpdata != null)
                        _bpdata = bpdata;
                    break;
                case Mov4500Type.BSSS:
                    var bsssdata = obj as Bsssdata;
                    if (bsssdata != null)
                        _bsssdata = bsssdata;
                    break;
                case Mov4500Type.ADCP:
                    var adcpdata = obj as Adcpdata;
                    if (adcpdata != null)
                        _adcpdata = adcpdata;
                    break;
                case Mov4500Type.CTD:
                    var ctddata = obj as Ctddata;
                    if (ctddata != null)
                        _ctddata = ctddata;
                    break;
                case Mov4500Type.LIFESUPPLY:
                    var lifesupply = obj as Lifesupply;
                    if (lifesupply != null)
                        _lifesupply = lifesupply;
                    break;
                case Mov4500Type.ENERGY:
                    var energysys = obj as Energysys;
                    if (energysys != null)
                        _energysys = energysys;
                    break;
                case Mov4500Type.SWITCH:
                    var switchdata = obj as Switchdata;
                    if (switchdata != null)
                        _switchdata = switchdata;
                    break;
                case Mov4500Type.ALERT:
                    var alertdata = obj as Alertdata;
                    if (alertdata != null)
                        _alertdata = alertdata;
                    break;
                case Mov4500Type.WORD:
                    var msg = obj as string;
                    if (msg != null)
                    {
                        _msg = msg;
                        Msghistory.Add(msg);
                    }
                    break;
                case Mov4500Type.IMAGE:

                    break;
                default:
                    throw new Exception("undefined data type!");
            }
        }
        internal byte[] Package(DataType mType)
        {
            Array.Clear(_mfskBytes,0,_mfskBytes.Length);
            Array.Clear(_mpskBytes,0,_mpskBytes.Length);
            switch (mType)
            {
                case DataType.MFSK:
                    break;
                case DataType.MPSK:
                    break;
                default:
                    throw new Exception("undefined modulation type!");
            }
            return _mfskBytes;
        }
        public byte[] PackageMFSKBytes
        {
            get { return Package(DataType.MFSK); }
        }
        public byte[] PackageMPSKBytes
        {
            get { return Package(DataType.MPSK); }
        }

        public List<string> Msghistory
        {
            get { return _msghistory; }          
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

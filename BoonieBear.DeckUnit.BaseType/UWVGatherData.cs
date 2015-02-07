using System.Collections.Generic;
using BoonieBear.DeckUnit.Protocol.ACMSeries;
namespace BoonieBear.DeckUnit.MovDataManage
{
    /// <summary>
    /// 潜器
    /// </summary>
    public class UWVGatherData
    {
        private readonly static object Syncobject = new object();
        private static UWVGatherData _uwvGatherData;
        private byte[] _packageBytes = new byte[GlobalVariables.ShipMFSKSize];
        private Bpdata _bpdata;
        private Bsssdata _bsssdata;
        private Subposition _subposition;
        private Ctddata _ctddata;
        private Lifesupply _lifesupply;
        private Energysys _energysys;
        private Alertdata _alertdata;
        private Switchdata _switchdata;
        private Adcpdata _adcpdata;
        private List<string> _wordList =new List<string>();
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

        internal byte[] Package()
        {

            return _packageBytes;
        }
        public byte[] PackageBytes
        {
            get { return Package(); }
        }
        
        public static UWVGatherData GetInstance()
        {
            if (_uwvGatherData == null)
            {
                lock (Syncobject)
                {
                    _uwvGatherData = new UWVGatherData();
                }
                return _uwvGatherData;
            }
            return _uwvGatherData;
        }

        
    }
}

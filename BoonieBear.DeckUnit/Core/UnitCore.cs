using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.CommLib.Serial;
using BoonieBear.DeckUnit.CommLib.TCP;
using BoonieBear.DeckUnit.CommLib.UDP;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.Events;
using BoonieBear.DeckUnit.ICore;
using BoonieBear.DeckUnit.UBP;
using TinyMetroWpfLibrary.EventAggregation;
using BoonieBear.DeckUnit.UnitBoxTraceService;
using BoonieBear.DeckUnit.DUConf;
using BoonieBear.DeckUnit.BaseType;
using BoonieBear.DeckUnit.LiveService;
namespace BoonieBear.DeckUnit.Core
{
    /// <summary>
    /// 核心业务类，包括命令，通信服务，工作逻辑
    /// </summary>
    class UnitCore
    {
        private readonly static object SyncObject = new object();
        //静态接口，用于在程序域中任意位置操作UnitCore中的成员
        private static UnitCore _instance;
        //事件绑定接口，用于事件广播
        private IEventAggregator _eventAggregator;
        //串口服务接口
        private ICommCore _iCommCore;
        //网络服务接口
        private INetCore _iNetCore;
        //文件服务接口
        private IFileCore _iFileCore;
        private UnitTraceService _unitTraceService;
        //基础配置信息
        private DeckUnitConf _deckUnitConf;
        private CommConfInfo _commConf;
        private ModemConfigure _modemConf;
        private BaseInfo _baseInfo;
        private Observer<CustomEventArgs> _observer;
        private bool _serviceStarted = false;
        public string Error { get; private set; }
        public UnitTraceService UnitTraceService
        {
            get { return _unitTraceService ?? (_unitTraceService = new UnitTraceService()); }
        }
        public static UnitCore GetInstance()
        {
            lock (SyncObject)
            {

                return _instance ?? (_instance = new UnitCore());
            }
        }
        protected UnitCore()
        {
            
            LoadConfiguration();

        }

        public bool LoadConfiguration()
        {
            bool ret = true;
            try
            {
                _deckUnitConf = DeckUnitConf.GetInstance();
                _modemConf = _deckUnitConf.GetModemConfigure();
                _commConf = _deckUnitConf.GetCommConfInfo();
                _baseInfo = _deckUnitConf.GetBaseInfo();

            }
            catch (Exception ex)
            {
                ret = false;
                EventAggregator.PublishMessage(new LogEvent(ex.Message, ex, LogType.Error));
            }
            return ret;
        }


        public IEventAggregator EventAggregator
        {
            get { return _eventAggregator ?? (_eventAggregator = UnitKernal.Instance.EventAggregator); }
        }

       

        public INetCore INetCore
        {
            get { return _iNetCore ?? (_iNetCore = NetLiveService.GetInstance(_commConf, _observer)); }
        }

        public bool Start()
        {
            try
            {
                INetCore.Initialize();
                INetCore.Start();
                _serviceStarted = INetCore.IsWorking;
                Error = INetCore.Error;
                return _serviceStarted;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return false;
            }
            
            
        }

        public void Stop()
        {
            if(_serviceStarted)
                INetCore.Stop();
            _serviceStarted = false;
        }
        public bool ServiceOK
        {
            get { return _serviceStarted; }
        }

        public DeckUnitConf DeckUnitConf
        {
            get { return _deckUnitConf; }
        }

        public Observer<CustomEventArgs> Observer
        {
            get { return _observer ?? (_observer = new DeckUnitDataObserver()); }

        }

        public IFileCore IFileCore
        {
            get { return _iFileCore; }
            set { _iFileCore = value; }
        }

        public ICommCore ICommCore
        {
            get { return _iCommCore ?? (_iCommCore = CommService.GetInstance(_commConf, _observer)); }
        }

        public static UnitCore Instance
        {
            get { return GetInstance(); }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.CommLib.Serial;
using BoonieBear.DeckUnit.CommLib.TCP;
using BoonieBear.DeckUnit.CommLib.UDP;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.Events;
using BoonieBear.DeckUnit.ICore;
using BoonieBear.DeckUnit.Models;
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
        private static UnitCore _instance = null;
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
        //程序基本信息
        private BaseInfo _baseInfo;
        //数据处理观察类
        private Observer<CustomEventArgs> _observer;
        //服务启动
        private bool _serviceStarted = false;
        //错误信息
        public string Error { get; private set; }
        
        public Mutex AcnMutex { get; set; }//全局acn解析锁
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
            AcnMutex = new Mutex();
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
                if(_deckUnitConf==null)
                    throw new Exception("甲板单元配置信息丢失");
                if (_modemConf == null)
                    throw new Exception("通信机配置信息丢失");
                if (_commConf == null)
                    throw new Exception("通信配置信息丢失");
                if (_baseInfo == null)
                    throw new Exception("甲板单元基本信息丢失");

            }
            catch (Exception ex)
            {
                ret = false;
                EventAggregator.PublishMessage(new LogEvent(ex.Message, LogType.Both));
            }
            return ret;
        }


        public IEventAggregator EventAggregator
        {
            get { return _eventAggregator ?? (_eventAggregator = UnitKernal.Instance.EventAggregator); }
        }

        
        public bool Start()
        {
            try
            {
                
                if(LoadConfiguration()==false)
                    return false;
                ACNProtocol.Init(_modemConf.ID);
                if (UnitTraceService.CreateService(DeckUnitConf.Connectstring) == false || DeckDataProtocol.Init(_modemConf.ID, "default.dudb") == false)
                     throw new Exception("数据库初始化失败");
                
                if (CommEngine != null)
                {
                    //CommEngine.Initialize();
                    //CommEngine.Start();
                    //var cmd = MSPHexBuilder.Pack250(true);
                    //CommEngine.SendCMD(cmd);//进入调试模式，开启网络
                    //Thread.Sleep(500);
                    CheckAddrAvailable(500); //wait dsp start network

                }
                if (NetEngine != null)
                {
                    NetEngine.Initialize();
                    NetEngine.Start();
                }
                _serviceStarted = NetEngine.IsWorking && CommEngine.IsWorking;
                Error = NetEngine.Error;
                return _serviceStarted;
            }
            catch (Exception e)
            {
                _serviceStarted = false;
                EventAggregator.PublishMessage(new LogEvent(e.Message, LogType.Both));
            }
            return _serviceStarted;

        }
        /// <summary>
        /// 测试通信机IP是否可以ping，测试N次，成功即返回true，不成功返回false
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private bool CheckAddrAvailable(int timeout)
        {
            var ipaddr = IPAddress.Parse(_commConf.LinkIP);
            Ping p = new Ping();
            try
            {
                PingReply pr;
                for (int i = 0; i < 4; i++)
                {
                    pr = p.Send(ipaddr, timeout);
                    if (pr.Status == IPStatus.Success)
                    {
                        Debug.WriteLine("ping ok!");
                        return true;
                    }
                    Debug.WriteLine("ping timeout!");

                }
                return false;
            }
            catch
            {
                return false;
            }
            
        }
        public void Stop()
        {
            DeckDataProtocol.Dispose();
            if (NetEngine!=null&&NetEngine.IsWorking)
            {
                NetEngine.Stop();
            }
            if (CommEngine != null && CommEngine.IsWorking)
            {
                CommEngine.Stop();
            }
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
        public INetCore NetEngine
        {
            get { return _iNetCore ?? (_iNetCore = NetLiveService_ACN.GetInstance(_commConf, Observer)); }
        }

        public IFileCore FileEngine
        {
            get { return _iFileCore; }
            set { _iFileCore = value; }
        }

        public ICommCore CommEngine
        {
            get { return _iCommCore ?? (_iCommCore = CommService.GetInstance(_commConf, Observer)); }
        }

        public static UnitCore Instance
        {
            get { return GetInstance(); }
        }
    }
}

using System;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.CommLib.Properties;
using BoonieBear.DeckUnit.CommLib.UDP;
using BoonieBear.DeckUnit.LiveService;
using BoonieBear.DeckUnit.TraceFileService;
using TinyMetroWpfLibrary.EventAggregation;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.Mov4500Conf;
using BoonieBear.DeckUnit.Mov4500TraceService;
using BoonieBear.DeckUnit.ICore;
using BoonieBear.DeckUnit.BaseType;
using BoonieBear.DeckUnit.Mov4500UI.Events;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using System.Threading.Tasks;
using BoonieBear.DeckUnit.WaveBox;
namespace BoonieBear.DeckUnit.Mov4500UI.Core
{
    /// <summary>
    /// 核心业务类，包括通信服务，数据解析，服务状态及一些其他的系统变量
    /// </summary>
    public class UnitCore
    {
        private readonly static object SyncObject = new object();
        //静态接口，用于在程序域中任意位置操作UnitCore中的成员
        private static UnitCore _instance;
        //事件绑定接口，用于事件广播
        private IEventAggregator _eventAggregator;
        //网络服务接口
        private IMovNetCore _iNetCore;
        //串口服务接口，如果有
        private ICommCore _iCommCore;
        //文件服务接口
        private IFileCore _iFileCore;
        private MovTraceService _movTraceService;
        //基础配置信息
        private MovConf _mov4500Conf;//系统设置类
        private MovConfInfo _movConfInfo;//除通信以外其他设置类
        private CommConfInfo _commConf;//通信设置
        private Observer<CustomEventArgs> _observer; 
        private bool _serviceStarted = false;
        public string Error { get; private set; }
        public MonitorMode WorkMode{get; private set;}
        public Mutex ACMMutex { get; set; }//全局解析锁
        public byte[] Single = null;
        public byte[] RecvOrOK = null;
        public byte[] AskOrOK = null;
        public byte[] AgreeOrReqRise = null;
        public byte[] RiseOrUrgent = null;
        public byte[] DisgOrRelBuoy = null;
        public MovTraceService MovTraceService
        {
            get { return _movTraceService ?? (_movTraceService = new MovTraceService(WorkMode)); }
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
            
            ACMMutex = new Mutex();

        }

        public bool LoadConfiguration()
        {
            bool ret = true;
            try
            {
                _mov4500Conf = MovConf.GetInstance();
                _commConf = _mov4500Conf.GetCommConfInfo();
                _movConfInfo = _mov4500Conf.GetMovConfInfo();
                WorkMode = (MonitorMode)Enum.Parse(typeof(MonitorMode),_movConfInfo.Mode.ToString());
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



        public IMovNetCore NetCore
        {
            get { return _iNetCore ?? (_iNetCore = NetLiveService_ACM.GetInstance(_commConf, _movConfInfo, Observer)); }
        }
        public ICommCore CommCore
        {
            get { return _iCommCore ?? (_iCommCore = CommService.GetInstance(_commConf, Observer)); }
        }

        private bool LoadMorse()
        {
            string soundpath = MovConf.GetInstance().MyExecPath + "\\" + "morse";
            if (Directory.Exists(soundpath))
            {
                try
                {
                    Single = File.ReadAllBytes(soundpath + "\\" + "1.dat");
                    RecvOrOK = File.ReadAllBytes(soundpath + "\\" + "3.dat");
                    AskOrOK = File.ReadAllBytes(soundpath + "\\" + "2.dat");
                    AgreeOrReqRise = File.ReadAllBytes(soundpath + "\\" + "22.dat");
                    RiseOrUrgent = File.ReadAllBytes(soundpath + "\\" + "5.dat");
                    DisgOrRelBuoy = File.ReadAllBytes(soundpath + "\\" + "33.dat");
                    return true;
                }
                catch (Exception)
                {
                    //do nothing
                }
                return false;
            }
            return false;
        }
        public bool Start()
        {
            try
            {
                if(!LoadConfiguration()) throw new Exception("无法读取基本配置");
                if (!LoadMorse()) throw new Exception("无法读取Morse数据");
                if(NetCore.IsInitialize)
                    NetCore.Stop();
                NetCore.Initialize();
                NetCore.Start();
                if(CommCore.IsInitialize)
                    CommCore.Stop();
                CommCore.Initialize();
                CommCore.Start();
                if(!MovTraceService.CreateService()) throw new Exception("数据保存服务启动失败");
                _serviceStarted = true;
                Error = NetCore.Error;
                return _serviceStarted;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                EventAggregator.PublishMessage(new LogEvent(ex.Message, LogType.OnlyLog));
                return false;
            }
            
            
        }

        public void Stop()
        {
            if (NetCore.IsWorking)
                NetCore.Stop();
            if (CommCore.IsWorking)
                CommCore.Stop();
            MovTraceService.TearDownService();
            _serviceStarted = false;
            
        }
        public bool IsWorking
        {
            get { return _serviceStarted; }
        }

        public MovConf MovConfigueService
        {
            get { return _mov4500Conf; }
        }

        public Observer<CustomEventArgs> Observer
        {
            get { return _observer ?? (_observer = new Mov4500DataObserver()); }

        }

        public IFileCore FileCore
        {
            get { return _iFileCore; }
            set { _iFileCore = value; }
        }

        public static UnitCore Instance
        {
            get { return GetInstance(); }
        }
        public Model3D ShipModel { get; set; }
        public Model3D MovModel { get; set; }
        private async Task<Model3DGroup> LoadAsync(string model3DPath, bool freeze)
        {
            return await Task.Factory.StartNew(() =>
            {
                var mi = new ModelImporter();

                // Alt 1. - freeze the model 
                return mi.Load(model3DPath, null, true);

            });
        }
    }
}

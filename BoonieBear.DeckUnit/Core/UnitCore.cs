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
using BoonieBear.TinyMetro.WPF.EventAggregation;
using BoonieBear.DeckUnit.UnitBoxTraceService;
using BoonieBear.DeckUnit.DUConf;
namespace BoonieBear.DeckUnit.Core
{
    /// <summary>
    /// 核心业务类，包括命令，通信服务，工作逻辑
    /// </summary>
    class UnitCore : IUnitCore
    {
        //静态接口，用于在程序域中任意位置操作UnitCore中的成员
        public static IUnitCore Instance;
        //事件绑定接口，用于事件广播
        private IEventAggregator _eventAggregator;
        private ISerialService _serialService;
        private ITCPClientService _tcpShellService;
        private ITCPClientService _tcpDataService;
        private IUDPService _udpService;
        private UnitTraceService _unitTraceService;
        private TcpClient _shelltcpClient ;
        private TcpClient _datatcpClient ;
        private UdpClient _udpClient;
        private DeckUnitConf _deckUnitConf;
        private bool _isWorking;
        private bool _initialed;
        private CommLib.IObserver<CustomEventArgs> _deckUnitObserver;

        public bool Init()
        {
            try
            {
                if (Initailed) throw new Exception("系统已经完成初始化");
                //读取配置文件
                string connstr = DeckConfigure.GetSqlString();
                if (!TraceService.CreateService(connstr)) throw new Exception("数据存储服务无法初始化");
                //大数据传输协议自带数据库接口，可以
                //自动更新数据库，如果是通信网则使用ACNProtocol.Init()
                var modemconfigure = DeckConfigure.GetModemConfigure();
                if (modemconfigure==null)
                    throw new Exception("无法读取配置信息(通信机)");
                if (!DeckDataProtocol.Init(modemconfigure.ID, connstr)) throw new Exception("数据传输协议无法初始化！");
 
                var configure = DeckConfigure.GetCommConfInfo();
                if (configure==null)
                    throw new Exception("无法读取配置信息(通信)");
                if (!CreateSerialService(configure)) throw new Exception("内部端口服务无法初始化");
                if (!CreateUDPService(configure.TraceUDPPort)) throw new Exception("数据交换服务无法初始化");
                if (!CreateTCPService(configure)) throw new Exception("指令交换服务无法初始化");
                Initailed = true;
            }
            catch (Exception ex)
            {
                EventAggregator.PublishMessage(new LogEvent(ex.Message, ex, LogType.Error));
                Initailed = false;
                
            }
            return Initailed;

        }

        private bool CreateUDPService(int port)
        {
            if (_udpClient == null)
                _udpClient = new UdpClient(port);
            if (!UDPService.Init(_udpClient)) return false;
            if (!UDPService.Start()) return false;
            UDPService.Register(DeckUnitObserver);
            return true;
        }

        public void Dispose()
        {
            TraceService.TearDownService();
            
            if (Initailed)
            {
                SerialService.UnRegister(DeckUnitObserver);
                SerialService.Stop();

                TCPShellService.UnRegister(DeckUnitObserver);
                TCPShellService.Stop();

                TCPDataService.UnRegister(DeckUnitObserver);
                TCPDataService.Stop();

                UDPService.UnRegister(DeckUnitObserver);
                UDPService.Stop();
            }
            Initailed = false;
        }

        /// <summary>
        /// tcp数据交换初始化
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        private bool CreateTCPService(CommConfInfo configure)
        {
            _shelltcpClient = new TcpClient {SendTimeout = 1000};
            _datatcpClient = new TcpClient {SendTimeout = 1000};
            if (!TCPShellService.Init(_shelltcpClient, IPAddress.Parse(configure.LinkIP), configure.NetPort1) ||
                (!TCPDataService.Init(_datatcpClient, IPAddress.Parse(configure.LinkIP), configure.NetPort2)))
                return false;
            // 同步方法，会阻塞进程，调用init用task
            TCPShellService.ConnectSync();
            TCPDataService.ConnectSync();
            if (!TCPShellService.Connected || !TCPDataService.Connected) return false;
            TCPShellService.Register(DeckUnitObserver);
            TCPDataService.Register(DeckUnitObserver);
            return true;
        }

        /// <summary>
        ///  初始化串口数据服务
        /// </summary>
        /// <param name="configure">通信参数</param>
        /// <returns>成功or失败</returns>
        private bool CreateSerialService(CommConfInfo configure)
        {
            if (!SerialService.Init(new SerialPort(configure.SerialPort)) || !SerialService.Start()) return false;
            SerialService.Register(DeckUnitObserver);
            return true;
        }

        #region 属性
        public ISerialService SerialService 
        {
            get { return _serialService ?? (_serialService = (new ACNSerialServiceFactory()).CreateService()); }
        }

        public ITCPClientService TCPDataService
        {
            get
            {
                return _tcpDataService ?? (_tcpDataService = (new TCPDataServiceFactory()).CreateService());
            }
        }

        public ITCPClientService TCPShellService
        {
            get
            {
                return _tcpShellService ?? (_tcpShellService = (new TCPShellServiceFactory()).CreateService());
            }
        }

        public IUDPService UDPService
        {
            get
            {
                return _udpService ?? (_udpService = (new UDPDataServiceFactory()).CreateService());
            }
        }

        public UnitTraceService TraceService
        {
            get
            {
                return _unitTraceService ?? (_unitTraceService = new UnitTraceService());
            }
        }

        public CommLib.IObserver<CustomEventArgs> DeckUnitObserver
        {
            get { return _deckUnitObserver ?? (_deckUnitObserver = new DeckUnitDataObserver()); }
            
        }

        public bool IsWorking
        {
            get { return _isWorking; }
            set { _isWorking = value; }
        }

        public bool Initailed
        {
            get { return _initialed; }
            set { _initialed = value; }
        }

        public IEventAggregator EventAggregator
        {
            get { return _eventAggregator ?? (_eventAggregator = UnitKernal.Instance.EventAggregator); }
        }

        public DeckUnitConf DeckConfigure
        {
            get { return _deckUnitConf ?? (_deckUnitConf = DeckUnitConf.GetInstance()); }
        }

        #endregion
    }
}

using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.CommLib.Protocol;
using BoonieBear.DeckUnit.CommLib.Serial;
using BoonieBear.DeckUnit.CommLib.TCP;
using BoonieBear.DeckUnit.CommLib.UDP;
using BoonieBear.DeckUnit.Core.DataObservers;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.DAL.DBModel;
using BoonieBear.DeckUnit.DAL.SqliteDAL;
using BoonieBear.DeckUnit.Events;
using BoonieBear.TinyMetro.WPF.EventAggregation;

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
        private ISqlDAL _sqlDAL;
        private TcpClient _shelltcpClient ;
        private TcpClient _datatcpClient ;
        private UdpClient _udpClient;
        private bool _isWorking;
        private bool _initialed;
        private CommLib.IObserver<CustomEventArgs> _deckUnitObserver;
        private const string Dblinkstring = @"Data Source=..\dudb\default.dudb;Pooling=True";

        public bool Init()
        {
            try
            {
                if (Initailed) throw new Exception("系统已经完成初始化");
                if (!SqlDAL.LinkStatus) throw new Exception("数据存储服务无法启动");
                var configure = SqlDAL.GetCommConfInfo();
                var modemconfigure = SqlDAL.GetModemConfigure();
                if (modemconfigure == null) throw new Exception("配置数据不存在");
                //甲板单元协议自带数据库接口，可以
                //自动更新数据库，如果是通信网则使用ACNProtocol.Init()
                DeckDataProtocol.Init(modemconfigure.ID, Dblinkstring);
                //串口打开成功
                if (!SerialInit(configure)) throw new Exception("内部端口服务无法初始化");
                if (!UDPInit()) throw new Exception("数据交换服务无法初始化");
                if (!TcpInit(configure)) throw new Exception("信息交换服务无法初始化");
                Initailed = true;
            }
            catch (Exception ex)
            {
                EventAggregator.PublishMessage(new LogEvent(ex.Message, ex, LogType.Error));
                Initailed = false;
                
            }
            return Initailed;

        }

        private bool UDPInit()
        {
            if (_udpClient == null)
                _udpClient = new UdpClient(10010);
            if (!UDPService.Init(_udpClient)) return false;
            if (!UDPService.Start()) return false;
            UDPService.Register(DeckUnitObserver);
            return true;
        }

        public void Dispose()
        {
            if (SqlDAL.LinkStatus)
                SqlDAL.Close();
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
        private bool TcpInit(CommConfInfo configure)
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
        private bool SerialInit(CommConfInfo configure)
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

        public ISqlDAL SqlDAL
        {
            get
            {
                return _sqlDAL ?? (_sqlDAL = new SqliteSqlDAL(Dblinkstring));
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

        #endregion
    }
}

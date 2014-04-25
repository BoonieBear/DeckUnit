using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.CommLib.Serial;
using BoonieBear.DeckUnit.CommLib.TCP;
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
        private static IUnitCore Instance;
        private IEventAggregator _eventAggregator;
        private ISerialService _serialService;
        private ITCPClientService _tcpShellService;
        private ITCPClientService _tcpDataService;
        private IUDPService _udpService;
        private ISqlDAL _sqlDAL;
        private TcpClient shelltcpClient = null;
        private TcpClient datatcpClient = null;
        private CommLib.IObserver<CustomEventArgs> _deckUnitObserver; 
        public bool Init()
        {
            try
            {
                if (!SqlDAL.LinkStatus) throw new Exception("数据服务无法启动");
                var configure = SqlDAL.GetCommConfInfo();
                //串口打开成功
                if (!SerialInit(configure)) throw new Exception("内部端口服务无法启动");
                return TcpInit(configure);
            }
            catch (Exception ex)
            {
                EventAggregator.PublishMessage(new LogEvent(ex.Message, ex, LogType.Error));
                return false;
            }

        }
        /// <summary>
        /// tcp数据交换初始化
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        private bool TcpInit(CommConfInfo configure)
        {
            shelltcpClient = new TcpClient {SendTimeout = 1000};
            datatcpClient = new TcpClient {SendTimeout = 1000};
            if (!_tcpShellService.Init(shelltcpClient, IPAddress.Parse(configure.LinkIP), configure.NetPort1) ||
                (!_tcpDataService.Init(datatcpClient, IPAddress.Parse(configure.LinkIP), configure.NetPort2)))
                throw new Exception("无法初始化数据交换服务");
            // 同步方法，会阻塞进程，调用init用task
            _tcpShellService.ConnectSync();
            _tcpDataService.ConnectSync();
            if (!_tcpShellService.Connected || !_tcpDataService.Connected) throw new Exception("无法连接数据交换服务");
            _tcpShellService.Register(DeckUnitObserver);
            _tcpDataService.Register(DeckUnitObserver);
            return true;
        }

        /// <summary>
        ///  初始化串口数据服务
        /// </summary>
        /// <param name="configure">通信参数</param>
        /// <returns>成功or失败</returns>
        private bool SerialInit(CommConfInfo configure)
        {
            return _serialService.Init(new SerialPort(configure.SerialPort)) && _serialService.Start();
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
        public IUDPService UDPService { get; private set; }

        public ISqlDAL SqlDAL
        {
            get
            {
                return _sqlDAL ?? (_sqlDAL = new SqliteSqlDAL(@"Data Source=..\db\default.dudb;Pooling=True"));
            } 
        }

        public CommLib.IObserver<CustomEventArgs> DeckUnitObserver
        {
            get
            {
                return _deckUnitObserver ?? (new DeckUnitDataObserver());
            }
        }

        bool IUnitCore.IsWorking
        {
            get { return isWorking; }
            set { isWorking = value; }
        }

        private static  bool isWorking { get; set; }

        public IEventAggregator EventAggregator
        {
            get { return _eventAggregator ?? (_eventAggregator = UnitKernal.Instance.EventAggregator); }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BoonieBear.DeckUnit.ICore;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.CommLib.TCP;
using BoonieBear.DeckUnit.CommLib.UDP;
using BoonieBear.DeckUnit.BaseType;
namespace BoonieBear.DeckUnit.LiveService
{
    public class NetLiveService:INetCore
    {
        private readonly static object SyncObject = new object();
        private static INetCore _netInstance;
        private ITCPClientService _tcpShellService;
        private ITCPClientService _tcpDataService;
        private IUDPService _udpService;
        private TcpClient _shelltcpClient;
        private TcpClient _datatcpClient;
        private UdpClient _udpClient;
        private CommConfInfo _commConf;
        private Observer<CustomEventArgs> _DataObserver;
        public string Error { get; set; }
        public bool IsInitialize { get; set; }
        public bool IsWorking{ get; set; }

        public static INetCore GetInstance(CommConfInfo conf, Observer<CustomEventArgs> observer)
        {
            lock (SyncObject)
            {

                return _netInstance ?? (_netInstance = new NetLiveService(conf, observer));
            }
        }
        private bool CreateUDPService(int port)
        {
           
            if (!UDPService.Start()) return false;
            UDPService.Register(NetDataObserver);
            return true;
        }
        /// <summary>
        /// tcp数据交换初始化
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        private bool CreateTCPService(CommConfInfo configure)
        {
            
            // 同步方法，会阻塞进程，调用init用task
            TCPShellService.ConnectSync();
            TCPDataService.ConnectSync();
            if (!TCPShellService.Connected || !TCPDataService.Connected) return false;
            TCPShellService.Register(NetDataObserver);
            TCPDataService.Register(NetDataObserver);
            return true;
        }

        protected NetLiveService(CommConfInfo conf, Observer<CustomEventArgs> observer)
        {
            _commConf = conf;
            _DataObserver = observer;
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

        public Observer<CustomEventArgs> NetDataObserver
        {
            get { return _DataObserver; }
            set { _DataObserver = value; }
        }

     
        public void Initialize()
        {
            if (IsInitialize)
            {
                throw new Exception("服务已初始化");
            }
            _shelltcpClient = new TcpClient { SendTimeout = 1000 };
            _datatcpClient = new TcpClient { SendTimeout = 1000 };
            if (!TCPShellService.Init(_shelltcpClient, IPAddress.Parse(_commConf.LinkIP), _commConf.NetPort1) ||
                (!TCPDataService.Init(_datatcpClient, IPAddress.Parse(_commConf.LinkIP), _commConf.NetPort2)))
                throw new Exception("通信网络初始化失败");
            if (_udpClient == null)
                _udpClient = new UdpClient(_commConf.TraceUDPPort);
            if (!UDPService.Init(_udpClient)) throw new Exception("调试网络初始化失败");
            IsInitialize = true;
        }

        public void Stop()
        {
            if (IsInitialize)
            {

                TCPShellService.UnRegister(NetDataObserver);
                TCPShellService.Stop();

                TCPDataService.UnRegister(NetDataObserver);
                TCPDataService.Stop();

                UDPService.UnRegister(NetDataObserver);
                UDPService.Stop();
            }
            IsWorking = false;
            IsInitialize = false;
        }

        public void Start()
        {
            IsWorking = false;
            if (_commConf == null || _DataObserver == null)
                throw new Exception("无法设置网络通信");
            if (!CreateUDPService(_commConf.TraceUDPPort)) throw new Exception("数据服务无法初始化");
            if (!CreateTCPService(_commConf)) throw new Exception("命令服务无法初始化");
            IsWorking = true;
        }

        
    }
}

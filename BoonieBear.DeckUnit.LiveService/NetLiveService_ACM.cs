using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BoonieBear.DeckUnit.ICore;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.CommLib.TCP;
using BoonieBear.DeckUnit.CommLib.UDP;
using BoonieBear.DeckUnit.BaseType;
namespace BoonieBear.DeckUnit.LiveService
{
    public class NetLiveService_ACM:INetCore
    {
        private readonly static object SyncObject = new object();
        private static INetCore _netInstance;
        private ITCPClientService _tcpShellService;
        private ITCPClientService _tcpDataService;

        private TcpClient _shelltcpClient;
        private TcpClient _datatcpClient;

        private UdpClient _movClient;//广播端口
        private UdpClient _gpsClient;
        private UdpClient _usblClient;
        private UdpClient _uwaClient;

        private IUDPService _gpsService;
        private IUDPService _uwaService;
        private IUDPService _usblService;
        
        private CommConfInfo _commConf;
        private MovConfInfo _mocConf;
        private Observer<CustomEventArgs> _DataObserver;
        public string Error { get; set; }
        public bool IsInitialize { get; set; }
        public bool IsWorking{ get; set; }
        public int SendBytes { get; set; }

        public static INetCore GetInstance(CommConfInfo conf,MovConfInfo movconf, Observer<CustomEventArgs> observer)
        {
            lock (SyncObject)
            {
                if (conf!=null)
                    return _netInstance ?? (_netInstance = new NetLiveService_ACM(conf, movconf, observer));
                else
                {
                    return null;
                }
            }
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
            TCPShellService.Register(NetDataObserver);
            TCPDataService.Register(NetDataObserver);
            if (TCPShellService.Connected&&TCPShellService.Start()&&TCPDataService.Connected&&TCPDataService.Start())
                return true;
            return false;
        }
        /// <summary>
        /// UDP数据交换初始化
        /// </summary>
        /// <returns></returns>
        protected bool CreateUDPService()
        {
            if (!GpsService.Start()) return false;
            GpsService.Register(NetDataObserver);
            if (!USBLService.Start()) return false;
            USBLService.Register(NetDataObserver);
            if (!UwaService.Start()) return false;
            UwaService.Register(NetDataObserver);
            return true;
        }

        protected void StopUDPService()
        {
            GpsService.Stop();
            GpsService.UnRegister(NetDataObserver);
            USBLService.Stop();
            USBLService.UnRegister(NetDataObserver);
            UwaService.Stop();
            UwaService.UnRegister(NetDataObserver);
        }
        protected NetLiveService_ACM(CommConfInfo conf, MovConfInfo movconf, Observer<CustomEventArgs> observer)
        {
            _commConf = conf;
            _mocConf = movconf;
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
        public IUDPService GpsService
        {
            get { return _gpsService ?? (_gpsService = (new ACMGPSServiceFactory()).CreateService()); }
        }
        public IUDPService UwaService
        {
            get { return _uwaService ?? (_uwaService = (new ACMUWAServiceFactory()).CreateService()); }
        }
        public IUDPService USBLService
        {
            get { return _usblService ?? (_usblService = (new ACMUSBLServiceFactory()).CreateService()); }
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
            _movClient = new UdpClient();
            if (!TCPShellService.Init(_shelltcpClient, IPAddress.Parse(_commConf.LinkIP), _commConf.NetPort1) ||
                (!TCPDataService.Init(_datatcpClient, IPAddress.Parse(_commConf.LinkIP), _commConf.NetPort2)))
                throw new Exception("通信网络初始化失败");
            if (_gpsClient == null)
                _gpsClient = new UdpClient(_mocConf.GPSPort);
            if (!GpsService.Init(_gpsClient)) throw new Exception("GPS端口打开失败");
            if (_uwaClient == null)
                _uwaClient = new UdpClient(_mocConf.SailPort);
            if (!UwaService.Init(_uwaClient)) throw new Exception("航控端口打开失败");
            if (_usblClient == null)
                _usblClient = new UdpClient(_mocConf.USBLPort);
            if (!USBLService.Init(_usblClient)) throw new Exception("USBL端口打开失败");
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

              

            }
            IsWorking = false;
            IsInitialize = false;
        }

        public void Start()
        {
            IsWorking = false;
            if (_commConf == null || _DataObserver == null)
                throw new Exception("无法设置网络通信");
            if (!CreateTCPService(_commConf)) throw new Exception("通信服务无法启动");
            if (!CreateUDPService()) throw new Exception("启动广播网络失败");
            IsWorking = true;
        }


        public Task<bool> BroadCast(byte[] buf)
        {
            throw new NotImplementedException();
        }





        public Task<bool> SendConsoleCMD(string cmd)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendCMD(byte[] buf)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendFile(Stream file)
        {
            throw new NotImplementedException();
        }
    }
}

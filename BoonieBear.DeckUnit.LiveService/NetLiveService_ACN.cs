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
    public class NetLiveService_ACN:INetCore
    {
        private readonly static object SyncObject = new object();
        private static INetCore _netInstance;
        private ITCPClientService _tcpShellService;
        private ITCPClientService _tcpDataService;

        private TcpClient _shelltcpClient;
        private TcpClient _datatcpClient;

        //udp网络
        private UdpClient _udpTraceClient;
        private UdpClient _udpDataClient;
        private IUDPService _udpTraceService;
        private IUDPService _udpDataService;
        private CommConfInfo _commConf;
        private Observer<CustomEventArgs> _DataObserver;
        public string Error { get; set; }
        public bool IsInitialize { get; set; }
        public bool IsWorking{ get; set; }
        public int SendBytes { get; set; }

        public static INetCore GetInstance(CommConfInfo conf, Observer<CustomEventArgs> observer)
        {
            lock (SyncObject)
            {
                if (conf!=null)
                    return _netInstance ?? (_netInstance = new NetLiveService_ACN(conf, observer));
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

        protected NetLiveService_ACN(CommConfInfo conf,Observer<CustomEventArgs> observer)
        {
            _commConf = conf;
            _DataObserver = observer;
        }

        public IUDPService UDPDataService
        {
            get
            {
                return _udpDataService ?? (_udpDataService = (new UDPDataServiceFactory()).CreateService());
            }
        }
        public IUDPService UDPTraceService
        {
            get
            {
                return _udpTraceService ?? (_udpTraceService = (new UDPDebugServiceFactory()).CreateService());
            }
        }
        private bool CreateUDPService()
        {
            if (!UDPTraceService.Start()) return false;
            UDPTraceService.Register(NetDataObserver);
            if (!UDPDataService.Start()) return false;
            UDPDataService.Register(NetDataObserver);
            return true;
        }
        private bool StopUDPService()
        {
            UDPTraceService.Stop();
            UDPTraceService.UnRegister(NetDataObserver);
            UDPDataService.Stop();
            UDPDataService.UnRegister(NetDataObserver);
            return true;
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
            if (_udpTraceClient == null)
                _udpTraceClient = new UdpClient(_commConf.TraceUDPPort);
            if (!UDPTraceService.Init(_udpTraceClient)) throw new Exception("调试广播网络初始化失败");
            if (_udpDataClient == null)
                _udpDataClient = new UdpClient(_commConf.DataUDPPort);
            if (!UDPDataService.Init(_udpDataClient)) throw new Exception("调试数据网络初始化失败");
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
                StopUDPService();


            }
            IsWorking = false;
            IsInitialize = false;
        }

        public void Start()
        {
            IsWorking = false;
            if (_commConf == null || _DataObserver == null)
                throw new Exception("网络通信无法设置");
            //if (!CreateTCPService(_commConf)) throw new Exception("网络服务无法启动");
            if (!CreateUDPService()) throw new Exception("启动广播网络失败");
            IsWorking = true;
        }


        public Task<bool> SendConsoleCMD(string cmd)
        {
                var shellcmd = new ACNTCPShellCommand(_shelltcpClient, cmd);
                return Command.SendTCPAsync(shellcmd);
        }

        public Task<bool> SendCMD(byte[] buf)
        {
            var ret = SendConsoleCMD("gd -n");
            if (ret.Result)
            {
                TaskEx.Delay(100);
                var cmd = new ACNTCPDataCommand(_datatcpClient, buf);
                return Command.SendTCPAsync(cmd);
            }
            return ret;


        }

        public async Task<bool> SendFile(Stream file)
        {
            SendBytes = 0;
            var shellcmd = new ACNTCPShellCommand(_shelltcpClient, "gd");
            var ret = Command.SendTCPAsync(shellcmd);
            if(await ret)
            {
                var datacmd = new ACNTCPStreamCommand(_datatcpClient, file, reportprogress);
                TCPDataService.Register(datacmd);
                return await Command.SendTCPAsync(datacmd).ContinueWith(x =>
                {
                    TCPDataService.UnRegister(datacmd);
                    return x.Result;
                });
            }
            return false;
        }

        private void reportprogress(int i)
        {
            SendBytes = i;
        }




        public Task<bool> BroadCast(byte[] buf)
        {
            throw new NotImplementedException();
        }
    }
}

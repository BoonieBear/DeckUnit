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
using BoonieBear.DeckUnit.ACMP;

namespace BoonieBear.DeckUnit.LiveService
{
    public class NetLiveService_ACM : IMovNetCore
    {
        private static readonly object SyncObject = new object();
        private static IMovNetCore _netInstance;
        private ITCPClientService _tcpShellService;
        private ITCPClientService _tcpDataService;

        private TcpClient _shelltcpClient;
        private TcpClient _datatcpClient;

        private UdpClient _movClient; //广播端口
        private UdpClient _gpsClient;
        private UdpClient _usblClient;
        private UdpClient _uwaClient;

        private IUDPService _gpsService;
        private IUDPService _uwaService;
        private IUDPService _usblService;

        private CommConfInfo _commConf;
        private MovConfInfo _mocConf;
        private Observer<CustomEventArgs> _DataObserver;
        private int PkgLimit = 1456;

        public string Error { get; set; }
        public bool IsInitialize { get; set; }

        public bool IsUDPWorking
        {
            get
            {
                if (_movClient != null && _uwaClient != null && _usblClient!=null)
                {
                    return true;
                }
                return false;
            }
        }//

        public bool IsTCPWorking
        {
            get
            {
                if (_shelltcpClient != null && _datatcpClient != null)
                {
                    return _shelltcpClient.Connected && _datatcpClient.Connected;
                }
                return false;
            }
        }
        public int SendBytes { get; set; }

        public static IMovNetCore GetInstance(CommConfInfo conf, MovConfInfo movconf, Observer<CustomEventArgs> observer)
        {
            lock (SyncObject)
            {
                if (conf != null)
                    return _netInstance ?? (_netInstance = new NetLiveService_ACM(conf, movconf, observer));
                else
                {
                    return null;
                }
            }
        }

        protected NetLiveService_ACM(CommConfInfo conf, MovConfInfo movconf, Observer<CustomEventArgs> observer)
        {
            _commConf = conf;
            _mocConf = movconf;
            _DataObserver = observer;
        }



        public ITCPClientService TCPDataService
        {
            get { return _tcpDataService ?? (_tcpDataService = (new TCPDataServiceFactory()).CreateService()); }
        }

        public ITCPClientService TCPShellService
        {
            get { return _tcpShellService ?? (_tcpShellService = (new TCPShellServiceFactory()).CreateService()); }
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
            if (_movClient == null)
                _movClient = new UdpClient(4001);
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
                StopTCpService();//关闭网络时必然全部关闭
                StopUDPService();

            }
            IsInitialize = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            Initialize();
            if (_commConf == null || _DataObserver == null)
                throw new Exception("无法设置网络通信");
            //if (!StartTCPService()) throw new Exception("通信机芯连接失败");//按需要单独起tcp服务
            if (!StartUDPService()) throw new Exception("广播网络启动失败");

        }


        public Task<bool> BroadCast(byte[] buf)
        {
            if (_movClient != null)
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"),  _mocConf.BroadCastPort);  
                Task.Factory.StartNew(
                    () => _movClient.Send(buf, buf.Length, endpoint));
            }
            return null;
        }

        public Task<bool> SendConsoleCMD(string cmd)
        {
            var shellcmd = new ACNTCPShellCommand(_shelltcpClient, cmd);
            return Command.SendTCPAsync(shellcmd);
        }

        /// <summary>
        /// 下发除了调制数据外的其他数据
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public Task<bool> SendCMD(byte[] buf)
        {
                var cmd = new ACNTCPDataCommand(_datatcpClient, buf);
                return Command.SendTCPAsync(cmd);
        }

        public async Task<bool> SendFile(Stream file)
        {
            SendBytes = 0;
            var shellcmd = new ACNTCPShellCommand(_shelltcpClient, "gd");
            var ret = Command.SendTCPAsync(shellcmd);
            if (await ret)
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

        //下发调制数据，水声需要手动添加最后0xeded
        public bool Send(int id, byte[] buf)
        {
            if (id == (int) ModuleType.MFSK || id == (int) ModuleType.FH)
            {
                byte[] newBytes = new byte[buf.Length + 4];
                Buffer.BlockCopy(BitConverter.GetBytes(id), 0, newBytes, 0, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(buf.Length), 0, newBytes, 2, 2);
                Buffer.BlockCopy(buf, 0, newBytes, 4, buf.Length);
                SendCMD(newBytes);
                return true;
            }
            else if (id == (int) ModuleType.MPSK || id == (int) ModuleType.SSB)
            {
                
                byte[] newBytes = new byte[PkgLimit + 4];
                int pkgnum = 0;
                pkgnum = buf.Length/PkgLimit;
                for (int i = 0; i < pkgnum; i++)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(id), 0, newBytes, 0, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(PkgLimit), 0, newBytes, 2, 2);
                    Buffer.BlockCopy(buf, i*PkgLimit, newBytes, 4, PkgLimit);
                    var cmd = new ACNTCPDataCommand(_datatcpClient, newBytes);
                    Command.SendTCPSync(cmd);
                }
                //last pkg
                int lastpkglenth = buf.Length - pkgnum*PkgLimit;
                if (lastpkglenth == 0)
                {
                    if (id == (int) ModuleType.MPSK)
                    {
                        Buffer.BlockCopy(BitConverter.GetBytes((int) ModuleType.PSKEND), 0, newBytes, 0, 2);
                        Buffer.BlockCopy(BitConverter.GetBytes(lastpkglenth), 0, newBytes, 2, 2);
                        var endcmd = new ACNTCPDataCommand(_datatcpClient, newBytes);
                        return Command.SendTCPSync(endcmd);
                    }
                    return true;
                }
                    
                if (id == (int) ModuleType.MPSK)
                    Buffer.BlockCopy(BitConverter.GetBytes((int)ModuleType.PSKEND), 0, newBytes, 0, 2);
                if (id == (int) ModuleType.SSB)
                    Buffer.BlockCopy(BitConverter.GetBytes(id), 0, newBytes, 0, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(lastpkglenth), 0, newBytes, 2, 2);
                Buffer.BlockCopy(buf, pkgnum*PkgLimit, newBytes, 4, lastpkglenth);
                var lastcmd = new ACNTCPDataCommand(_datatcpClient, newBytes);
                return Command.SendTCPSync(lastcmd);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool StartUDPService()
        {
            if (!IsInitialize)
                Initialize();
            if (!GpsService.Start()) return false;
            GpsService.Register(NetDataObserver);
            if (!USBLService.Start()) return false;
            USBLService.Register(NetDataObserver);
            if (!UwaService.Start()) return false;
            UwaService.Register(NetDataObserver);

            return true;
        }

        public bool StartTCPService()
        {
            if (_shelltcpClient == null)
                _shelltcpClient = new TcpClient { SendTimeout = 1000 };
            if (_datatcpClient == null)
                _datatcpClient = new TcpClient { SendTimeout = 1000 };
            if (!TCPShellService.Init(_shelltcpClient, IPAddress.Parse(_commConf.LinkIP), _commConf.NetPort1) ||
                (!TCPDataService.Init(_datatcpClient, IPAddress.Parse(_commConf.LinkIP), _commConf.NetPort2)))
                throw new Exception("通信网络初始化失败");
            TCPShellService.ConnectSync();
            TCPDataService.ConnectSync();
            TCPShellService.Register(NetDataObserver);
            TCPDataService.Register(NetDataObserver);
            if (TCPShellService.Connected && TCPShellService.Start() && TCPDataService.Connected &&
                TCPDataService.Start())
            {

                return true;
            }
            _shelltcpClient.Close();
            _datatcpClient.Close();
            _shelltcpClient = null;
            _datatcpClient = null;

            return false;
        }

        public void StopUDPService()
        {
            GpsService.Stop();
            GpsService.UnRegister(NetDataObserver);
            USBLService.Stop();
            USBLService.UnRegister(NetDataObserver);
            UwaService.Stop();
            UwaService.UnRegister(NetDataObserver);
            _gpsClient = null;
            _movClient = null;
            _usblClient = null;
            _uwaClient = null;

            IsInitialize = false;
        }

        public void StopTCpService()
        {
            TCPShellService.UnRegister(NetDataObserver);
            TCPShellService.Stop();

            TCPDataService.UnRegister(NetDataObserver);
            TCPDataService.Stop();
            _shelltcpClient = null;
            _datatcpClient = null;

            IsInitialize = false;
        }


        public bool IsWorking
        {
            get
            {
                return IsUDPWorking && IsTCPWorking;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        //水声结束包
        public bool SendSSBEND()
        {
            byte[] newBytes = new byte[4];
            Buffer.BlockCopy(BitConverter.GetBytes((int)ModuleType.SSBEND), 0, newBytes, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(2), 0, newBytes, 2, 2);
            var cmd = new ACNTCPDataCommand(_datatcpClient, newBytes);
            return Command.SendTCPSync(cmd);
        }

        //发送空包，用于ssb和voice第一包之前
        public bool SendSSBNon()
        {
            byte[] newBytes = new byte[PkgLimit + 4];
            Buffer.BlockCopy(BitConverter.GetBytes((int)ModuleType.SSB), 0, newBytes, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(PkgLimit), 0, newBytes, 2, 2);
            var cmd = new ACNTCPDataCommand(_datatcpClient, newBytes);
            return Command.SendTCPSync(cmd);

        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BoonieBear.DeckUnit.CommLib.Serial;
using BoonieBear.DeckUnit.ICore;
using BoonieBear.DeckUnit.BaseType;
using BoonieBear.DeckUnit.CommLib;
using TinyMetroWpfLibrary.Utility;
namespace BoonieBear.DeckUnit.LiveService
{
    public class CommService_BPADCP:ICommCore
    {
        private readonly static object SyncObject = new object();
        private static ICommCore _CommInstance;
        private ISerialService _BPserialService;
        private ISerialService _ADCPserialService;
        private CommConfInfo _commConf;
        private SerialPort _BPserialPort;
        private SerialPort _ADCPserialPort;
        private Observer<CustomEventArgs> _DataObserver;
        public string Error { get; set; }
        public bool IsInitialize { get; set; }
        public bool IsWorking { get; set; }
        public static ICommCore GetInstance(CommConfInfo conf, Observer<CustomEventArgs> observer)
        {
            lock (SyncObject)
            {

                return _CommInstance ?? (_CommInstance = new CommService_BPADCP(conf, observer));
            }
        }
        protected CommService_BPADCP(CommConfInfo conf, Observer<CustomEventArgs> observer)
        {
            _commConf = conf;
            _DataObserver = observer;
        }
		
		public ISerialService BPSerialService
        {
            get { return _BPserialService ?? (_BPserialService = (new ACNBPSerialServiceFactory()).CreateService()); }
        }

        public ISerialService ADCPSerialService
        {
            get { return _ADCPserialService ?? (_ADCPserialService = (new ACNADCPSerialServiceFactory()).CreateService()); }
        }

        public Observer<CustomEventArgs> CommDataObserver
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
            //可以加入其他的初始化工作 
            _BPserialPort = new SerialPort(_commConf.BPComm.Comm, _commConf.BPComm.Baud, (Parity)Enum.Parse(typeof(Parity), _commConf.BPComm.Parity.ToString()), _commConf.BPComm.DataBits, (StopBits)Enum.Parse(typeof(StopBits), _commConf.BPComm.StopBits.ToString()));
            _ADCPserialPort = new SerialPort(_commConf.ADCPComm.Comm, _commConf.ADCPComm.Baud, (Parity)Enum.Parse(typeof(Parity), _commConf.ADCPComm.Parity.ToString()), _commConf.ADCPComm.DataBits, (StopBits)Enum.Parse(typeof(StopBits), _commConf.ADCPComm.StopBits.ToString()));
            if (!BPSerialService.Init(_BPserialPort)) throw new Exception("BP端口打开失败");
            if (!ADCPSerialService.Init(_ADCPserialPort)) throw new Exception("ADCP端口打开失败");
            IsInitialize = true;
        }

        /// <summary>
        ///  初始化串口数据服务
        /// </summary>
        /// <param name="configure">通信参数</param>
        /// <returns>成功or失败</returns>
        private bool CreateSerialService(CommConfInfo configure)
        {
            if (_commConf == null || _DataObserver == null)
                throw new Exception("无法设置串口通信");

            if (!BPSerialService.Start()) return false;
            BPSerialService.Register(CommDataObserver);
            if (!ADCPSerialService.Start()) return false;
            ADCPSerialService.Register(CommDataObserver);
            return true;
        }
        public void Stop()
        {
            if (IsInitialize)
            {
                BPSerialService.UnRegister(CommDataObserver);
                BPSerialService.Stop();
                ADCPSerialService.UnRegister(CommDataObserver);
                ADCPSerialService.Stop();
            }
            IsInitialize = false;
        }

        public void Start()
        {
            IsWorking = false;
            if (_commConf == null || _DataObserver == null)
                throw new Exception("无法设置串口，请检查硬件连接并重启程序");
            if (!CreateSerialService(_commConf)) throw new Exception("串口服务无法初始化，请检查硬件连接并重启程序");
            IsWorking = true;
        }

		public void Sendbreak()//ADCP break命令
        {
            _ADCPserialPort.BreakState = true;//发出来的是全零
            Thread.Sleep(1000);
            _ADCPserialPort.BreakState = false;
        }

        public Task<bool> Sendcs()//ADCP CS命令
        {
            // TODO: Add your command handler code here
            string breakcmd = "cs\r\n";
            byte[] SendBuffer = Encoding.Default.GetBytes(breakcmd);
            var commfactory = new BPSerialCommHexCommandFactory(_ADCPserialPort);
            var cmd = commfactory.CreateSerialComm(SendBuffer);
            return Command.SendSerialAsync(cmd);
        }


        public Task<bool> SendConsoleCMD(byte[] buf,int Bpid)
        {
            //求命令校验和、写log文件、写命令到串口
            int i = 0;
            int checknum = 0;
            string scmd;
            while (buf[i] != 0x2A) //'*'
            {
                checknum += buf[i];
                i++;
            }
            checknum += '*';
            scmd = string.Format("${0:D2},{1:D2},", Bpid, i + 12);//重新计算包长
            byte[] Bcmd = Encoding.Default.GetBytes(scmd);
            for (i = 0; i < 7; i++)           //校验和是$与*之间的字符之和(包括$、*)
                checknum += Bcmd[i];
            byte[] SendBuffer = Encoding.Default.GetBytes(string.Format("{0:s}{1:s}{2:X4}\xd\xa", Encoding.Default.GetString(Bcmd), Encoding.Default.GetString(buf), checknum));
            var commfactory = new BPSerialCommHexCommandFactory(_BPserialPort);
            var cmd = commfactory.CreateSerialComm(SendBuffer);
            return Command.SendSerialAsync(cmd);
        }

        public Task<bool> SendLoaderCMD(string str)
        {
            var commfactory = new ACNSerialLoaderCommandFactory(_BPserialPort);
            var cmd = commfactory.CreateSerialComm(str);
            return Command.SendSerialAsync(cmd);

        }

        public async Task<bool> SendCMD(byte[] buf)
        {
            var commfactory = new ACNSerialCommHexCommandFactory(_BPserialPort);
            var cmd = commfactory.CreateSerialComm(buf);          
            return false;
        }

        public Task<bool> SendFile(Stream file)
        {
            var commfactory = new ACNSerialLoaderCommandFactory(_BPserialPort);

            using (StreamReader sr = new StreamReader(file))
            {
                string str = sr.ReadToEnd();
                var cmd = commfactory.CreateSerialComm(str);
                return Command.SendSerialAsync(cmd);
            }
        }


        public ISerialService SerialService
        {
            get { throw new NotImplementedException(); }
        }

        public Task<bool> SendConsoleCMD(string cmd)
        {
            throw new NotImplementedException();
        }
    }
}

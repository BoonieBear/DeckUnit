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
    public class CommService:ICommCore
    {
        private readonly static object SyncObject = new object();
        private static ICommCore _CommInstance;
        private ISerialService _serialService;
        private CommConfInfo _commConf;
        private SerialPort _serialPort;
        private Observer<CustomEventArgs> _DataObserver;
        public string Error { get; set; }
        public bool IsInitialize { get; set; }
        public bool IsWorking { get; set; }
        public static ICommCore GetInstance(CommConfInfo conf, Observer<CustomEventArgs> observer)
        {
            lock (SyncObject)
            {

                return _CommInstance ?? (_CommInstance = new CommService(conf, observer));
            }
        }
        protected CommService(CommConfInfo conf, Observer<CustomEventArgs> observer)
        {
            _commConf = conf;
            _DataObserver = observer;
        }
        public ISerialService SerialService
        {
            get { return _serialService ?? (_serialService = (new ACNSerialServiceFactory()).CreateService()); }
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
            IsInitialize = true;
        }

        /// <summary>
        ///  初始化串口数据服务
        /// </summary>
        /// <param name="configure">通信参数</param>
        /// <returns>成功or失败</returns>
        private bool CreateSerialService(CommConfInfo configure)
        {
            _serialPort = new SerialPort(configure.SerialPort);
            if (!SerialService.Init(_serialPort) || !SerialService.Start()) return false;
            SerialService.Register(CommDataObserver);
            return true;
        }
        public void Stop()
        {
            if (IsInitialize)
            {
                SerialService.UnRegister(CommDataObserver);
                SerialService.Stop();
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




        public Task<bool> SendConsoleCMD(string str)
        {
            var commfactory = new ACNSerialCommHexCommandFactory(_serialPort);
            var cmd = commfactory.CreateSerialComm(StringHexConverter.ConvertHexToChar(StringHexConverter.ConvertStrToHex(str)));
            return Command.SendSerialAsync(cmd);
        }

        public Task<bool> SendLoaderCMD(string str)
        {
            var commfactory = new ACNSerialLoaderCommandFactory(_serialPort);
            var cmd = commfactory.CreateSerialComm(str);
            return Command.SendSerialAsync(cmd);

        }

        public async Task<bool> SendCMD(byte[] buf)
        {
            var commfactory = new ACNSerialCommHexCommandFactory(_serialPort);
            var cmd = commfactory.CreateSerialComm(buf);
            SerialService.Register(cmd);
            var ret = Command.SendSerialAsync(cmd);
            await ret;
            SerialService.UnRegister(cmd);
            if (ret.Result)
            {
                SerialService.UnRegister(cmd);
                return true;
            }
            return false;
        }

        public Task<bool> SendFile(Stream file)
        {
            var commfactory = new ACNSerialLoaderCommandFactory(_serialPort);

            using (StreamReader sr = new StreamReader(file))
            {
                string str = sr.ReadToEnd();
                var cmd = commfactory.CreateSerialComm(str);
                return Command.SendSerialAsync(cmd);
            }
        }
    }
}

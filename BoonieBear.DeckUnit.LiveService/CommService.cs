using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.CommLib.Serial;
using BoonieBear.DeckUnit.ICore;
using BoonieBear.DeckUnit.BaseType;
using BoonieBear.DeckUnit.CommLib;
namespace BoonieBear.DeckUnit.LiveService
{
    public class CommService:ICommCore
    {
        private readonly static object SyncObject = new object();
        private static ICommCore _CommInstance;
        private ISerialService _serialService;
        private CommConfInfo _commConf;
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
            if (!SerialService.Init(new SerialPort(configure.SerialPort)) || !SerialService.Start()) return false;
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
                throw new Exception("无法设置内部端口");
            if (!CreateSerialService(_commConf)) throw new Exception("命令服务无法初始化");
            IsWorking = true;
        }

       
    }
}

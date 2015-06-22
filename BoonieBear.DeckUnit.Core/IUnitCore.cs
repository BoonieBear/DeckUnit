using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.DAL;
using TinyMetroWpfLibrary.EventAggregation;
using BoonieBear.DeckUnit.BaseType;
namespace BoonieBear.DeckUnit.ICore
{
    public interface ICore
    {
        void Initialize();
        void Stop();
        void Start();
        bool IsWorking { get; set; }
        bool IsInitialize { get; set; }
        string Error { get; set; }
    }

    public interface INetCore:ICore
    {
        
        //TCP客户端接收数据服务
        ITCPClientService TCPDataService { get; }
        //TCP客户端shell服务
        ITCPClientService TCPShellService { get; }
        //UDP接收数据服务
        IUDPService UDPService { get; }
        /// <summary>
        /// 数据观察类，主要负责数据的解析和保存
        /// </summary>
        Observer<CustomEventArgs> NetDataObserver { get; }

    }
    public interface ICommCore:ICore
    {
        
        //串口数据接收服务
        ISerialService SerialService { get; }
        /// <summary>
        /// 数据观察类，主要负责数据的解析和保存
        /// </summary>
        Observer<CustomEventArgs> CommDataObserver { get; }

    }
    public interface IFileCore : ICore
    {

        //文件服务
        //TBD
        /// <summary>
        /// 数据观察类，主要负责数据的解析和保存
        /// </summary>
        CommLib.Observer<CustomEventArgs> FileDataObserver { get; }

    }
}
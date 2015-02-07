using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.TinyMetro.WPF.EventAggregation;

namespace BoonieBear.DeckUnit.ICore
{
    public interface IUnitCore
    {
        //初始化
        bool Init();
        //功能停止，用于程序关闭或是基础设置重设
        void Dispose();
        //串口数据接收服务
        ISerialService SerialService { get; }
        //TCP客户端接收数据服务
        ITCPClientService TCPDataService { get; }
        //TCP客户端shell服务
        ITCPClientService TCPShellService { get; }
        //UDP接收数据服务
        IUDPService UDPService { get; }
        //数据库接口
        ISqlDAL SqlDAL { get; }
        /// <summary>
        /// 数据观察类，主要负责数据的解析和保存
        /// </summary>
        CommLib.IObserver<CustomEventArgs> DeckUnitObserver { get;  } 
        //当前是否有工作在进行
        bool IsWorking { get; set; }
        bool Initailed { get; set; }
        IEventAggregator EventAggregator {get; }
    }
}
using System.IO;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.DAL;
using TinyMetroWpfLibrary.EventAggregation;
using BoonieBear.DeckUnit.BaseType;
using System.Threading;
using System.Threading.Tasks;
namespace BoonieBear.DeckUnit.ICore
{
    public enum DownLoadFileType
    {
        Wave,
        RltUpdate,
        FixFirm,
        FloatM2,
        FloatM4,
        FPGA,
        BootLoader,

    }
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

        Task<bool> SendConsoleCMD(string cmd);

        Task <bool> SendCMD(byte[] buf);
        Task<bool> DownloadFile(Stream file,DownLoadFileType type);
        Task<bool> ResetMechan();//重启机芯
        Task<bool> BroadCast(byte[] buf);
        int SendBytes { get;}
        /// <summary>
        /// 数据观察类，主要负责数据的解析和保存
        /// </summary>
        Observer<CustomEventArgs> NetDataObserver { get; }

    }

    //add some apis to send specific id data:FH,FSK,PSK,OFDM,SSB and start special service
    public interface IMovNetCore : INetCore
    {
        bool StartUDPService();
        bool StartTCPService();
        void StopUDPService();
        void StopTCpService();
        bool Send(int id, byte[] buf);
        bool SendSSBNon();
        bool SendSSBEND(); 
        bool IsUDPWorking { get; }//
        bool IsTCPWorking { get; }
    }
    public interface ICommCore:ICore
    {
        ISerialService SerialService { get; }
        Task<bool> SendConsoleCMD(string cmd);
        //串口数据接收服务
        ISerialService BPSerialService { get; }
        ISerialService ADCPSerialService { get; }

        Task<bool> SendConsoleCMD(byte[] cmd,int Bpid);
        Task<bool> SendLoaderCMD(string cmd);
        Task<bool> SendCMD(byte[] buf);

        Task<bool> SendFile(Stream file);
        void Sendbreak();
        Task<bool> Sendcs();
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
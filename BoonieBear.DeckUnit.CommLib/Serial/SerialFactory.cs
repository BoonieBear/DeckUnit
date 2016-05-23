using System.IO.Ports;

namespace BoonieBear.DeckUnit.CommLib.Serial
{
    #region 命令工厂

    public class BPSerialCommHexCommandFactory
    {
        private SerialPort _serialPort;
        public BPSerialCommHexCommandFactory(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public SerialBaseComm CreateSerialComm(byte[] bytes)
        {
            return new BPSerialHexCommand(_serialPort, bytes);
        }

    }

    /// <summary>
    /// 命令模式下的串口通信命令工厂，生产ACN协议的命令子类
    /// </summary>
    public class ACNSerialCommHexCommandFactory 
    {
        private SerialPort _serialPort;
        public ACNSerialCommHexCommandFactory(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public SerialBaseComm CreateSerialComm(byte[] bytes)
        {
            return new ACNSerialHexCommand(_serialPort, bytes);
        }

    }
    /// <summary>
    /// 字符模式下的ACN串口通信命令工厂，返回loader模式下的字符命令子类和下载数据子类
    /// </summary>
    public class ACNSerialLoaderCommandFactory 
    {
        private SerialPort _serialPort;
        public ACNSerialLoaderCommandFactory(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public SerialBaseComm CreateSerialComm(string str)
        {
            return new ACNSerialLoaderCommand(_serialPort,str);
        }
    }
    #endregion
    #region 服务工厂

    public partial class ACNSerialServiceFactory : ISerialServiceFactory
    {
        public ISerialService CreateService()
        {
            return new ACNSerialSerialService();
        }
    }
	
	    public partial class ACNBPSerialServiceFactory : ISerialServiceFactory
    {
        public ISerialService CreateService()
        {
            return new ACNBPSerialSerialService();
        }
    }

    public partial class ACNADCPSerialServiceFactory : ISerialServiceFactory
    {
        public ISerialService CreateService()
        {
            return new ACNADCPSerialSerialService();
        }
    }
    #endregion
}

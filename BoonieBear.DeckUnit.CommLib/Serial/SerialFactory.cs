using System.IO.Ports;

namespace BoonieBear.DeckUnit.CommLib.Serial
{
    #region 命令工厂
    /// <summary>
    /// 命令模式下的串口通信命令工厂，生产ACN协议的命令子类
    /// </summary>
    public class ACNSerialCommHexCommandFactory : ISerialCommFactory
    {
        private SerialPort _serialPort;
        public ACNSerialCommHexCommandFactory(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public SerialBaseComm CreateSerialComm(ACNCommandMode mode, int id,byte[] bytes,string cmd)
        {
            return new ACNSerialHexCommand(_serialPort,mode, id, bytes);
        }

    }
    /// <summary>
    /// 字符模式下的ACN串口通信命令工厂，返回loader模式下的字符命令子类和下载数据子类
    /// </summary>
    public class ACNSerialLoaderCommandFactory : ISerialCommFactory
    {
        private SerialPort _serialPort;
        public ACNSerialLoaderCommandFactory(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public SerialBaseComm CreateSerialComm(ACNCommandMode mode, int id, byte[] bytes,string str)
        {
            return new ACNSerialLoaderCommand(_serialPort,mode,str,bytes);
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
    #endregion
}

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;

namespace BoonieBear.DeckUnit.CommLib
{
    //命令类接口，用在Command类中，使用各个命令类的实现完成各个命令的发送
    public interface IComm
    {
        bool Send();
    }
    #region 串口操作接口和工厂接口
    public interface ISerialComm
    {
        bool Init(SerialPort serialPort);
        void GetMsg(string str);
        void GetData(Byte[] bytes);
        void SendData();
        void SendMsg();
    }

    public interface ISerialFactory
    {
        ISerialComm CreateSerialComm();
    }
    #endregion


    #region TCP接口和工厂类
    public interface ITCPComm
    {
        bool Init(TcpClient tcpClient);
        void GetMsg(string str);
        void GetData(Byte[] bytes);
        bool Send();
        bool SendMsg();
    }

    #endregion  
}

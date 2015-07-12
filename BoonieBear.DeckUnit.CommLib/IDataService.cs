using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

namespace BoonieBear.DeckUnit.CommLib
{
    public interface ISerialService
    {
        bool Init(SerialPort serialPort);
        void Register(Observer<CustomEventArgs> observer);
        void UnRegister(Observer<CustomEventArgs> observer);
        bool Start();
        bool Stop();
        void ChangeMode(SerialServiceMode mode);
        void OnParsed(CustomEventArgs eventArgs);
        SerialPort ReturnSerialPort();
    }

    public interface Observer<in T>
    {
        void Handle(Object sender, T e);
    }

    public interface IUDPService
    {
        bool Init(UdpClient udpClient);
        void Register(Observer<CustomEventArgs> observer);
        void UnRegister(Observer<CustomEventArgs> observer);
        bool Start();
        void Stop();

        UdpClient ReturnUdpClient();
        
        void OnParsed(CustomEventArgs eventArgs);
    }

    public interface ITCPClientService
    {
        bool Init(TcpClient tcpClient,IPAddress ip,int destport);
        void Register(Observer<CustomEventArgs> observer);
        void UnRegister(Observer<CustomEventArgs> observer);
        void ConnectSync();
        bool Start();
        void Stop();
        //void RecvThread(object obj);
        //TcpClient ReturnTcpClient();
        //void OnParsed(CustomEventArgs eventArgs);

        bool Connected
        {
            get;

        }
    }
}

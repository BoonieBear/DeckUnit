using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO.Ports;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using BoonieBear.DeckUnit.CommLib.Serial;

namespace BoonieBear.DeckUnit.CommLib
{
    public interface ISerialService
    {
        bool Init(SerialPort serialPort);
        void Register(IObserver<CustomEventArgs> observer);
        void UnRegister(IObserver<CustomEventArgs> observer);
        bool Start();
        bool Stop();
        SerialPort ReturnSerialPort();
        void ChangeMode(SerialServiceMode mode);
        void OnParsed(CustomEventArgs eventArgs);
    }

    public interface IObserver<in T>
    {
        void Handle(Object sender, T e);
    }

    public interface IUDPService
    {
        bool Init(UdpClient udpClient);
        void Register(IObserver<CustomEventArgs> observer);
        void UnRegister(IObserver<CustomEventArgs> observer);
        bool Start();
        void Stop();

        UdpClient ReturnUdpClient();
        
        void OnParsed(CustomEventArgs eventArgs);
    }

    public interface ITCPClientService
    {
        bool Init(TcpClient tcpClient,IPAddress ip,int destport);
        void Register(IObserver<CustomEventArgs> observer);
        void UnRegister(IObserver<CustomEventArgs> observer);
        void ConnectSync();
        bool Start();
        void Stop();
        void RecvThread(object obj);
        TcpClient ReturnTcpClient();
        void OnParsed(CustomEventArgs eventArgs);

        bool Connected
        {
            get;

        }
    }
}

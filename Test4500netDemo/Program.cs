
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.CommLib.TCP;
using BoonieBear.DeckUnit.ACMP;
namespace Test4500netDemo
{
    static class Program
    {
        
        static void Main(string[] args)
        {
            var auto = new AutoResetEvent(false);
            ACM4500Protocol.Init();
            var acmoObserver = new ACMDataObserver();
            var datatcpClient = new TcpClient();
            datatcpClient.SendTimeout = 1000;
            var service = new TCPDataServiceFactory();
            var dataservice = service.CreateService(); 

            Console.WriteLine("Begin Connect to Server……");
            var ret = TCPconnect(dataservice, datatcpClient);
            if (ret)
                if(dataservice.Start())
                {
                    Console.WriteLine("Start Listen to Server……");
                    dataservice.Register(acmoObserver);
                    var bytes = new byte[135];
                    var head = 0x4500;
                    var b = BitConverter.GetBytes(head);
                    Buffer.BlockCopy(b,0,bytes,0,2);
                    var length = BitConverter.GetBytes(133);
                    Buffer.BlockCopy(length, 0, bytes, 2, 2);
                    var cmd = new ACNTCPDataCommand(datatcpClient, bytes);
                    Console.WriteLine("Press key to send a test package");
                    Console.ReadKey();

                    Command.SendTCPAsync(cmd);
                    Console.WriteLine("Test package Sended");
                    Console.WriteLine("Waiting……Press a Key to Exit Test");
                    Console.ReadKey();
                    dataservice.UnRegister(acmoObserver);
                }
            Console.WriteLine("Test Ended ");
            Console.ReadKey();
        }

        static bool TCPconnect(ITCPClientService dataservice,TcpClient datatcpClient)
        {
            if (dataservice.Init(datatcpClient, IPAddress.Parse("192.168.2.222"), 8081))
            {
                dataservice.ConnectSync();
                Console.WriteLine("Connect " + (dataservice.Connected ? "OK" : "Failed"));

            }
            else
            {
                Console.WriteLine("Can not Initial Socket");
            }
            if (dataservice.Connected)
                return true;
            return false;
        }
    }
}

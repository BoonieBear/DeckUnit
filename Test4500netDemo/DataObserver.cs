using System;
using System.Net.Sockets;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.CommLib.TCP;
using BoonieBear.DeckUnit.ACMP;
namespace Test4500netDemo
{
    public class ACMDataObserver : BoonieBear.DeckUnit.CommLib.IObserver<CustomEventArgs>
    {
        public void Handle(object sender, CustomEventArgs e)
        {
            if (e.ParseOK && (e.Mode ==CallMode.DataMode))
            {
                ACM4500Protocol.GetBytes(e.DataBuffer);
                var id = ACM4500Protocol.Parse();
                if (id == 0x45FF)
                {
                    Console.WriteLine("Receive 0x45FF");
                    var bytes = new byte[135];
                    var head = 0x4500;
                    var b = BitConverter.GetBytes(head);
                    Buffer.BlockCopy(b, 0, bytes, 0, 2);
                    var length = BitConverter.GetBytes(133);
                    Buffer.BlockCopy(length, 0, bytes, 2, 2);
                    var cmd = new ACNTCPDataCommand((TcpClient) e.CallSrc, bytes);
                    Command.SendTCPAsync(cmd);
                    Console.WriteLine("Send Test Package");
                }
            }
        }
    }
}

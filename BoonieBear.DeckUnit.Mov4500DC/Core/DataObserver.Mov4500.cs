using System;
using System.Net.Sockets;
using BoonieBear.DeckUnit.ACMP;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.Mov4500UI.Events;

namespace BoonieBear.DeckUnit.Mov4500UI.Core
{
    public class Mov4500DataObserver:Observer<CustomEventArgs>
    {
        public void Handle(object sender, CustomEventArgs e)
        {
            if (e.ParseOK)
            {
                if (e.Mode == CallMode.Sail)//水下航控或ADCP或BP
                {
                    var src = e.CallSrc as UdpClient;
                    if (src != null)//udp转发，没有改动数据结构
                    {
                        var id =BitConverter.ToInt16(e.DataBuffer, 0);
                        switch (id)
                        {
                            case 0x1001:
                                break;
                        }
                    }
 
                }
                else if (e.Mode == CallMode.USBL)
                {
                 
                }
            }
            
        }

        private void ParseData(byte[] bytes)
        {
            UnitCore.Instance.ACMMutex.WaitOne();
            try
            {
                ACM4500Protocol.GetBytes(bytes);
                ACM4500Protocol.Parse();
                UnitCore.Instance.ACMMutex.ReleaseMutex();
            }
            catch (Exception ex)
            {
                UnitCore.Instance.ACMMutex.ReleaseMutex();
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(ex.Message,ex,LogType.Error));
            }
        }
    }
}

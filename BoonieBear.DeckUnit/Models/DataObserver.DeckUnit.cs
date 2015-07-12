using System;
using System.IO.Ports;
using System.Net.Sockets;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.ACNP;
using BoonieBear.DeckUnit.Core;
using Newtonsoft.Json;
using BoonieBear.DeckUnit.Events;
using BoonieBear.DeckUnit.JsonUtils;
using System.Diagnostics;
namespace BoonieBear.DeckUnit.Models
{
    public class DeckUnitDataObserver:Observer<CustomEventArgs>
    {
        private string str;
        public void Handle(object sender, CustomEventArgs e)
        {
            if (e.ParseOK)
            {
                if (e.Mode == CallMode.NoneMode)
                {
                    str = e.Outstring;
                    var tcpsrc = e.CallSrc as TcpClient;
                    if (tcpsrc != null) //网络shell
                        UnitCore.Instance.UnitTraceService.WriteShell(str);
                     var udpsrc = e.CallSrc as UdpClient;
                     if (udpsrc != null) //trace
                        UnitCore.Instance.UnitTraceService.WriteTrace(str);
                }
                if (e.Mode == CallMode.DataMode)
                {
                    var bytes = new byte[e.DataBufferLength];
                    Buffer.BlockCopy(e.DataBuffer, 0, bytes, 0, e.DataBufferLength);
                    //UnitCore.Instance.UnitTraceService.Save(bytes);//TBD
                    ACNProtocol.GetDataForParse(bytes);
                    try
                    {
                        if (ACNProtocol.Parse())
                        {
                            var nodetree = StringListToTree.TransListToNodeWriteLineic(ACNProtocol.parselist);
                            var newtree = StringListToTree.RemoveFatherPointer(nodetree);
                            var jsonstr = JsonConvert.SerializeObject(newtree);
                            str = jsonstr;

                        }
                    }
                    catch (Exception ex)
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent( ex, LogType.Error));
                    }

                }
            }
            else
            {
                Debug.WriteLine(e.ErrorMsg);
            }
        }
    }
}

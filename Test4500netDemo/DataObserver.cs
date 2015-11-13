using System;
using System.Net.Sockets;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.CommLib.TCP;
using BoonieBear.DeckUnit.ACMP;
namespace Test4500netDemo
{
    public class ACMDataObserver : Observer<CustomEventArgs>
    {
        public void Handle(object sender, CustomEventArgs e)
        {
            if (e.ParseOK && (e.Mode ==CallMode.DataMode))
            {
                ACM4500Protocol.GetBytes(e.DataBuffer);
                var ret = ACM4500Protocol.Parse();
                
            }
        }
    }
}

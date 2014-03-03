using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace BoonieBear.DeckUnit.CommLib.ACNProtocol
{
    public class ACNSerialHexCommand : SerialBaseComm,IComm
    {

        public override void GetMsg(string str)
        {
            throw new NotImplementedException();
        }

        public bool Send()
        {
            throw new NotImplementedException();
        }

        public bool SendString()
        {
            throw new NotImplementedException();
        }

        public void SendMsg()
        {
            throw new NotImplementedException();
        }
    }
}

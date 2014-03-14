using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoonieBear.DeckUnit.CommLib.UDP
{
    #region UDP命令工厂接口和服务工厂接口


    public interface IUDPCommFactory
    {
        UDPBaseComm CreateSerialComm(ACNCommandMode mode, int id, byte[] bytes,string str);
    }

    public interface IUDPServiceFactory
    {
        IUDPService CreateService();
    }
    #endregion
}

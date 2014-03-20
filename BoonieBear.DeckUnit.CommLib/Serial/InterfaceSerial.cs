using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BoonieBear.DeckUnit.CommLib.Serial
{
    #region 串口命令工厂接口和服务工厂接口

    public interface ISerialCommFactory
    {
        SerialBaseComm CreateSerialComm(ACNCommandMode mode,int id, byte[] bytes,string str);
    }

    public interface ISerialServiceFactory
    {
        ISerialService CreateService();
    }
    #endregion
}

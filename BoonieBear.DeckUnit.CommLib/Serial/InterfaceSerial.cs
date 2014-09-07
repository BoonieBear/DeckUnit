using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BoonieBear.DeckUnit.CommLib.Serial
{
    #region 串口服务工厂接口

    public interface ISerialServiceFactory
    {
        ISerialService CreateService();
    }
    #endregion
}

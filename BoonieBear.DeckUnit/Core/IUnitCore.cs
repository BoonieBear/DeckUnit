using BoonieBear.DeckUnit.CommLib;

namespace BoonieBear.DeckUnit.Core
{
    public interface IUnitCore
    {
        ISerialService SerialService { get; }
        ITCPClientService TCPClientService { get; }
        IUDPService UDPService { get; }
    }
}
using BoonieBear.DeckUnit.CommLib;

namespace BoonieBear.DeckUnit.Core
{
    /// <summary>
    /// 核心业务类，包括命令，通信服务，工作逻辑
    /// </summary>
    class UnitCore : IUnitCore
    {
        private static IUnitCore Instance;
        public ISerialService SerialService { get; private set; }
        public ITCPClientService TCPClientService { get; private set; }
        public IUDPService UDPService { get; private set; }
    }
}

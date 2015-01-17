namespace BoonieBear.DeckUnit.CommLib.UDP
{
    #region UDP命令工厂接口和服务工厂接口


    public interface IUDPCommFactory
    {
        UDPBaseComm CreateUDPComm(ACNCommandMode mode, byte[] bytes,string str);
    }

    public interface IUDPServiceFactory
    {
        IUDPService CreateService();
    }
    #endregion
}

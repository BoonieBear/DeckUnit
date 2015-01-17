namespace BoonieBear.DeckUnit.CommLib.Serial
{
    #region 串口服务工厂接口

    public interface ISerialServiceFactory
    {
        ISerialService CreateService();
    }
    #endregion
}

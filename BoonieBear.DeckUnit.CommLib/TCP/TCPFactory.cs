namespace BoonieBear.DeckUnit.CommLib.TCP
{
    
    /// <summary>
    /// TCP Shell服务
    /// </summary>
    public class TCPShellServiceFactory : ITCPServiceFactory
    {
        public ITCPClientService CreateService()
        {
            return new ACNTCPShellService();
        }
    }
    /// <summary>
    /// TCP Shell服务
    /// </summary>
    public class TCPDataServiceFactory : ITCPServiceFactory
    {
        public ITCPClientService CreateService()
        {
            return new ACNTCPDataService();
        }
    }
}

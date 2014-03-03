namespace BoonieBear.DeckUnit.CommLib.ACNProtocol
{
    public class ACNSerialHexCommandFactory : ISerialFactory
    {
        public ISerialComm CreateSerialComm()
        {
            return new ACNSerialHexCommand();
        }
    }
}

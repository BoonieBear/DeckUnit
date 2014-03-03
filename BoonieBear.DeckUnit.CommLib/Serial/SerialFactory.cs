namespace BoonieBear.DeckUnit.CommLib.Serial
{
    public class ACNSerialHexCommandFactory : ISerialFactory
    {
        public ISerialComm CreateSerialComm()
        {
            return new ACNSerialHexCommand();
        }
    }
}

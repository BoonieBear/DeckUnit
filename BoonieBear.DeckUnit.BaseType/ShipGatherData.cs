namespace BoonieBear.DeckUnit.MovDataManage
{
    public class ShipGatherData
    {
        private readonly static object Syncobject = new object();
        private static ShipGatherData _shipGatherData;
        private ShipGatherData() { }
        
        public static ShipGatherData GetInstance()
        {
            if (_shipGatherData == null)
            {
                lock (Syncobject)
                {
                    _shipGatherData = new ShipGatherData();
                }
                return _shipGatherData;
            }
            return _shipGatherData;
        }

    }
}

using System;

namespace BoonieBear.DeckUnit.ACMP
{
    public class ShipGatherData
    {
        private readonly static object SyncObject = new object();
        private static ShipGatherData _shipGatherData;
        private int SubPostNum = 5;
        private SubLatest5Post _subLatest5Post;
        private byte[] _packageBytes = new byte[MovGlobalVariables.ShipMFSKSize];

        private ShipGatherData()
        {
            _subLatest5Post = new SubLatest5Post();
        }
        
        public static ShipGatherData GetInstance()
        {
            lock (SyncObject)
            {
                return _shipGatherData ?? (_shipGatherData = new ShipGatherData());
            }
        }
        public void Add(object obj, Mov4500Type mType)
        {
            switch (mType)
            {
                case Mov4500Type.ALLPOST:
                    break;
                case Mov4500Type.WORD:
                    break;
                default:
                    throw new Exception("undefined data type!");
            }
        }
    }
}

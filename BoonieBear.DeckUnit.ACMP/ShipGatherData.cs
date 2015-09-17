using System;

namespace BoonieBear.DeckUnit.ACMP
{
    /// <summary>
    /// 用来管理母船各种数据的收集，打包，解析
    /// </summary>
    public class ShipGatherData
    {
        private readonly static object SyncObject = new object();
        private static ShipGatherData _shipGatherData;
        private int SubPostNum = 5;
        private SubLatestPost _subLatestPost;
        private Sysposition _sysposition;//最后一个位置信息,用来显示
        private string _msg;
        private byte[] _packageBytes = new byte[MovGlobalVariables.ShipMFSKSize];

        private ShipGatherData()
        {
            _subLatestPost = new SubLatestPost(SubPostNum);
            _sysposition=new Sysposition();
        }

        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        public Sysposition Sysposition
        {
            get { return _sysposition; }
            set { _sysposition = value; }
        }

        public static ShipGatherData GetInstance()
        {
            lock (SyncObject)
            {
                return _shipGatherData ?? (_shipGatherData = new ShipGatherData());
            }
        }
        public void Add(object obj, Mov4500DataType mDataType)
        {
            switch (mDataType)
            {
                
                default:
                    throw new Exception("undefined data type!");
            }
        }
    }
}

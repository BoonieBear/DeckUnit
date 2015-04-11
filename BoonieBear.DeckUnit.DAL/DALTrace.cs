using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.DAL.DBModel;
namespace BoonieBear.DeckUnit.DAL
{
    class DALTrace
    {
        private readonly static object SyncObject = new object();
        private static DALTrace _dalTrace;
        private ISqlDAL _sqldal;
        private string connectstring = @"Data Source=default.dudb;Pooling=True";
        public string Errormsg { get; set; }
        public static DALTrace GetInstance()
        {
            lock (SyncObject)
            {
                return _dalTrace ?? (_dalTrace = new DALTrace());
            }
        }

        private bool CreateDataPath()
        {
            
        }
        protected DALTrace(DBType dbType)
        {
            try
            {
                if (Directory.Exists(@"") != true)
                {
                    
                }
                _sqldal = DALFactory.CreateDAL(dbType);
                if (_sqldal.LinkStatus)
                {
                    
                }
            }
            catch (Exception e)
            {
                Errormsg = e.Message;
            }
        }
 
    }
}

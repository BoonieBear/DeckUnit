using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoonieBear.DeckUnit.DataStorageService
{
    class UnitTraceService:ITraceService
    {
        private static ITraceService Instance;
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public bool SetupService()
        {
            throw new NotImplementedException();
        }

        public bool TearDownService()
        {
            throw new NotImplementedException();
        }

        public long Save(string sType, object bTraceBytes)
        {
            throw new NotImplementedException();
        }
    }
}

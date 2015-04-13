using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.TraceFileService;
using System.IO;
namespace BoonieBear.DeckUnit.DataStorageService
{
    public class UnitTraceService
    {
        private DALTrace _dalTrace;//dal
        private ADFile _adFile;
        public string Error { get; set; }
        public string LogPath { get; set; }

        public string FileName
        {
            get
            {
                if (_adFile.FileLen>0)
                {
                    return _adFile.FileName;
                }
                return null;
            }
        }

        public bool CreateService()
        {

            //create log directory
            string logPathDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            LogPath = Environment.CurrentDirectory + @"\Log\" + logPathDate;
            Directory.CreateDirectory(LogPath);
            var debugPath = new DirectoryInfo(LogPath);
            _adFile = new ADFile("LOG", ".dat");
            _adFile.SetPath(debugPath);

            //setup sql
            _dalTrace = DALTrace.GetInstance();
            if(_dalTrace.isLink)
                return true;
            Error = _dalTrace.Errormsg;
            return false;
        }

        public bool TearDownService()
        {
            bool isOK = true;
            try
            {
                _dalTrace.CloseDAL();
                _adFile.Close();
            }
            catch (Exception e)
            {
                Error = e.Message;
                isOK = false;
            }

            return isOK;
        }

        public long Save(string sType, object bTraceBytes)
        {
            throw new NotImplementedException();
        }
    }
}

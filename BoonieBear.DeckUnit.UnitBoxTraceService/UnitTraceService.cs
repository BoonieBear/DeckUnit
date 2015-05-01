using System;
using System.IO;
using BoonieBear.DeckUnit.TraceFileService;
using BoonieBear.DeckUnit.DAL;
namespace BoonieBear.DeckUnit.UnitBoxTraceService
{
    public class UnitTraceService
    {
        private DALTrace _dalTrace;//dal
        private ADFile _adFile;//trace file
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

        public bool CreateService(string connectstr="")
        {

            //create log directory
            string logPathDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            LogPath = Environment.CurrentDirectory + @"\Log\" + logPathDate;
            Directory.CreateDirectory(LogPath);
            var debugPath = new DirectoryInfo(LogPath);
            _adFile = new ADFile("LOG", ".dat");
            _adFile.SetPath(debugPath);

            //setup sql
            _dalTrace = DALTrace.GetInstance(connectstr);
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

        public bool Save(CommandLog log, byte[] bytes)
        {
            if (_adFile.Create())
            {
                if (_adFile.Write(bytes)>0)
                {
                    _adFile.Close();
                    log.FilePath = FileName;
                    if (_dalTrace.isLink)
                        return _dalTrace.SaveCommLog(log);
                    Error = _dalTrace.Errormsg;
                }
                else
                {
                    Error = "写文件失败！";
                }
            }
            else
            {
                Error = "创建文件失败！";
            }
            return false;
            
        }
    }
}

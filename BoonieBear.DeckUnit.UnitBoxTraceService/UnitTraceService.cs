﻿using System;
using System.IO;
using BoonieBear.DeckUnit.TraceFileService;
using BoonieBear.DeckUnit.DAL;
namespace BoonieBear.DeckUnit.UnitBoxTraceService
{
    public class UnitTraceService
    {
        private DALTrace _dalTrace;//dal
        private ADFile _adFile;//trace file
        private LogFile traceFile;
        private LogFile shelLogFile;
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
            string datapath = LogPath + @"\Data";
            Directory.CreateDirectory(datapath);
            var debugPath = new DirectoryInfo(datapath);
            _adFile = new ADFile("Data", ".dat");
            _adFile.SetPath(debugPath);
            string logpath = LogPath + @"\Record";
            var recordPath = new DirectoryInfo(logpath);
            shelLogFile = new LogFile("Shell", ".txt");
            shelLogFile.SetPath(recordPath);

            traceFile = new LogFile("Trace", ".txt");
            traceFile.SetPath(recordPath);
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
                traceFile.Close();
                shelLogFile.Close();
            }
            catch (Exception e)
            {
                Error = e.Message;
                isOK = false;
            }

            return isOK;
        }

        public bool WriteShell(string str)
        {
            if (shelLogFile.Create())
            {
                if (shelLogFile.Write(str) > 0)
                {
                    Error = "";
                }
                else
                {
                    Error = "写Shell文件失败！";
                }
            }
            else
            {
                Error = "创建Shell文件失败！";
            }
            return false;
        }

        public bool WriteTrace(string str)
        {
            if (traceFile.Create())
            {
                if (traceFile.Write(str) > 0)
                {
                    Error = "";
                }
                else
                {
                    Error = "写Trace文件失败！";
                }
            }
            else
            {
                Error = "创建Trace文件失败！";
            }
            return false;
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
                    {
                        if (_dalTrace.SaveCommLog(log))
                        {
                            Error = "";
                            return true;
                        }
                        else
                        {
                            Error = _dalTrace.Errormsg;
                        }
                            
                    }
                    else
                        Error = "未连接数据库";
                }
                else
                {
                    Error = "写Data文件失败！";
                }
            }
            else
            {
                Error = "创建Data文件失败！";
            }
            return false;
            
        }
    }
}

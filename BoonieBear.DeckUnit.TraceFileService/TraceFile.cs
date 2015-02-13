using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BoonieBear.DeckUnit.TraceFileService
{
    public class TraceFile
    {
        private readonly static object SyncObject = new object();
        private static TraceFile _traceFile;

        private string _logPathDate;
        private DirectoryInfo _debugPath;
        private DirectoryInfo _serialDataPath;
        private DirectoryInfo _serialCmdPath;
        private DirectoryInfo _netCmdPath;
        private DirectoryInfo _netDataPath;
        private DirectoryInfo _adDataPath;
        private DirectoryInfo _wavePath;

        public string Errormsg { get; set; }
        public ADFile Ch1AdFile = new ADFile("Ch1", "ad");
        public ADFile Ch2AdFile = new ADFile("Ch2", "ad");
        public ADFile Ch3AdFile = new ADFile("Ch3", "ad");
        public ADFile Ch4AdFile = new ADFile("Ch4", "ad");
        public ADFile WaveFile = new ADFile("Voice", "wv");
        public ADFile NetDataFile = new ADFile("NetRecvData", "NRD");
        public ADFile NetCmdFile = new ADFile("NetCmdData", "NCD");
        public ADFile ComDataFile = new ADFile("ComRecvData", "CRD");
        public ADFile ComCmdFile = new ADFile("ComCmdData", "CCD");
        public LogFile DebugFile = new LogFile("Debug", "dbg");

        public static TraceFile GetInstance()
        {
            lock (SyncObject)
            {
                return _traceFile ?? (_traceFile = new TraceFile());
            }
        }
        public bool Initialize()
        {
            bool bInitail;
            try
            {
                if (!Directory.Exists(@".\Log"))
                    Directory.CreateDirectory(@".\Log");

                _logPathDate = DateTime.Now.Date.ToString("yyyy MM dd");
                _logPathDate = @".\Log\" + _logPathDate;

                Directory.CreateDirectory(_logPathDate + @"\Debug");
                _debugPath = new DirectoryInfo(_logPathDate + @"\Debug");
                DebugFile.SetPath(_debugPath);

                Directory.CreateDirectory(_logPathDate + @"\SerialData");
                _serialDataPath = new DirectoryInfo(_logPathDate + @"\SerialData");
                ComDataFile.SetPath(_serialDataPath);

                Directory.CreateDirectory(_logPathDate + @"\SerialCmd");
                _serialCmdPath = new DirectoryInfo(_logPathDate + @"\SerialCmd");
                ComCmdFile.SetPath(_serialCmdPath);

                Directory.CreateDirectory(_logPathDate + @"\NetCmd");
                _netCmdPath = new DirectoryInfo(_logPathDate + @"\NetCmd");
                NetCmdFile.SetPath(_netCmdPath);

                Directory.CreateDirectory(_logPathDate + @"\NetData");
                _netDataPath = new DirectoryInfo(_logPathDate + @"\NetData");
                NetDataFile.SetPath(_netCmdPath);

                Directory.CreateDirectory(_logPathDate + @"\ADData");
                _adDataPath = new DirectoryInfo(_logPathDate + @"\ADData");
                Ch1AdFile.SetPath(_adDataPath);
                Ch2AdFile.SetPath(_adDataPath);
                Ch3AdFile.SetPath(_adDataPath);
                Ch4AdFile.SetPath(_adDataPath);

                Directory.CreateDirectory(_logPathDate + @"\WaveData");
                _wavePath = new DirectoryInfo(_logPathDate + @"\WaveData");
                WaveFile.SetPath(_wavePath);

                bInitail = true;
                Errormsg = null;
            }
            catch (Exception e)
            {
                Errormsg = e.Message;
                bInitail = false;
            }
            return bInitail;
        }
    }
}
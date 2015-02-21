using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BoonieBear.DeckUnit.TraceFileService
{
    public enum TraceType
    {
        Binary = 0,
        String = 1,
    }
    public class TraceFile
    {
        private readonly static object SyncObject = new object();
        private static TraceFile _traceFile;
 
        private string _logPathDate;
        

        public string Errormsg { get; set; }
        
        private Hashtable _BinaryTable = new Hashtable();//二进制trace file
        private Hashtable _StringTable = new Hashtable();//字符串型trace file

       
        public static TraceFile GetInstance()
        {
            lock (SyncObject)
            {
                return _traceFile ?? (_traceFile = new TraceFile());
            }
        }

        protected TraceFile()
        {
            try
            {
                if (!Directory.Exists(@".\Log"))
                    Directory.CreateDirectory(@".\Log");
            }
            catch (Exception e)
            {
                Errormsg = e.Message;
            }
        }

        public bool Close()
        {
            bool isOk = true;
            try
            {
                var nameList = new string[_BinaryTable.Count];
                _BinaryTable.Keys.CopyTo(nameList, 0);
                var it = nameList.GetEnumerator();
                if (it != null)
                {

                    while (it.MoveNext())
                    {
                        Remove(it.Current.ToString());
                    }
                }

                nameList = new string[_StringTable.Count];
                _StringTable.Keys.CopyTo(nameList, 0);
                it = nameList.GetEnumerator();
                if (it != null)
                {

                    while (it.MoveNext())
                    {
                        Remove(it.Current.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                if (Errormsg == null)
                    Errormsg = e.Message;
                isOk = false;
            }
            return isOk;

        }

        public void Remove(string name)
        {
            try
            {
                if (_StringTable.ContainsKey(name))
                {
                    ((csFile) _StringTable[name]).Close();
                    _StringTable.Remove(name);
                }
                else if (_BinaryTable.ContainsKey(name))
                {
                    ((csFile) _BinaryTable[name]).Close();
                    _BinaryTable.Remove(name);
                }
                else
                {
                    Errormsg = "no such file!";
                }
            }
            catch (Exception e)
            {
                Errormsg = e.Message;
                throw e;
            }
            
        }
        public bool CreateFile(string keyName,TraceType tType, string header = "Log", string ext = "dat",string path = @"\Debug")
        {
            bool isOk = true;
            try
            {
                if (_StringTable.ContainsKey(keyName) || _BinaryTable.ContainsKey(keyName))
                {
                    Errormsg = @"replicate trace file name";
                    isOk = false;
                }
                //create log directory
                _logPathDate = DateTime.Now.Date.ToString("yyyy MM dd");
                _logPathDate = @".\Log\" + _logPathDate;
                Directory.CreateDirectory(_logPathDate + path);
                var debugPath = new DirectoryInfo(_logPathDate + path);
                //create tracefile
                switch (tType)
                {
                    case TraceType.Binary:
                        var newTrace = new ADFile(header, ext);
                        newTrace.SetPath(debugPath);
                        _BinaryTable.Add(keyName,newTrace);
                        break;
                    case TraceType.String:
                        var logTrace = new LogFile(header, ext);
                        logTrace.SetPath(debugPath);
                        _StringTable.Add(keyName,logTrace);
                        break;
                    default:
                        isOk = false;
                        Errormsg = "undefine trace type!";
                        break;
                }
            }
            catch (Exception e)
            {
                Errormsg = e.Message;
                isOk = false;
            }
            return isOk;
        }

        public LogFile GetStringTrace(string keyName)
        {
            if (_StringTable.ContainsKey(keyName))
            {
                return (LogFile) _StringTable[keyName];
            }
            return null;
        }
        public ADFile GetAdTrace(string keyName)
        {
            if (_BinaryTable.ContainsKey(keyName))
            {
                return (ADFile)_BinaryTable[keyName];
            }
            return null;
        }
    }
}
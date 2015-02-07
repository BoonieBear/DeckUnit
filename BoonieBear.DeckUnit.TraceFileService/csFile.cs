using System;
using System.Globalization;
using System.IO;

namespace BoonieBear.DeckUnit.TraceFileService
{
	/// <summary>
	/// 文件读取存储类，保存原始数据，记录类使用该类
	/// </summary>
	public class csFile
	{
        public string fileName;
        public StreamReader ts;
        public StreamWriter ws;
        public BinaryReader br;
        public BinaryWriter bw;
        public bool opened, writeOpened;
		//-----------
		public csFile() {
			init();
		}
		//-----------
		private void init() {
			opened = false;
			writeOpened = false;
		}
		//-----------
		public csFile(string file_name) 	{
			fileName = file_name;
			init();
		}
		//-----------
		public bool OpenForRead(string file_name){
			fileName = file_name;
			try {
				ts = new StreamReader (fileName);
				opened=true;
			}
			catch(FileNotFoundException e) {
				return false;
			}
			return true;
		}
		//-----------
		public bool OpenForRead() {
			return OpenForRead(fileName);
		}
		//-----------
		public string readLine() {
			return ts.ReadLine ();
		}
		//-----------
        public virtual void writeLine(string s)
        {
			ws.WriteLine (s);
		}
		//-----------
        public virtual void close()
        {
            writeOpened = false;
            if (ts!=null)
				ts.Close();
            if (ws!=null)
				ws.Close();
            if (br != null)
                br.Close();
            if (bw != null)
                bw.Close();
		}
		//-----------
		public bool OpenForWrite() {
			return OpenForWrite(fileName);
		}
		//-----------
		public bool OpenForWrite(string file_name) {
			try{
				ws = new StreamWriter (file_name);
                ws.AutoFlush = true;
				fileName = file_name;
				writeOpened = true;
				return true;
			}
			catch(FileNotFoundException e) {
				return false;
			}
		}
        //-----------
        public bool BinaryOpenWrite()
        {
            return BinaryOpenWrite(fileName);
        }
        //-----------
        public bool BinaryOpenWrite(string file_name)
        {
            try
            {
                bw = new BinaryWriter(File.Open(file_name, FileMode.OpenOrCreate));
                fileName = file_name;
                writeOpened = true;
                return true;
            }
            catch (FileNotFoundException e)
            {
                return false;
            }
        }
        //-----------
        public bool BinaryOpenRead()
        {
            return BinaryOpenRead(fileName);
        }
        public bool BinaryOpenRead(string file_name)
        {
            try
            {
                br = new BinaryReader(File.Open(file_name, FileMode.Open));
                fileName = file_name;
                writeOpened = true;
                return true;
            }
            catch (FileNotFoundException e)
            {
                return false;
            }
        }
        public virtual void BinaryWrite(byte[] data)
        {
            this.bw.Write(data);
        }
	}

    public class LogFile:csFile
    {
        public csFile logfile;
        private string ext;
        public long length; 
        public LogFile(string extstr)
        {
            ext = extstr;
            logfile = new csFile();
            length = 0;
            
        }
        public bool OpenFile(DirectoryInfo di)
        {
            if(di.Exists)
            {
                string timestring = DateTime.Now.Year.ToString("0000_", CultureInfo.InvariantCulture) + DateTime.Now.Month.ToString("00_", CultureInfo.InvariantCulture) + DateTime.Now.Day.ToString("00_", CultureInfo.InvariantCulture) + DateTime.Now.Hour.ToString("00_", CultureInfo.InvariantCulture)
                    + DateTime.Now.Minute.ToString("00_", CultureInfo.InvariantCulture) + DateTime.Now.Second.ToString("00", CultureInfo.InvariantCulture);
                string filename = di.FullName;//文件路径
                filename = filename + "\\" + ext + timestring + ".txt";

                return logfile.OpenForWrite(filename);
            }
            else
                return false;
        }
        public override void writeLine(string s)
        {
            string timestring = "("+DateTime.Now.Month.ToString("00", CultureInfo.InvariantCulture) +"/"+ DateTime.Now.Day.ToString("00", CultureInfo.InvariantCulture) +" "+ DateTime.Now.Hour.ToString("00", CultureInfo.InvariantCulture)
                    + ":" + DateTime.Now.Minute.ToString("00", CultureInfo.InvariantCulture) + ":" + DateTime.Now.Second.ToString("00", CultureInfo.InvariantCulture)+")";
            s = timestring + s;
            logfile.ws.WriteLine(s);
            
            length += s.Length;
            length += 2;
        }

        public override void close()
        {
            logfile.close();
            length = 0;
            if (logfile.ts != null)
                logfile.ts.Close();
            if (logfile.ws != null)
                logfile.ws.Close();
            if (logfile.br != null)
                logfile.br.Close();
            if (logfile.bw != null)
                logfile.bw.Close();
        }
        
    }

    public class AdFile : csFile
    {
        public csFile adfile;
        private long length;
        private string ext;
        public AdFile(string extstr)
        {
            ext = extstr;
            length = 0;
            adfile = new csFile();
            
        }
        public long FileLen
        {
            get
            {
                return length;
            }
        }
        public bool OpenFile(DirectoryInfo di)
        {
            if(di.Exists)
            {
                string timestring = DateTime.Now.Year.ToString("0000", CultureInfo.InvariantCulture) + DateTime.Now.Month.ToString("00", CultureInfo.InvariantCulture) + DateTime.Now.Day.ToString("00", CultureInfo.InvariantCulture) + DateTime.Now.Hour.ToString("00", CultureInfo.InvariantCulture)
                    + DateTime.Now.Minute.ToString("00", CultureInfo.InvariantCulture) + DateTime.Now.Second.ToString("00", CultureInfo.InvariantCulture);
                string filename = di.FullName;//文件路径
                filename = filename + "\\" + ext + timestring + ".dat";
                
                return adfile.BinaryOpenWrite(filename);
            }
            else
                return false;
        }
        public override void BinaryWrite(byte[] data)
        {
            adfile.bw.Write(data);
            length += data.Length;
        }
        public override void close()
        {
            length = 0;
            
            if (adfile.ts != null)
                adfile.ts.Close();
            if (adfile.ws != null)
                adfile.ws.Close();
            if (adfile.br != null)
                adfile.br.Close();
            if (adfile.bw != null)
            {
                adfile.bw.Close();
            }
            adfile.close();
        }
        
    }
}

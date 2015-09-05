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
	    private string fileName;
        protected StreamReader ts;
        protected StreamWriter ws;
        protected BinaryReader br;
        protected BinaryWriter bw;
	    private bool opened;
	    private bool writeOpened;
	    protected string ext = @"dat";
        protected long length;
	    protected long sizelimit;
        protected string header = "Log";
	    public DirectoryInfo Di = new DirectoryInfo(@".\");
		//-----------
        public csFile(string strheader = null, string extstr = null)
        {
            header = strheader ?? (header);
            ext = extstr ?? (ext);
			init();
		}
		//-----------
		private void init() {
			opened = false;
			writeOpened = false;
		    length = 0;
		    sizelimit = 1024*1024;//1MB
		}

	    public void SetFileSizeLimit(long size)
	    {
	        if (size>0)
	        {
	            sizelimit = size;
	        }
	    }
	    public string CreateFullFileName()
	    {
            string timestring = CreateNonBlankTimeString();

            return Di.FullName +"\\"+ header + timestring + "." + ext;
	    }

	    public string CreateNonBlankTimeString()
	    {
	         string timestring = DateTime.Now.Year.ToString("0000_", CultureInfo.InvariantCulture) +
	               DateTime.Now.Month.ToString("00_", CultureInfo.InvariantCulture) +
	               DateTime.Now.Day.ToString("00_", CultureInfo.InvariantCulture) +
	               DateTime.Now.Hour.ToString("00_", CultureInfo.InvariantCulture)
	               + DateTime.Now.Minute.ToString("00_", CultureInfo.InvariantCulture) +
	               DateTime.Now.Second.ToString("00", CultureInfo.InvariantCulture);
	        return timestring;
	    }
        public string CreateTimeString()
        {
            string timestring = "(" + DateTime.Now.Month.ToString("00", CultureInfo.InvariantCulture) + "/" + DateTime.Now.Day.ToString("00", CultureInfo.InvariantCulture) + " " + DateTime.Now.Hour.ToString("00", CultureInfo.InvariantCulture)
                    + ":" + DateTime.Now.Minute.ToString("00", CultureInfo.InvariantCulture) + ":" + DateTime.Now.Second.ToString("00", CultureInfo.InvariantCulture) + ")";
            return timestring;
        }
	    public void SetPath(DirectoryInfo di=null)
	    {
	        if (di != null)
	            Di = di;
	    }
		//-----------
		public csFile(string file_name) 	{
			fileName = file_name;
			init();
		}
        public long FileLen
        {
            get
            {
                return length;
            }
        }

	    public string FileName
	    {
	        get { return fileName; }
	    }

	    public bool Opened
	    {
	        get { return opened; }
	    }

	    public bool WriteOpened
	    {
	        get { return writeOpened; }
	    }


	    //-----------
        public bool OpenForRead(string file_name){
			fileName = file_name;
			try {
                ts = new StreamReader(File.Open(fileName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite));
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
        public virtual void Close()
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
		public bool OpenForWrite(string file_name)
		{
		    if (file_name == null) throw new ArgumentNullException("file_name");
		    try{
                fileName = file_name;
                ws = new StreamWriter(File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.Read));
                ws.AutoFlush = true;
				
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
            if (file_name == null) throw new ArgumentNullException("file_name");
            try
            {
                bw = new BinaryWriter(File.Open(file_name, FileMode.OpenOrCreate,FileAccess.Write,FileShare.Read));
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
            if (file_name == null) throw new ArgumentNullException("file_name");
            try
            {
                br = new BinaryReader(File.Open(file_name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
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
            bw.Write(data);
        }

	    public virtual bool Create()
	    {
	        return false;
	    }

	}

    public class LogFile:csFile
    {
        public LogFile(string strheader = null, string extstr = null)
            : base(strheader, extstr){}

        public static int index = 0;
        private bool OpenStreamFile()
        {
            if (Di.Exists == false)
            {
                Di.Create();
            }
            return Di.Exists && OpenForWrite(CreateFullFileName());
        }
        public bool Create()
        {
            return OpenStreamFile();
        }
                

        public long Write(string s)
        {
            if (WriteOpened == false)
            {
                if (Create() == false)
                    return 0;
            }

            s = CreateTimeString() + s;
            ws.WriteLine(s);

            length += s.Length;
            length += 2;
            if (length>sizelimit)
            {
                Close();
                header += (++index).ToString();
            }
            return length;
        }
        
    }

    public class ADFile : csFile
    {
        public ADFile(string strheader = null, string extstr = null)
            : base(strheader, extstr){}

        public static int index = 0;
        private bool OpenBinaryFile()
        {
            if (Di.Exists == false)
            {
                Di.Create();
            }
            return Di.Exists && BinaryOpenWrite(CreateFullFileName());
        }
        public override bool Create()
        {
            SetFileSizeLimit(5*1024*1024);
            return OpenBinaryFile();
        }

        //if you want to create new file everytime, call close() after write()
        public long Write(byte[] data)
        {
            if (WriteOpened == false)
            {
                if (Create() == false)
                    return 0;
            }
            bw.Write(data);
            length += data.Length;
            if (length>sizelimit)
            {
                Close();
                header += (++index).ToString();
            }
            return length;
        }
    }
}

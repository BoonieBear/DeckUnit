using BoonieBear.DeckUnit.TraceFileService;
using BoonieBear.DeckUnit.ACMP;
namespace BoonieBear.DeckUnit.Mov4500TraceService
{
    
    /// <summary>
    /// 4500记录文件类，调用tracefile类生成需要的记录文件，
    /// </summary>
    public class MovTraceService
    {
        private TraceFile _traceFile = TraceFile.GetInstance();
        private MonitorMode _mode = MonitorMode.SHIP;
        public string Error
        {
            get { return _traceFile.Errormsg; }
        }

        public void SetMode(MonitorMode mode )
        {
            _mode = mode;
        }
        public MovTraceService(MonitorMode mode)
        {
            SetMode(mode);
        }


        /// <summary>
        /// 根据运行模式生成不同的记录文件
        /// </summary>
        /// <returns>生成结果</returns>
        public bool CreateService()
        {
            if (_mode==MonitorMode.SHIP)
            {
                if (_traceFile.CreateFile("DEBUG", TraceType.String, "Debug", "dbg", @"\Debug") == false)
                {
                    return false;
                }
                if (_traceFile.CreateFile("GPS", TraceType.String, "Gps", "gps", @"\GPS") == false)
                {
                    return false;
                }
                if (_traceFile.CreateFile("USBL", TraceType.String, "Usbl", "pos", @"\USBL") == false)
                {
                    return false;
                }
                //通信数据
                if (_traceFile.CreateFile("XMTDATA", TraceType.Binary, "XmtData", "xdat", @"\XmtData") == false)
                {
                    return false;
                }

                if (_traceFile.CreateFile("RECVDATA", TraceType.Binary, "RecvData", "rdat", @"\RecvData") == false)
                {
                 
                    return false;
                }
                //水声数据
                if (_traceFile.CreateFile("RECVVOICE", TraceType.Binary, "RecvVoice", "rwav", @"\RecvVoice") == false)
                {
                 
                    return false;
                }
                //录音数据
                if (_traceFile.CreateFile("XMTVOICE", TraceType.Binary, "XmtVoice", "xwav", @"\XmtVoice") == false)
                {
                 
                    return false;
                }
                //航控数据
                if (_traceFile.CreateFile("ACOUSTICTOSAIL", TraceType.Binary, "AcousticData", "ats", @"\AtsData") == false)
                {
                 
                    return false;
                }
            }
            else if (_mode == MonitorMode.SUBMARINE)
            {
                if (_traceFile.CreateFile("DEBUG", TraceType.String, "Debug", "dbg", @"\Debug") == false)
                {
                 
                    return false;
                }

                if (_traceFile.CreateFile("BP", TraceType.String, "Bp", "bp", @"\BP") == false)
                {
                    return false;
                }

                //通信数据
                if (_traceFile.CreateFile("XMTDATA", TraceType.Binary, "XmtData", "xdat", @"\XmtData") == false)
                {
                    
                    return false;
                }

                if (_traceFile.CreateFile("RECVDATA", TraceType.Binary, "RecvData", "rdat", @"\RecvData") == false)
                {
                    
                    return false;
                }
                //水声数据
                if (_traceFile.CreateFile("RECVVOICE", TraceType.Binary, "RecvVoice", "rwav", @"\RecvVoice") == false)
                {
                    
                    return false;
                }
                //录音数据
                if (_traceFile.CreateFile("XMTVOICE", TraceType.Binary, "XmtVoice", "xwav", @"\XmtVoice") == false)
                {
                    
                    return false;
                }
                //航控数据
                if (_traceFile.CreateFile("SAILTOACOUSTIC", TraceType.Binary, "SailData", "sta", @"\StaData") == false)
                {
                    
                    return false;
                }
                if (_traceFile.CreateFile("ACOUSTICTOSAIL", TraceType.Binary, "AcousticData", "ats", @"\AtsData") == false)
                {
                    
                    return false;
                }
                //ADCP
                if (_traceFile.CreateFile("ADCP", TraceType.Binary, "Adcp", "vlt", @"\ADCP") == false)
                {
                    
                    return false;
                }
            }
            else//不可能
            {
                return false;
            }

            return true;
        }

        public bool TearDownService()
        {
            return _traceFile.Close();
        }

        public long Save(string sType, object bTraceBytes)
        {
            long ret = 0;
            switch (_traceFile.GeTraceType(sType))
            {
                case TraceType.Binary:
                    ret = _traceFile.WriteData(sType, (byte[])bTraceBytes);
                    break;
                case TraceType.SingleBinary:
                    ret = _traceFile.WriteSingleData(sType, (byte[])bTraceBytes);
                    break;
                case TraceType.String:
                    ret = _traceFile.WriteString(sType, (string)bTraceBytes);
                    break;
                default://none
                    ret = 0;
                    break;
 
            }
            return ret;
           
        }
    }
}

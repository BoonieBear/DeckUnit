using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using BoonieBear.DeckUnit.JsonUtils;
using TinyMetroWpfLibrary.Utility;
using BoonieBear.DeckUnit.ACNP;
namespace BoonieBear.DeckUnit.CommLib.Serial
{
    public abstract class SerialSerialServiceBase :ISerialService
    {
        #region 属性

        protected static SerialPort _serialPort;

        public static event EventHandler<CustomEventArgs> DoParse;
        private List<byte> _recvQueue = new List<byte>();
        public SerialServiceMode SerialServiceMode { get; set; }
        #endregion

        #region 方法
        public bool Init(SerialPort serialPort)
        {
            try
            {
                SerialServiceMode = SerialServiceMode.HexMode;
                _recvQueue.Clear();
                _serialPort = serialPort;
                if (SerialPort.GetPortNames().All(t => t != _serialPort.PortName.ToUpper()))
                {
                   return false;
                }
                if (!_serialPort.IsOpen) _serialPort.Open();
                return _serialPort.IsOpen;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            
        }

        public virtual void Register(Observer<CustomEventArgs> observer)
        {
            DoParse -= observer.Handle;
            DoParse+=observer.Handle;
        }

        public virtual void ChangeMode(SerialServiceMode mode)
        {
            SerialServiceMode = mode;
        }

        public virtual void UnRegister(Observer<CustomEventArgs> observer)
        {
            DoParse -= observer.Handle;
        }

        public bool Stop()
        {
            _serialPort.DataReceived -= _SerialPort_DataReceived;
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
            }
            catch (Exception)
            {
                
                return false;
            }
            return true;
        }

        public virtual bool Start()
        {
            _serialPort.DataReceived -= _SerialPort_DataReceived;
            _serialPort.DataReceived += _SerialPort_DataReceived;
            return _serialPort.IsOpen;
        }

        public virtual SerialPort ReturnSerialPort()
        {
            return _serialPort;
        }
        private void _SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var nCount = _serialPort.BytesToRead;
            if (SerialServiceMode==SerialServiceMode.HexMode && nCount < 16)
            {
                Thread.Sleep(50);
                return;
            }
            for (int i = nCount - 1; i >= 0; i--)
            {
                _recvQueue.Add((byte)_serialPort.ReadByte());
                CheckQueue(ref _recvQueue);
            }
            
        }

        protected abstract void CheckQueue(ref List<byte> lstBytes);

        public void OnParsed(CustomEventArgs eventArgs)
        {
            if (DoParse != null)
            {
                DoParse(this, eventArgs);
            }
        }
        #endregion
    }
    //使用ACN协议的串口服务类，Start()中传入解析后的数据类
    public class ACNSerialSerialService: SerialSerialServiceBase
    {
        
        
        protected override void CheckQueue(ref List<byte> queue)
        {
            var bytes = new byte[queue.Count];
            queue.CopyTo(bytes);
            var strcmd = Encoding.ASCII.GetString(bytes);
            switch (SerialServiceMode)
            {
                case SerialServiceMode.HexMode:
                    while (strcmd.Contains("EB90")&&strcmd.Contains("END"))
                    {
                        Debug.WriteLine(strcmd);
                        var eb90index = strcmd.IndexOf("EB90", StringComparison.Ordinal);
                        var endindex = strcmd.IndexOf("END", StringComparison.Ordinal)+3;
                        if ( eb90index < endindex  )
                        {
                            var payload = strcmd.Substring(eb90index, endindex - eb90index);
                            strcmd = strcmd.Remove(eb90index, endindex - eb90index);
                            var buf = new byte[endindex - eb90index];
                            Array.Copy(bytes, eb90index, buf, 0, endindex - eb90index);
                            //删除移走的字符
                            queue.RemoveRange(0, endindex);
                            ParseOnHexMode(buf,payload);
                        }
                    }
                    
                    break;
                case SerialServiceMode.LoaderMode:
                    ParseOnLoaderMode(strcmd);
                    queue.Clear();
                    break;
                default:
                    ParseOnLoaderMode(strcmd);
                    queue.Clear();
                    break;
            }
        
        }

        private void ParseOnLoaderMode(string hexString)
        {
            var e = new CustomEventArgs(0, hexString, null, 0, true, null, CallMode.LoaderMode, _serialPort); 
            OnParsed(e);
        }

        private void ParseOnHexMode(byte[] bytes, string hexString)
        {

                var str = hexString.Split(',');
                //MainForm.pMainForm.BuoyChoice.SelectedIndex = int.Parse(BuoyID);
                if (str[1] == "03")
                {
                    Debug.WriteLine("收到应答包");
                    if (str[3] == "Y")
                    {
                        var error = "命令发送成功";
                        
                        var e = new CustomEventArgs(0, hexString, bytes, 0, true, error, CallMode.AnsMode, _serialPort);
                        OnParsed(e);
                        //校验正确

                        //
                    }
                    else
                    {
                        var error = "命令发送失败";
                        var e = new CustomEventArgs(0, hexString, bytes, 0, false, error, CallMode.AnsMode, _serialPort);
                        OnParsed(e);
                    }
                }
                else//不是应答包
                {
                    //保存数据
                    /*
                    MSPRecvDataFile.OpenFile(MainForm.pMainForm.SerialRecvPathInfo);
                    MSPRecvDataFile.BinaryWrite(cmd);
                    MSPRecvDataFile.close();
                     */
                    string time;
                    byte[] data;
                    int id;
                    if (ACNProtocol.DepackCommData(bytes, out time, out id, out data))
                    {
                        var ack = "EB90,03," + ACNProtocol.BuoyID + ",Y,";
                        var crc = CRCHelper.CRC16(ack);
                        var bcrc = BitConverter.GetBytes(crc);
                        var tmp = bcrc[0];
                        bcrc[0] = bcrc[1];
                        bcrc[1] = tmp;
                        //ack += Encoding.Default.GetString(bcrc);
                        ack += "  ,END";
                        byte[] b = Encoding.Default.GetBytes(ack);
                        b[13] = bcrc[0];
                        b[14] = bcrc[1];
                        //MsgWriteLine(MsgMode.RecvSerialData, id, "MSP430", "上位机", SourceDataClass.DataId[id].ToString());
                        if (_serialPort.IsOpen)
                        {
                            _serialPort.Write(b, 0, b.Length);
                        }
                        //处理数据分为特殊命令和dsp数据
                        var mode = CallMode.DataMode;
                        if (id == 170)
                        {
                            /*
                            MSPDataFile.OpenFile(MainForm.pMainForm.SerialRecvPathInfo);
                            string DataFilename = MSPDataFile.adfile.fileName;
                            MSPDataFile.BinaryWrite(data);
                            MSPDataFile.close();
                            WriteCommWriteLine("收到DSP转发数据。");
                            */
                            string error = null;
                            var flag = false;
                            
                            try
                            {

                                ACNProtocol.GetDataForParse(data);
                                if (ACNProtocol.Parse())
                                {
                                    hexString = StringListToTree.LstToJson(ACNProtocol.parselist);
                                    flag = true;
                                }
                                else
                                {
                                    error = ACNProtocol.Errormessage;
                                    mode = CallMode.ErrMode;
                                    flag = false;
                                }
                                
                            }
                            catch (Exception exception)
                            {
                                error = exception.Message;
                                mode = CallMode.ErrMode;
                                flag = false;
                            }
                            finally
                            {
                                var e = new CustomEventArgs(170, hexString, data, data.Length, flag, error, mode, _serialPort);

                                OnParsed(e);
                            }

                        }
                        else
                        {
                            mode = CallMode.CommData;
                            var hexStr = StringHexConverter.ConvertCharToHex(data, data.Length);
                            var info = new StringBuilder();
                            if (id == 20)//浮标返回
                                id = 12;
                            string error = null;
                            bool flag = true;
                            try
                            {
                                
                                switch (id)
                                {
                                    case 12: //实时状态
                                        info.AppendLine("收到状态数据：");
                                        var rtcTime = hexStr.Substring(0, 4) + "年" + hexStr.Substring(4, 2) + "月"
                                                      + hexStr.Substring(6, 2) + "日" + hexStr.Substring(8, 2) + "时"
                                                      + hexStr.Substring(10, 2) + "分" + hexStr.Substring(12, 2) + "秒";
                                        info.AppendLine("RTC时间：" + rtcTime);
                                        var dateTimeStr = hexStr.Substring(0, 4) + "-" + hexStr.Substring(4, 2) + "-"
                                                          + hexStr.Substring(6, 2) + " " + hexStr.Substring(8, 2) + ":"
                                                          + hexStr.Substring(10, 2) + ":" + hexStr.Substring(12, 2);
                                        var dt = Convert.ToDateTime(dateTimeStr);
                                        var ts = dt.Subtract(DateTime.Now);
                                        if (ts.TotalSeconds < 0)
                                            info.AppendLine("RTC时间比本地时间慢" + Math.Abs(ts.TotalSeconds) + "秒");
                                        else
                                            info.AppendLine("RTC时间比本地时间快" + Math.Abs(ts.TotalSeconds) + "秒");
                                        info.AppendLine("浮标号：" + hexStr.Substring(14, 2));
                                        info.AppendLine("节点号：" + hexStr.Substring(16, 2));
                                        var sn = hexStr.Substring(18, 2);
                                        if (sn == "00")
                                            info.AppendLine("东经：" + int.Parse(hexStr.Substring(20, 4)) + "°" +
                                                            int.Parse(hexStr.Substring(24, 2)) + "." +
                                                            int.Parse(hexStr.Substring(26, 4)));
                                        else if (sn == "01")
                                            info.AppendLine("西经：" + int.Parse(hexStr.Substring(20, 4)) + "°" +
                                                            int.Parse(hexStr.Substring(24, 2)) + "." +
                                                            int.Parse(hexStr.Substring(26, 4)));
                                        else
                                            info.AppendLine("经度位置无可用数据");
                                        sn = hexStr.Substring(30, 2);
                                        if (sn == "00")
                                            info.AppendLine("北纬：" + int.Parse(hexStr.Substring(32, 2)) + "°" +
                                                            int.Parse(hexStr.Substring(34, 2)) + "." +
                                                            int.Parse(hexStr.Substring(36, 4)));
                                        else if (sn == "01")
                                            info.AppendLine("南纬：" + int.Parse(hexStr.Substring(32, 2)) + "°" +
                                                            int.Parse(hexStr.Substring(34, 2)) + "." +
                                                            int.Parse(hexStr.Substring(36, 4)));
                                        else
                                            info.AppendLine("纬度位置无可用数据");
                                        info.AppendLine("串口2设备:" +
                                                        Enum.GetName(typeof (DeviceAddr),
                                                            int.Parse(hexStr.Substring(40, 4))));
                                        info.AppendLine("串口3设备:" +
                                                        Enum.GetName(typeof (DeviceAddr),
                                                            int.Parse(hexStr.Substring(44, 4))));

                                        int emittype = int.Parse(hexStr.Substring(48, 2));
                                        if (emittype == 0)
                                            info.AppendLine("发射机类型:PWM发射机");
                                        else
                                            info.AppendLine("发射机类型:线性发射机");
                                        info.AppendLine("换能器个数:" + hexStr.Substring(50, 2));
                                        info.AppendLine("48V电流工作状态时间:");
                                        info.AppendLine("低电流时间:" + Int64.Parse(hexStr.Substring(52, 10)));
                                        info.AppendLine("中电流时间:" + Int64.Parse(hexStr.Substring(62, 10)));
                                        info.AppendLine("高电流时间:" + Int64.Parse(hexStr.Substring(72, 10)));
                                        info.AppendLine("单片机工作和休眠时间:");
                                        info.AppendLine("休眠时间:" + int.Parse(hexStr.Substring(82, 6)));
                                        info.AppendLine("工作时间:" + int.Parse(hexStr.Substring(88, 6)));
                                        info.AppendLine("电源数据:");
                                        info.AppendLine("3.3V电压值:" +
                                                        (double.Parse(hexStr.Substring(94, 4))/1000).ToString() + "V");
                                        info.AppendLine("48V电压值:" +
                                                        (double.Parse(hexStr.Substring(98, 6))/1000).ToString() + "V");
                                        info.AppendLine("48V电池剩余电量:" + double.Parse(hexStr.Substring(104, 8))/1000 +
                                                        "mA*h");
                                        info.AppendLine("48V已用电量:" + double.Parse(hexStr.Substring(112, 8))/1000 +
                                                        "mA*h");
                                        info.AppendLine("3V电池剩余电量:" + double.Parse(hexStr.Substring(120, 8))/1000 +
                                                        "mA*h");
                                        info.AppendLine("3V已用电量:" + double.Parse(hexStr.Substring(128, 8))/1000 + "mA*h");
                                        var minus = int.Parse(hexStr.Substring(136, 2));
                                        if (minus == 0)
                                            info.AppendLine("温度: 零上" + double.Parse(hexStr.Substring(138, 6))/1000 +
                                                            "°C");
                                        else
                                            info.AppendLine("温度: 零下" + double.Parse(hexStr.Substring(138, 6))/1000 +
                                                            "°C");
                                        minus = int.Parse(hexStr.Substring(144, 2));
                                        if (minus == 0)
                                            info.AppendLine("喂狗开关:" + "关");
                                        else
                                            info.AppendLine("喂狗开关:" + "开");
                                        info.AppendLine("AD门限值:" + int.Parse(hexStr.Substring(146, 2)));
                                        info.AppendLine("串口2定时唤醒时间:" + int.Parse(hexStr.Substring(148, 10)) + "秒");
                                        info.AppendLine("串口3定时唤醒时间:" + int.Parse(hexStr.Substring(158, 10)) + "秒");
                                        info.AppendLine("单片机重启次数:" + int.Parse(hexStr.Substring(168, 4)));
                                        info.AppendLine("浮标工作状态:" + (hexStr.Substring(172, 2) == "00" ? "休眠" : "工作"));
                                        info.AppendLine("版本信息:" +
                                                        (double.Parse(hexStr.Substring(174, 4))/1000).ToString("F03") +
                                                        " " + hexStr.Substring(178, 4) + "年" + hexStr.Substring(182, 2) +
                                                        "月"
                                                        + hexStr.Substring(184, 2) + "日");

                                        break;
                                    case 14: //版本号。2013年1月14日之后舍弃
                                        info.AppendLine("版本信息:" +
                                                        (double.Parse(hexStr.Substring(0, 4))/1000).ToString("F03") +
                                                        " " + hexStr.Substring(4, 4) + "年" + hexStr.Substring(8, 2) +
                                                        "月"
                                                        + hexStr.Substring(10, 2) + "日");
                                        break;
                                    case 10:
                                        info.AppendLine("通信机电量低报警！！！！！！！！！！！！");
                                        info.AppendLine("3.3V电压值:" +
                                                        (double.Parse(hexStr.Substring(0, 4))/1000).ToString() + "V");
                                        info.AppendLine("48V电压值:" +
                                                        (double.Parse(hexStr.Substring(4, 6))/1000).ToString() + "V");
                                        info.AppendLine("48V电池剩余电量:" + double.Parse(hexStr.Substring(10, 8))/1000 +
                                                        "mA*h");
                                        info.AppendLine("48V已用电量:" + double.Parse(hexStr.Substring(18, 8))/1000 + "mA*h");
                                        info.AppendLine("3V电池剩余电量:" + double.Parse(hexStr.Substring(26, 8))/1000 +
                                                        "mA*h");
                                        info.AppendLine("3V已用电量:" + double.Parse(hexStr.Substring(34, 8))/1000 + "mA*h");
                                        break;
                                    case 2:
                                        info.AppendLine("DSP故障!!!");
                                        //tabControl1.SelectedIndex = 1;
                                        break;
                                    case 15:
                                        info.AppendLine("休眠时间错误!!!");
                                        break;
                                    case 9:
                                        info.AppendLine("漏水报警！！！！！！！！！！！");
                                        //tabControl1.SelectedIndex = 1;
                                        break;
                                    default:
                                        if (hexStr.Length > 0)
                                            info.AppendLine("收到数据：" + hexStr);
                                        break;
                                }
                            }
                            catch (Exception exception)
                            {
                                flag = false;
                                mode = CallMode.ErrMode;
                                error = "链路数据错误：" + exception.Message;
                                var e = new CustomEventArgs(0, null, data, data.Length, flag, error, mode, _serialPort);
                                OnParsed(e);
                            }
                            finally
                            {

                                var e = new CustomEventArgs(id, info.ToString(), data, data.Length, flag, error, mode, _serialPort);
                                OnParsed(e);
                            }

                        }

                    }
                    else
                    {
                        var ack = "EB90,03," + ACNProtocol.BuoyID + ",N,";
                        var crc = CRCHelper.CRC16(ack);
                        var bcrc = BitConverter.GetBytes(crc);
                        var tmp = bcrc[0];
                        bcrc[0] = bcrc[1];
                        bcrc[1] = tmp;
                        //ack += Encoding.Default.GetString(bcrc);
                        ack += "  ,END";
                        var b = Encoding.Default.GetBytes(ack);
                        b[13] = bcrc[0];
                        b[14] = bcrc[1];

                        if (_serialPort.IsOpen)
                        {
                            //写消息框
                            _serialPort.Write(b, 0, b.Length);
                            //
                        }
                        const string error = "链路传输错误";
                        var e = new CustomEventArgs(0, null, data, data.Length, false, error, CallMode.ErrMode, _serialPort);
                        OnParsed(e);
                    }
                }
            }
   
        }
}

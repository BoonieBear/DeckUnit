using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyMetroWpfLibrary.Utility;
using BoonieBear.DeckUnit.BaseType;
using BoonieBear.DeckUnit.ACMP;
namespace BoonieBear.DeckUnit.Mov4500Conf
{
    public class MovConf
    {
        private readonly static object SyncObject = new object();
        private static MovConf _movConf;

        //配置文件
        private string xmldoc = "BasicConf.xml";//const
        public static MovConf GetInstance()
        {
            lock (SyncObject)
            {
                return _movConf ?? (_movConf = new MovConf());
            }
        }

        public string MyExecPath;
        protected MovConf()
        {
            MyExecPath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            xmldoc = MyExecPath + "\\" + xmldoc;

        }
        protected string GetValue(string[] str)
        {
            return XmlHelper.GetConfigValue(xmldoc, str);
        }
        protected bool SetValue(string[] str,string value)
        {
            return XmlHelper.SetConfigValue(xmldoc, str, value);
        }
        public MonitorMode GetMode()
        {
            string[] str = { "Setup", "Mode" };
            return (MonitorMode)Enum.Parse(typeof(MonitorMode), GetValue(str));
        }
        public bool SetMode(MonitorMode mode)
        {
            string value = ((int)mode).ToString();
            string[] str = { "Setup", "Mode" };
            return SetValue(str,value);
        }

        public int GetNormAmp()
        {
            string[] str = { "Setup", "NormAmp" };
            return int.Parse( GetValue(str));
        }

        public MonitorGMode GetGMode()
        {
            string[] str = { "Setup", "GMode" };
            return (MonitorGMode)Enum.Parse(typeof(MonitorGMode), GetValue(str));
        }
        public bool SetGMode(MonitorGMode mode)
        {
            string value = ((int)mode).ToString();
            string[] str = { "Setup", "GMode" };
            return SetValue(str, value);
        }
        public string GetShipIP()
        {
            string[] str = { "Net", "ShipIP" };
            return GetValue(str);
        }
        public bool SetShipIP(string ip)
        {
            string[] str = { "Net", "ShipIP" };
            return SetValue(str,ip);
        }
        
        public string GetUWVIP()
        {
            string[] str = { "Net", "UWVIP" };

            return GetValue(str);
        }
        public bool SetUWVIP(string ip)
        {
            string[] str = { "Net", "UWVIP" };
            return SetValue(str,ip);
        }
        public string GetXmtChannel()
        {
            string[] str = { "Setup", "XMTChannel" };
            return GetValue(str);
        }
        public string GetXmtAmp()
        {
            string[] str = { "Setup", "XMTAMP" };
            return GetValue(str);
        }
        public string GetGain()
        {
            string[] str = { "Setup", "Gain" };
            return GetValue(str);
        }
        public bool SetXmtChannel(int i)
        {
            string[] str = { "Setup", "XMTChannel" };
            return SetValue(str,i.ToString());
        }
        public bool SetXmtAmp(float amp)
        {
            string[] str = { "Setup", "XMTAMP" };
            return SetValue(str,amp.ToString("F3"));
        }
        public bool SetGain(int gain)
        {
            string[] str = { "Setup", "Gain" };
            return SetValue(str,gain.ToString());
        }
        public string GetComPort()
        {
            string[] str = {"Net", "ComPort" };
            return GetValue(str);
        }
        public string GetDataPort()
        {
            string[] str = { "Net", "DataPort" };
            return GetValue(str);
        }
        public string GetBroadCastPort()
        {
            string[] str = { "Net", "Broadcast" };
            return GetValue(str);
        }
        public ModemConfigure GetModemConfigure()
        {
            throw new NotImplementedException();
        }


        public CommType GetBPComm()
        {
            string[] str = { "COM", "BPComm" };
            string sComm = GetValue(str);
            if (sComm == null)
                sComm = "COM10,19200,8,0,1";
            string[] string_Comm = sComm.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            CommType ComPara = new CommType();
            ComPara.Comm = string_Comm[0];
            ComPara.Baud = int.Parse(string_Comm[1]);
            ComPara.DataBits = int.Parse(string_Comm[2]);
            ComPara.Parity = int.Parse(string_Comm[3]);
            ComPara.StopBits = int.Parse(string_Comm[4]);
            return ComPara;
        }

        public CommType GetADCPComm()
        {
            string[] str = { "COM", "ADCPComm" };
            string sComm = GetValue(str);
            if (sComm == null)
                sComm = "COM3,9600,8,0,1";
            string[] string_Comm = sComm.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            CommType ComPara = new CommType();
            ComPara.Comm = string_Comm[0];
            ComPara.Baud = int.Parse(string_Comm[1]);
            ComPara.DataBits = int.Parse(string_Comm[2]);
            ComPara.Parity = int.Parse(string_Comm[3]);
            ComPara.StopBits = int.Parse(string_Comm[4]);
            return ComPara;
        }

        public OASSEND[] GetOASID()
        {
            string[] str = { "Setup", "OASID" };
            string sOASID = GetValue(str);
            string[] string_OASID = sOASID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            OASSEND[] OASID = new OASSEND[string_OASID.Length];
            for (int i = 0; i < string_OASID.Length; i++)
            {
                OASID[i].direction = i;
                OASID[i].id = int.Parse(string_OASID[i]);
            }
            return OASID;

        }

        public int[] GetCycID()
        {
            string[] str = { "Setup", "CycID" };
            string sCycID = GetValue(str);
            string[] string_CycID = sCycID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int[] CycID = new int[string_CycID.Length];
            for (int i = 0; i < string_CycID.Length; i++)
            {
                CycID[i] = int.Parse(string_CycID[i]);
            }
            return CycID;
        }

        public int GetPulseWidth()
        {
            string[] str = { "Setup", "PulseWidth" };
            return int.Parse(GetValue(str));
        }

        public int GetDistLimit()
        {
            string[] str = { "Setup", "DistLimit" };
            return int.Parse(GetValue(str));
        }

        public int GetGainDownLimit()
        {
            string[] str = { "Setup", "GainDownLimit" };
            return int.Parse(GetValue(str));
        }

        public int GetGainStart()
        {
            string[] str = { "Setup", "GainStart" };
            return int.Parse(GetValue(str));
        }

        public int GetGainUPLimit()
        {
            string[] str = { "Setup", "GainUPLimit" };
            return int.Parse(GetValue(str));
        }

        public int GetShootModel()
        {
            string[] str = { "Setup", "ShootModel" };
            return int.Parse(GetValue(str));
        }

        public int GetTVCModel()
        {
            string[] str = { "Setup", "TVCModel" };
            return int.Parse(GetValue(str));
        }

        public int GetTVCValue()
        {
            string[] str = { "Setup", "DistLimit" };
            return int.Parse(GetValue(str));
        }

        public float GetBlindRegion()
        {
            string[] str = { "Setup", "BlindRegion" };
            return float.Parse(GetValue(str));
        }

        public int GetGateTh()
        {
            string[] str = { "Setup", "GateTh" };
            return int.Parse(GetValue(str));
        }

        public int GetSoundSpeed()
        {
            string[] str = { "Setup", "SoundSpeed" };
            return int.Parse(GetValue(str));
        }

        public int GetSERVERMODE()
        {
            string[] str = { "Setup", "SERVERMODE" };
            return int.Parse(GetValue(str));
        }

        public int GetCELLNUM()
        {
            string[] str = { "Setup", "CELLNUM" };
            return int.Parse(GetValue(str));
        }


        public CommConfInfo GetCommConfInfo()
        {
            var cominfo = new CommConfInfo();
            try
            {
                if (GetMode()==MonitorMode.SHIP)
                {
                    cominfo.LinkIP = GetShipIP();
                }                    
                else
                {
                    cominfo.LinkIP = GetUWVIP();
                }
                cominfo.NetPort1 = int.Parse(GetComPort());
                cominfo.NetPort2 = int.Parse(GetDataPort());
                cominfo.TraceUDPPort = int.Parse(GetBroadCastPort());
                cominfo.BPComm = GetBPComm();
                cominfo.ADCPComm = GetADCPComm();
                
            }
            catch (Exception)
            {
                
                return null;
            }
            return cominfo;
        }

        /// <summary>
        /// 获取航控广播端口
        /// </summary>
        /// <returns></returns>
        public int GetSailPort()
        {
            string[] str = { "UDP", "Sail" };
            int port = 3000;
            int.TryParse(GetValue(str),out port);
            if (port == 0)
                port = 3000;
            return port;
        }
        /// <summary>
        /// 获取GPS广播端口
        /// </summary>
        /// <returns></returns>
        public int GetGPSPort()
        {
            string[] str = { "UDP", "GPS" };
            int port = 5000;
            int.TryParse(GetValue(str),out port);
            if (port == 0)
                port = 5000;
            return port;
        }
        // <summary>
        /// 获取USBL广播端口
        /// </summary>
        /// <returns></returns>
        public int GetUSBLPort()
        {
            string[] str = { "UDP", "USBL" };
            int port = 2000;
            int.TryParse(GetValue(str),out port);
            if (port == 0)
                port = 2000;
            return port;
        }
        // <summary>
        /// 获取mov广播端口
        /// </summary>
        /// <returns></returns>
        public int GetMovPort()
        {
            string[] str = { "UDP", "Mov" };
            int port = 4000;
            int.TryParse(GetValue(str),out port);
            if (port == 0)
                port = 4000;
            return port;
        }
        public MovConfInfo GetMovConfInfo()
        {
            var Movinfo = new MovConfInfo();
            try
            {
                Movinfo.GPSPort = GetGPSPort();
                Movinfo.Mode = (int)GetMode();
                Movinfo.USBLPort = GetUSBLPort();
                Movinfo.SailPort = GetSailPort();
                Movinfo.BroadCastPort = GetMovPort();
            }
            catch (Exception)
            {

                return null;
            }
            return Movinfo;
        }
    }
}

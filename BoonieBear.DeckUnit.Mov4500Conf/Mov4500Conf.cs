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

        public CommConfInfo GetCommConfInfo()
        {
            var cominfo = new CommConfInfo();
            try
            {
                if (GetMode()==MonitorMode.SHIP)
                    cominfo.LinkIP = GetShipIP();
                else
                {
                    cominfo.LinkIP = GetUWVIP();
                }
                cominfo.NetPort1 = int.Parse(GetComPort());
                cominfo.NetPort2 = int.Parse(GetDataPort());
                cominfo.TraceUDPPort = int.Parse(GetBroadCastPort());
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

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

        public MonitorMode GetMode()
        {
            string[] str = { "Setup", "Mode" };
            return (MonitorMode)Enum.Parse(typeof(MonitorMode), GetValue(str));
        }

        private string GetIP()
        {
            string[] str = { "Net", "IP" };
            return GetValue(str);
        }
        private string GetComPort()
        {
            string[] str = {"Net", "ComPort" };
            return GetValue(str);
        }
        private string GetDataPort()
        {
            string[] str = { "Net", "DataPort" };
            return GetValue(str);
        }
        private string GetBroadCastPort()
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
                cominfo.LinkIP = GetIP();
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

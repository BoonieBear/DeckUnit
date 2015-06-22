using System;
using System.Collections.Generic;
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

        protected MovConf()
        {
            string MyExecPath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            xmldoc = MyExecPath + "\\" + xmldoc;

        }
        protected string GetValue(string[] str)
        {
            return XmlHelper.GetConfigValue(xmldoc, str);
        }

        public MonitorMode GetMode()
        {
            string[] str = { "Mov", "Setup", "Mode" };
            return (MonitorMode)Enum.Parse(typeof(MonitorMode), GetValue(str));
        }

        private string GetIP()
        {
            string[] str = { "Mov", "Net", "IP" };
            return GetValue(str);
        }
        private string GetComPort()
        {
            string[] str = { "Mov", "Net", "ComPort" };
            return GetValue(str);
        }
        private string GetDataPort()
        {
            string[] str = { "Mov", "Net", "DataPort" };
            return GetValue(str);
        }
        private string GetBroadCastPort()
        {
            string[] str = { "Mov", "Net", "Broadcast" };
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


    }
}

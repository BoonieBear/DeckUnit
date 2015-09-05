using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.DAL.SqliteDAL;
using TinyMetroWpfLibrary.Utility;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.BaseType;
namespace BoonieBear.DeckUnit.DUConf
{
    public class DeckUnitConf
    {
        private readonly static object SyncObject = new object();
        private static DeckUnitConf _unitConf;
        private ISqlDAL sqlDal;
        //配置文件
        private string xmldoc = "BasicConf.xml";//const
        public static DeckUnitConf GetInstance()
        {
            lock (SyncObject)
            {
                return _unitConf ?? (_unitConf = new DeckUnitConf());
            }
        }
        public string Connectstring { get; set; }
        protected DeckUnitConf()
        {
            string MyExecPath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            xmldoc = MyExecPath + "\\" + xmldoc;
            Connectstring = GetSqlString();
            sqlDal = SqliteSqlDAL.GetInstance(Connectstring);
        }

        protected string GetValue(string[] str)
        {
            return XmlHelper.GetConfigValue(xmldoc, str);
        }

        public string GetSqlString()
        {
            string[] str = {"数据库配置", "连接字符串"};
            return GetValue(str);
        }

        public ModemConfigure GetModemConfigure()
        {
            if(sqlDal.LinkStatus)
                return sqlDal.GetModemConfigure();
            return null;
        }

        public CommConfInfo GetCommConfInfo()
        {
            if (sqlDal.LinkStatus)
                return sqlDal.GetCommConfInfo();
            return null;
        }

        public BaseInfo GetBaseInfo()
        {
            if (sqlDal.LinkStatus)
                return sqlDal.GetBaseConfigure();
            return null;
        }

        public List<AlarmConfigure> GetAlarmConfigure()
        {
            if (sqlDal.LinkStatus)
                return sqlDal.GetAlarmConfigureList("");
            return null;
        }

        public bool UpdateCommSet(CommConfInfo ciInfo)
        {
            if (sqlDal.LinkStatus)
            {
                sqlDal.UpdateCommConfInfo(ciInfo);
                var newinfo = GetCommConfInfo();
                if((newinfo.DataUDPPort == ciInfo.DataUDPPort)&&(newinfo.NetPort1 == ciInfo.NetPort1)&&(newinfo.NetPort2 == ciInfo.NetPort2)
                    &&(newinfo.SerialPortRate == ciInfo.SerialPortRate)&&(newinfo.TraceUDPPort == ciInfo.TraceUDPPort)&&(newinfo.LinkIP == ciInfo.LinkIP)
                    &&(newinfo.SerialPort == ciInfo.SerialPort))
                    return true;
            }
            return false;
        }
        public bool UpdateModemSet(ModemConfigure mfInfo)
        {
            if (sqlDal.LinkStatus)
            {
                sqlDal.UpdateModemConfigure(mfInfo);
                var newinfo = GetModemConfigure();
                if ((newinfo.ID == mfInfo.ID) && (newinfo.NetSwitch == mfInfo.NetSwitch) && (newinfo.NodeType == mfInfo.NodeType)
                    && (newinfo.TransducerNum == mfInfo.TransducerNum) && (newinfo.TransmiterType == mfInfo.TransmiterType) && (newinfo.AccessMode == mfInfo.AccessMode)
                    && (newinfo.Com2Device == mfInfo.Com2Device) && (newinfo.Com3Device == mfInfo.Com3Device))
                    return true;
            }
            return false;
        }
    }
}

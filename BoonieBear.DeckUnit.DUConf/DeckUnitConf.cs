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

        protected DeckUnitConf()
        {
            string MyExecPath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            xmldoc = MyExecPath + "\\" + xmldoc;
            string connstr = GetSqlString();
            sqlDal = new SqliteSqlDAL(connstr);
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
    }
}

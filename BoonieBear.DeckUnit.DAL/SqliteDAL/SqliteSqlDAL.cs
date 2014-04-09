using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.DAL.DBModel;

namespace BoonieBear.DeckUnit.DAL.SqliteDAL
{
    public class SqliteSqlDAL : ISqlite
    {
        private SqliteHelperSQL sqliteHelperSQL;
        private bool _linkStatus = false;

        private string[] _tableName =
        {
            "AlarmConfInfo", "BasicInfo", "CommandIDInfo", "CommandLog", "CommConfInfo",
            "DeviceInfo", "ModemConfInfo", "TaskInfo"
        };
        public SqliteSqlDAL(string connectionString)
        {
            sqliteHelperSQL = new SqliteHelperSQL(connectionString);
            _linkStatus = sqliteHelperSQL.Linked;
        }

        public void Close()
        {
            if (sqliteHelperSQL!=null)
            { 
                sqliteHelperSQL.CloseSqlConnection();
                sqliteHelperSQL = null;
            }
            _linkStatus = false;
        }
        public bool LinkStatus
        {
            get { return _linkStatus; }
            set { _linkStatus = value; }
        }

        

        public int AddAlarm(AlarmConfigure alarmConfigure)
        {
            if(sqliteHelperSQL.CheckExistVal(_tableName[0], "AlarmConf_ID", alarmConfigure.AlarmID.ToString()))
                throw new Exception("重复的ID值");
            string[] values = {alarmConfigure.AlarmID.ToString(),alarmConfigure.Alarmname,alarmConfigure.Floor.ToString(),alarmConfigure.Ceiling.ToString(),alarmConfigure.Alarmswitch.ToString(),alarmConfigure.Tips};
            using (sqliteHelperSQL.InsertInto(_tableName[0], values))
            {
                using (var reader = sqliteHelperSQL.ExecuteQuery("select last_insert_rowid()"))
                {
                    reader.Read();
                    return reader.GetInt32(0);
                }
            }
            
        }

        public void UpdateAlarmConfigure(AlarmConfigure alarmConfigure)
        {
            string[] col =
            {
                "AlarmConf_NAME", "AlarmConf_FLOOR", "AlarmConf_CEILING", "AlarmConf_SWITCH",
                "AlarmConf_TIPS"
            };
   
            string[] values =
            {
                alarmConfigure.Alarmname, alarmConfigure.Floor.ToString(),
                alarmConfigure.Ceiling.ToString(), alarmConfigure.Alarmswitch?"1":"0", alarmConfigure.Tips
            };

            sqliteHelperSQL.UpdateInto(_tableName[0], col, values, "AlarmConf_ID",
                    alarmConfigure.AlarmID.ToString()) ;
           
        }

        public void DeleteAlarm(int alarmid)
        {
            string[] col = {"AlarmConf_ID"};
            string[] val = {alarmid.ToString()};
            sqliteHelperSQL.Delete(_tableName[0], col, val);
        }

        public AlarmConfigure GetAlarmConfigure(int alarmid)
        {
            throw new NotImplementedException();
        }

        public DataSet GetAlarmList(string strWhere)
        {
            throw new NotImplementedException();
        }

        public CommConfInfo GetCommConfInfo()
        {
            throw new NotImplementedException();
        }

        public void UpdateCommConfInfo(CommConfInfo commConf)
        {
            throw new NotImplementedException();
        }

        public int AddTask(Task task)
        {
            throw new NotImplementedException();
        }

        public void Update(Task task)
        {
            throw new NotImplementedException();
        }

        public void DeleteTask(int id)
        {
            throw new NotImplementedException();
        }

        public Task GetTask(int id)
        {
            throw new NotImplementedException();
        }

        public DataSet GetTaskLst(string strWhere)
        {
            throw new NotImplementedException();
        }

        public BaseInfo GetBaseConfigure()
        {
            throw new NotImplementedException();
        }

        public int AddLog(CommandLog commandLog)
        {
            throw new NotImplementedException();
        }

        public void DeleteLog(int id)
        {
            throw new NotImplementedException();
        }

        public AlarmConfigure GetCommandLog(int id)
        {
            throw new NotImplementedException();
        }

        public DataSet GetLogLst(string strWhere)
        {
            throw new NotImplementedException();
        }

        public ModemConfigure GetModemConfigure()
        {
            throw new NotImplementedException();
        }

        public void UpdateModemConfigure(ModemConfigure modemConfigure)
        {
            throw new NotImplementedException();
        }

        public CommandID GetIDInfo(int id)
        {
            throw new NotImplementedException();
        }

        public DataSet GetIDList(string strWhere)
        {
            throw new NotImplementedException();
        }

        public int AddDevice(DeviceInfo deviceInfo)
        {
            throw new NotImplementedException();
        }

        public void DeleteDevice(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateDevice(int id)
        {
            throw new NotImplementedException();
        }

        public DataSet GetDeviceLst()
        {
            throw new NotImplementedException();
        }

        public DeviceInfo GetDeviceInfo(int id)
        {
            throw new NotImplementedException();
        }
    }
}

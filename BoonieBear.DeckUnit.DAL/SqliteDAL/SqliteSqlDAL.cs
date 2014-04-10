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
            "AlarmConfInfo", "BasicInfo",  "CommandLog", "CommConfInfo",
             "ModemConfInfo", "TaskInfo"
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
                using (var reader = sqliteHelperSQL.ExecuteQuery("SELECT COUNT(*) FROM " + _tableName[0]))
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

        public AlarmConfigure GetAlarmConfigureByID(int alarmid)
        {
            var lst = GetAlarmConfigureList("AlarmConf_ID = " + alarmid.ToString());
            if (lst.Count > 0)
                return lst[0];//count is always 0 because we check the id when add record;
            return null;
        }


        public List<AlarmConfigure> GetAlarmConfigureList(string strWhere)
        {
            var alarmlist = new List<AlarmConfigure>();

            var reader = sqliteHelperSQL.SelectWhere(_tableName[0],strWhere);
            while (reader.Read())
            {
                var al = new AlarmConfigure
                {
                    AlarmID = reader.GetInt32(0),
                    Alarmname = reader.GetString(1),
                    Floor = reader.GetFloat(2),
                    Ceiling = reader.GetFloat(3),
                    Alarmswitch = (bool)reader.GetValue(4),
                    Tips = reader.GetString(5)
                };
                alarmlist.Add(al);
            }
            return alarmlist;

        }

        public CommConfInfo GetCommConfInfo()
        {
            var reader = sqliteHelperSQL.ReadFullTable(_tableName[3]);
            if (reader.Read())
            {
                var ci = new CommConfInfo
                {
                    SerialPort = reader.GetString(0),
                    SerialPortRate = reader.GetInt32(1),
                    NetPort1 = reader.GetInt32(2),
                    NetPort2 = reader.GetInt32(3),
                    LinkIP = reader.GetString(4),
                    TraceUDPPort = reader.GetInt32(5)
                };
                return ci;
            }
            return null;
        }

        public void UpdateCommConfInfo(CommConfInfo commConf)
        {

            string[] col =
            {
                "CommConf_SERIAL", "CommConf_SERIALRATE", "CommConf_NET1", "CommConf_NET2",
                "CommConf_LINKIP","CommConf_TRACEPORT"
            };

            string[] values =
            {
                commConf.SerialPort, commConf.SerialPortRate.ToString(),commConf.NetPort1.ToString(),
                commConf.NetPort2.ToString(),commConf.LinkIP,commConf.TraceUDPPort.ToString()
            };

            sqliteHelperSQL.UpdateInto(_tableName[3], col, values, "","");
           
        }

        public int AddTask(Task task)
        {
            throw new NotImplementedException();
        }

        public void UpdateTask(Task task)
        {
            throw new NotImplementedException();
        }

        public void DeleteTask(int id)
        {
            throw new NotImplementedException();
        }

        public Task GetTask(long id)
        {
            throw new NotImplementedException();
        }

        public List<Task> GetTaskLst(string strWhere)
        {
            throw new NotImplementedException();
        }

        public BaseInfo GetBaseConfigure()
        {
            var br = sqliteHelperSQL.ReadFullTable(_tableName[1]);
            if (br.Read())
            {
                var bi = new BaseInfo();

                bi.Name = br.GetString(0);
                bi.Version = br.GetString(1);
                bi.Copyright = br.GetString(2);
                bi.Pubtime = br.GetDateTime(3).ToString();
                bi.Other = br.GetString(4);
                return bi;
            }
            return null;
        }

        public int AddLog(CommandLog commandLog)
        {
            int id = 0;
            using (var reader = sqliteHelperSQL.ExecuteQuery("SELECT COUNT(*) FROM " + _tableName[2]))
            {
                if(reader.Read())
                    id = reader.GetInt32(0);
            }
            var time = DateTime.UtcNow.ToString("s") ;
            string[] values = { (id + 1).ToString(), time, commandLog.CommID.ToString(), commandLog.Type ? "1" : "0", commandLog.SourceID.ToString(), commandLog.DestID.ToString(),commandLog.FilePath };
            sqliteHelperSQL.InsertInto(_tableName[2], values);
            return id+1;
        }

        public void DeleteLog(int id)
        {
            throw new NotImplementedException();
        }

        public CommandLog GetCommandLog(int id)
        {
            throw new NotImplementedException();
        }

        public List<CommandLog> GetLogLst(string strWhere)
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



    }
}

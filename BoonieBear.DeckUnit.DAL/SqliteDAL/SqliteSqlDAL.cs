﻿using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.DAL.DBModel;
using BoonieBear.DeckUnit.Utilities;

namespace BoonieBear.DeckUnit.DAL.SqliteDAL
{
    public class SqliteSqlDAL : ISqlDAL
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
            var b = new[] {0}; 
            task.ErrIndex.CopyTo(b,0);
            string[] values =
            {
                task.TaskID.ToString(), task.TaskState.ToString(),task.SourceID.ToString(),task.DestID.ToString(),task.DestPort.ToString(),task.CommID.ToString(),
                b[0].ToString(),task.HasPara?"1":"0", StringHexConverter.ConvertCharToHex(task.ParaBytes,(task.ParaBytes==null)?0:task.ParaBytes.Length),
                task.StarTime.ToString("s"),task.TotolTime.ToString(),task.LastTime.ToString("s"),task.RecvBytes.ToString(),task.FilePath,task.IsParsed?"1":"0",task.JSON,
            };
            using (sqliteHelperSQL.InsertInto(_tableName[5], values))
            {
                using (var reader = sqliteHelperSQL.ExecuteQuery("SELECT COUNT(*) FROM " + _tableName[5]))
                {
                    reader.Read();
                    return reader.GetInt32(0);
                }
            }
        }

        public void UpdateTask(Task task)
        {
            string[] col =
            {
                "TaskInfo_ID", "TaskInfo_STATE", "TaskInfo_SOURCEID", "TaskInfo_DESTID","TaskInfo_DESTPORT","TaskInfo_COMMDID",
                "TaskInfo_ERRINDEX","TaskInfo_ISPARA","TaskInfo_PARA","TaskInfo_STARTTIME","TaskInfo_TOTALTIME","TaskInfo_LASTENDTIME","TaskInfo_RECVBYTES",
                "TaskInfo_DATAPATH","TaskInfo_ISPARSED","TaskInfo_STRUCTDATA",
            };
            var b = new[] { 0};
            task.ErrIndex.CopyTo(b, 0);
            string[] values =
            {
                task.TaskID.ToString(), task.TaskState.ToString(),task.SourceID.ToString(),task.DestID.ToString(),task.DestPort.ToString(),task.CommID.ToString(),
                b[0].ToString(),task.HasPara?"1":"0", StringHexConverter.ConvertCharToHex(task.ParaBytes,task.ParaBytes.Length),
                task.StarTime.ToString("s"),task.TotolTime.ToString(),task.LastTime.ToString("s"),task.RecvBytes.ToString(),task.FilePath,task.IsParsed?"1":"0",task.JSON,
            };

            sqliteHelperSQL.UpdateInto(_tableName[5], col, values, "", "");
        }

        public void DeleteTask(long id)
        {
            string[] col = { "TaskInfo_ID" };
            string[] val = { id.ToString() };
            sqliteHelperSQL.Delete(_tableName[5], col, val);
        }

        public Task GetTask(long id)
        {

            var lst = GetTaskLst("TaskInfo_ID = " + id.ToString());
            if (lst.Count > 0)
                return lst[0];//count is always 0 because we check the id when add record;
            return null;

        }

        public List<Task> GetTaskLst(string strWhere)
        {
            var tasklist = new List<Task>();

            var reader = sqliteHelperSQL.SelectWhere(_tableName[5], strWhere);
            while (reader.Read())
            {
                var al = new Task();
                {
                    al.TaskID = reader.GetInt64(0);
                    al.TaskState = reader.GetInt32(1);
                    al.SourceID = reader.GetInt32(2);
                    al.DestID = reader.GetInt32(3);
                    al.DestPort = reader.GetInt32(4);
                    al.CommID = reader.GetInt32(5);
                    al.ErrIndex = new BitArray(new[]{reader.GetInt32(6)});
                    al.HasPara = (bool) reader.GetValue(7);
                    al.ParaBytes = StringHexConverter.ConvertHexToChar(reader.GetString(8));
                    al.StarTime = reader.GetDateTime(9);
                    al.TotolTime = reader.GetInt32(10);
                    al.LastTime = reader.GetDateTime(11);
                    al.RecvBytes = reader.GetInt32(12);
                    al.FilePath = reader.GetString(13);
                    al.IsParsed = (bool) reader.GetValue(14);
                    al.JSON = reader.GetString(15);
                };
                tasklist.Add(al);
            }
            return tasklist;
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
                string[] col = {"LogID"};
                string[] val = {id.ToString()};
                sqliteHelperSQL.Delete(_tableName[2], col, val);
        }

        public CommandLog GetCommandLog(int id)
        {
            var lst = GetLogLst("LogID = " + id.ToString());
            if (lst.Count > 0)
                return lst[0];//count is always 0 because we check the id when add record;
            return null;
        }

        public List<CommandLog> GetLogLst(string strWhere)
        {
            var comlst = new List<CommandLog>();

            var reader = sqliteHelperSQL.SelectWhere(_tableName[2], strWhere);
            while (reader.Read())
            {
                var al = new CommandLog
                {
                    LogID = reader.GetInt32(0),
                    LogTime = reader.GetDateTime(1),
                    CommID = reader.GetInt32(2),
                    Type = (bool)reader.GetValue(3),
                    SourceID = reader.GetInt32(4),
                    DestID = reader.GetInt32(5),
                    FilePath = reader.GetString(6),
                };
                comlst.Add(al);
            }
            return comlst;
        }

        public ModemConfigure GetModemConfigure()
        {
            var rd = sqliteHelperSQL.SelectWhere(_tableName[4],"");
            if (rd.Read())
            {
                var ci = new ModemConfigure
                {
                    ID = rd.GetInt32(0),
                    TransmiterType = rd.GetInt32(1),
                    TransducerNum = rd.GetInt32(2),
                    ModemType = rd.GetInt32(3),
                    Com2Device = rd.GetInt32(4),
                    Com3Device = rd.GetInt32(5),
                    NetSwitch = (bool)rd.GetValue(6),
                    NodeType = rd.GetInt32(7),
                    AccessMode = rd.GetInt32(8),
                };
                return ci;
            }
            return null;
        }

        public void UpdateModemConfigure(ModemConfigure modemConfigure)
        {
            string[] col =
            {
                "ModemConf_ID", "ModemConf_TRANSMITERTYPE", "ModemConf_TRANSDUCERNUM", "ModemConf_MODEMTYPE",
                "ModemConf_COM2DEVICE","ModemConf_COM3DEVICE","ModemConf_NETSWITCH","ModemConf_NODETYPE","ModemConf_ACCESSMODE"
            };

            string[] values =
            {
               modemConfigure.ID.ToString(),modemConfigure.TransmiterType.ToString(),modemConfigure.TransducerNum.ToString(),modemConfigure.ModemType.ToString(),
               modemConfigure.Com2Device.ToString(),modemConfigure.Com3Device.ToString(),modemConfigure.NetSwitch?"1":"0",modemConfigure.NodeType.ToString(),
               modemConfigure.AccessMode.ToString()
            };

            sqliteHelperSQL.UpdateInto(_tableName[4], col, values, "","");
        }



    }
}
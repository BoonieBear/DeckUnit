using System;
using System.Collections.Generic;
using System.Data;
using BoonieBear.DeckUnit.DAL.DBModel;

namespace BoonieBear.DeckUnit.DAL
{
    /// <summary>
    /// 数据库访问接口
    /// </summary>
    public interface ISqlite : IAlarmConfigure, ICommConfigure, ITask, IBaseConfigure, ICommandLog, IModemConfigure
    {
       
    }

    public interface IBaseConfigure
    {
        BaseInfo GetBaseConfigure();

    }
    public interface IAlarmConfigure
    {
        int AddAlarm(AlarmConfigure alarmConfigure);
        void UpdateAlarmConfigure(AlarmConfigure alarmConfigure);
        void DeleteAlarm(int alarmid);
        AlarmConfigure GetAlarmConfigureByID(int alarmid);
        List<AlarmConfigure> GetAlarmConfigureList(string strWhere);
    }

    public interface ICommConfigure
    {
        CommConfInfo GetCommConfInfo();
        void UpdateCommConfInfo(CommConfInfo commConf);

    }
    public interface ICommandLog
    {
        int AddLog(CommandLog commandLog);
        void DeleteLog(int id);
        CommandLog GetCommandLog(int id);
        List<CommandLog> GetLogLst(string strWhere);
    }
    public interface ITask
    {
        int AddTask(Task task);
        void UpdateTask(Task task);
        void DeleteTask(int id);
        Task GetTask(Int64 id);
        List<Task> GetTaskLst(string strWhere);
    }
    public interface IModemConfigure
    {
        ModemConfigure GetModemConfigure();
        void UpdateModemConfigure(ModemConfigure modemConfigure);
    }
}

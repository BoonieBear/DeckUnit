using System.Data;
using BoonieBear.DeckUnit.DAL.DBModel;

namespace BoonieBear.DeckUnit.DAL
{
    /// <summary>
    /// 数据库访问接口
    /// </summary>
    public interface ISqlite : IAlarmConfigure, ICommConfigure, ITask, IBaseConfigure, ICommandLog, IModemConfigure,
        ICommIDInfo, IDeviceInfo
    {
       
    }

    public interface IDeviceInfo
    {
        int AddDevice(DeviceInfo deviceInfo);
        void DeleteDevice(int id);
        void UpdateDevice(int id);
        DataSet GetDeviceLst();
        DeviceInfo GetDeviceInfo(int id);
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
        AlarmConfigure GetAlarmConfigure(int alarmid);
        DataSet GetAlarmList(string strWhere);
    }

    public interface ICommIDInfo
    {
        CommandID GetIDInfo(int id);
        DataSet GetIDList(string strWhere);
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
        AlarmConfigure GetCommandLog(int id);
        DataSet GetLogLst(string strWhere);
    }
    public interface ITask
    {
        int AddTask(Task task);
        void Update(Task task);
        void DeleteTask(int id);
        Task GetTask(int id);
        DataSet GetTaskLst(string strWhere);
    }
    public interface IModemConfigure
    {
        ModemConfigure GetModemConfigure();
        void UpdateModemConfigure(ModemConfigure modemConfigure);
    }
}

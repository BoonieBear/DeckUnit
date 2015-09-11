using System;
using System.Collections;

namespace BoonieBear.DeckUnit.BaseType
{
    /// <summary>
    /// 数据库实体类
    /// </summary>
    public class BaseInfo
    {
        private string _name;
        private string _version;
        private string _copyright;
        private string _pubtime;
        private string _other;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public string Copyright
        {
            get { return _copyright; }
            set { _copyright = value; }
        }

        public string Pubtime
        {
            get { return _pubtime; }
            set { _pubtime = value; }
        }

        public string Other
        {
            get { return _other; }
            set { _other = value; }
        }
    }

    public class AlarmConfigure
    {
        private int _alarmid;
        private string _alarmname;
        private float _floor;
        private float _ceiling;
        private bool _alarmswitch;
        private string _tips;

        public int AlarmID
        {
            get { return _alarmid; }
            set { _alarmid = value; }
        }

        public string Alarmname
        {
            get { return _alarmname; }
            set { _alarmname = value; }
        }

        public float Floor
        {
            get { return _floor; }
            set { _floor = value; }
        }

        public float Ceiling
        {
            get { return _ceiling; }
            set { _ceiling = value; }
        }

        public bool Alarmswitch
        {
            get { return _alarmswitch; }
            set { _alarmswitch = value; }
        }

        public string Tips
        {
            get { return _tips; }
            set { _tips = value; }
        }
    }

    public class CommandLog
    {
        public int LogID { get; set; }
        public DateTime LogTime { get; set; }

        public int CommID { get; set; }
        //数据类型，true:收到数据,false: 发送的命令
        public bool Type { get; set; }
        public int SourceID { get; set; }
        public int DestID { get; set; }
        public string FilePath { get; set; }
    }

    

    public class BDTask
    {
        public long TaskID { get; set; }
        public int TaskStage { get; set; }
        public int SourceID { get; set; }
        public int DestID { get; set; }

        public int DestPort { get; set; }
        public int CommID { get; set; }
        public int TotalPkg { get; set; }
        public bool HasPara { get; set; }
        public byte[] ParaBytes { get; set; }
        public DateTime StarTime { get; set; }
        public Int32 TotolTime { get; set; }
        public DateTime LastTime { get; set; }
        public int RecvBytes { get; set; }
        /// <summary>
        /// 完整数据文件路径
        /// </summary>
        public string FilePath { get; set; }
        public bool IsParsed { get; set; }
        public string ErrIdxStr { get; set; }//例子0;2;1;4;6
    }
}

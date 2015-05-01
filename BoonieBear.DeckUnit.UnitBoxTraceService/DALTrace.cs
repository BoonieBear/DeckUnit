﻿using System;
using System.Diagnostics;
using System.Xml.Serialization;
using BoonieBear.DeckUnit.DAL;

namespace BoonieBear.DeckUnit.UnitBoxTraceService
{
    /// <summary>
    /// 保存命令记录到数据库中
    /// </summary>
    public class DALTrace
    {
        private readonly static object SyncObject = new object();
        private static DALTrace _dalTrace;
        private ISqlDAL _sqldal;
        private static string connectstring = @"Data Source=default.dudb;Pooling=True";
        public string Errormsg { get; set; }
        public bool isLink { get; set; }

        public static string Connectstring
        {
            get { return connectstring; }
            set { connectstring = value; }
        }

        public static DALTrace GetInstance(string connstring="")
        {
            if (connstring!="")
            {
                connectstring = connstring;
            }
            
            lock (SyncObject)
            {
                return _dalTrace ?? (_dalTrace = new DALTrace(DBType.Sqlite));
            }
        }

        protected DALTrace(DBType dbType)
        {
            try
            {
                DALFactory.Connectstring = connectstring;
                _sqldal = DALFactory.CreateDAL(dbType);
                isLink = true;
            }
            catch (Exception e)
            {
                Errormsg = e.Message;
                isLink = false;
            }
        }

        public void CloseDAL()
        {
            if (_sqldal.LinkStatus)
                _sqldal.Close();
        }

        public bool SaveCommLog(CommandLog commandLog)
        {
            if (_sqldal.LinkStatus)
                return (_sqldal.AddLog(commandLog)>0);
            return false;
        }
    }
}
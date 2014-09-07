using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.DAL.SqliteDAL;

namespace BoonieBear.DeckUnit.DAL
{
    public enum DBType
    {
        Sqlite,
        MysqlL,
        Mssql,
    }
    /// <summary>
    /// 数据库访问工厂类
    /// </summary>
    public class DALFactory
    {
        static public  string connectstring;
        public static ISqlDAL CreateDAL(DBType dbType)
        {
            switch (dbType)
            {
                case DBType.Sqlite:
                    return new SqliteSqlDAL(connectstring);
                default:
                    return new SqliteSqlDAL(connectstring);
            }
        }
    }
}

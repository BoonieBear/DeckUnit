using BoonieBear.DeckUnit.DAL.SqliteDAL;

namespace BoonieBear.DeckUnit.DAL
{
    public enum DBType
    {
        Sqlite,
        Mysql,
        SqlServer,
    }
    /// <summary>
    /// 数据库访问工厂类
    /// </summary>
    public class DALFactory
    {
        private static string connectstring;

        public static string Connectstring
        {
            get { return connectstring; }
            set { connectstring = value; }
        }

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

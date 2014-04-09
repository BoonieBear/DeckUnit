using System;
using System.Diagnostics;
using BoonieBear.DeckUnit.DAL.SqliteDAL;
using NUnit.Framework;

namespace BoonieBear.DeckUnit.DALTests.SqliteDAL
{
    [TestFixture()]
    public class SqliteHelperSQLTests
    {
        private string connnectstring;
        private SqliteHelperSQL sqliteHelper;
        [TestFixtureSetUp]
        public void SqliteHelperSQLTest()
        {
            connnectstring = "Data Source=default.dudb;Pooling=True";
        }

        [SetUp]
        public void OpenDBTest()
        {
            sqliteHelper = new SqliteHelperSQL(connnectstring);
        }

        [TearDown]
        public void CloseSqlConnectionTest()
        {
            sqliteHelper.CloseSqlConnection();
        }

        [Test()]
        public void ExecuteQueryTest()
        {
            var reader = sqliteHelper.ExecuteQuery("SELECT  * FROM BasicInfo");
            {
                while (reader.Read())
                {
                    Debug.WriteLine(reader.GetValue(0).ToString() + "," + reader.GetString(1) + "," + reader.GetString(2) + "," + reader.GetString(3) + "," + reader.GetString(4));
                }
            }
            Assert.Greater(reader.VisibleFieldCount, 1);
        }

        [Test()]
        public void ReadFullTableTest()
        {
            var reader = sqliteHelper.ReadFullTable("BasicInfo");
            while (reader.Read())
            {
                Debug.WriteLine( reader.GetString(0) + "," + reader.GetString(1) + "," + reader.GetString(2) + "," + reader.GetDateTime(3).AddHours(8) + "," + reader.GetString(4));
            }
            Assert.Greater(reader.VisibleFieldCount,1);
        }

        [Test()]
        public void InsertIntoTest()
        {
            string[] str = { "1", "2", "3", "\"" + DateTime.UtcNow.ToString("s") + "\"", "5" };
            var reader = sqliteHelper.InsertInto("BasicInfo",str);
            ReadFullTableTest();
        }

        [Test()]
        public void UpdateIntoTest()
        {
            string[] col = { "Basic_NAME", "Basic_VERSION" };
            string[] val = {"jiabandanyuan","1.0.0.2"};
            var reader = sqliteHelper.UpdateInto("BasicInfo", col, val, "Basic_NAME", "水声通信甲板单元1");
            Assert.IsNotNull(reader);
        }

        [Test()]
        public void DeleteTest()
        {
            string[] col = {"Basic_VERSION"};
            string[] val = {"1.0.0.1"};
            Assert.NotNull(sqliteHelper.Delete("BasicInfo", col, val));
        }

        [Test()]
        public void InsertIntoSpecificTest()
        {
            string[] col = { "Basic_NAME", "Basic_VERSION" };
            string[] val = { "jiabandanyuan", "1.0.0.2" };
            Assert.NotNull(sqliteHelper.InsertIntoSpecific("BasicInfo", col, val));
        }
        [Test()]
        public void CreateTableTest()
        {
            string[] col = {"Basic_VERSION"};
            string[] val = {"NTEXT"};
            Assert.NotNull(sqliteHelper.CreateTable("TmpLst",col,val));
        }
        [Test()]
        public void DeleteContentsTest()
        {

            Assert.NotNull(sqliteHelper.DeleteContents("TmpLst"));
        }

       

        [Test()]
        public void SelectWhereTest()
        {
            string[] col = {"Basic_VERSION"};
            string[] item = { "Basic_OTHER" };
            string[] val = {"1.0.1"};
            string[] operation = {"="};
            var reader = sqliteHelper.SelectWhere("BasicInfo", item, col, operation, val);
            while (reader.Read())
            {
                Debug.WriteLine(reader.GetString(0));
            }
            Assert.Pass();
        }
        [Test()]
        public void CheckExistValTest()
        {
            Assert.IsTrue(sqliteHelper.CheckExistVal("BasicInfo", "Basic_NAME", "水声通信甲板单元"));
        }
    }
}

using System.Diagnostics;
using BoonieBear.DeckUnit.DAL.DBModel;
using BoonieBear.DeckUnit.DAL.SqliteDAL;
using NUnit.Framework;

namespace BoonieBear.DeckUnit.DALTests.SqliteDAL
{
    [TestFixture()]
    public class SqliteSqlDALTests
    {
        private SqliteSqlDAL Sqliteman;
        [SetUp()]
        public void SqliteSqlDALTest()
        {
            Sqliteman = new SqliteSqlDAL("Data Source=default.dudb;Pooling=True");
        }

        [TearDown]
        public void TearDown()
        {
            if(Sqliteman.LinkStatus)
                Sqliteman.Close();
        }
        [Test()]
        public void AddAlarmTest()
        {
            var al = new AlarmConfigure
            {
                AlarmID = 121,
                Alarmname = "test",
                Alarmswitch = false,
                Floor = 12.6,
                Ceiling = 29,
                Tips = "Nunit Test"
            };
            var ret = Sqliteman.AddAlarm(al);
            Debug.WriteLine("ret={0}",ret);
            Assert.GreaterOrEqual(ret,1);
        }

        [Test()]
        public void UpdateAlarmConfigureTest()
        {
            var al = new AlarmConfigure
            {
                AlarmID = 1321,
                Alarmname = "new test",
                Alarmswitch = true,
                Floor = 12.6,
                Ceiling = 229,
                Tips = "Nunit Test"
            };
            Sqliteman.UpdateAlarmConfigure(al);
            Assert.IsTrue(true);
        }

        [Test()]
        public void DeleteAlarmTest()
        {
            Sqliteman.DeleteAlarm(11);
            Assert.Pass();
        }

        [Test()]
        public void GetAlarmConfigureTest()
        {

        }

        [Test()]
        public void GetAlarmListTest()
        {

        }

        [Test()]
        public void GetCommConfInfoTest()
        {

        }

        [Test()]
        public void UpdateCommConfInfoTest()
        {

        }

        [Test()]
        public void AddTaskTest()
        {

        }

        [Test()]
        public void UpdateTest()
        {

        }

        [Test()]
        public void DeleteTaskTest()
        {

        }

        [Test()]
        public void GetTaskTest()
        {

        }

        [Test()]
        public void GetTaskLstTest()
        {

        }

        [Test()]
        public void GetBaseConfigureTest()
        {

        }

        [Test()]
        public void AddLogTest()
        {

        }

        [Test()]
        public void DeleteLogTest()
        {

        }

        [Test()]
        public void GetCommandLogTest()
        {

        }

        [Test()]
        public void GetLogLstTest()
        {

        }

        [Test()]
        public void GetModemConfigureTest()
        {

        }

        [Test()]
        public void UpdateModemConfigureTest()
        {

        }

        [Test()]
        public void GetIDInfoTest()
        {

        }

        [Test()]
        public void GetIDListTest()
        {

        }

        [Test()]
        public void AddDeviceTest()
        {

        }

        [Test()]
        public void DeleteDeviceTest()
        {

        }

        [Test()]
        public void UpdateDeviceTest()
        {

        }

        [Test()]
        public void GetDeviceLstTest()
        {

        }

        [Test()]
        public void GetDeviceInfoTest()
        {

        }
    }
}

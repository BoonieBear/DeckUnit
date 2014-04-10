using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
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
        public void GetAlarmConfigureListTest()
        {
            var lst = Sqliteman.GetAlarmConfigureList("");
            foreach (var al in lst)
            {
                Debug.WriteLine(al.AlarmID.ToString());
                Debug.WriteLine(al.Alarmname);
                Debug.WriteLine(al.Ceiling.ToString());
                Debug.WriteLine(al.Floor.ToString());
                Debug.WriteLine(al.Tips);
                Debug.WriteLine(al.Alarmswitch.ToString());
            }
            Assert.Pass();
        }
        
        [Test()]
        public void AddAlarmTest()
        {
            Exception exception = null;
            var al = new AlarmConfigure
            {
                AlarmID = 1211,
                Alarmname = "test",
                Alarmswitch = false,
                Floor = 12.6f,
                Ceiling = 29f,
                Tips = "Nunit Test"
            };
            try
            {
                var ret = Sqliteman.AddAlarm(al);
                Debug.WriteLine("ret={0}", ret);
                Assert.GreaterOrEqual(ret,1);
            }
            catch (Exception ex)
            {

                exception = ex;
            }
            if (exception!=null)
                Assert.AreEqual(exception.GetType(), typeof(Exception),exception.Message);
        }

        [Test()]
        public void UpdateAlarmConfigureTest()
        {
            var al = new AlarmConfigure
            {
                AlarmID = 1321,
                Alarmname = "new test",
                Alarmswitch = true,
                Floor = 12.6f,
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
        public void GetAlarmConfigureByIDTest()
        {
            var al = Sqliteman.GetAlarmConfigureByID(121);
            Debug.WriteLine(al.AlarmID.ToString());
            Debug.WriteLine(al.Alarmname);
            Debug.WriteLine(al.Ceiling.ToString());
            Debug.WriteLine(al.Floor.ToString());
            Debug.WriteLine(al.Tips);
            Debug.WriteLine(al.Alarmswitch.ToString());
            Assert.Pass();

        }

       

        [Test()]
        public void GetCommConfInfoTest()
        {
            var ci = Sqliteman.GetCommConfInfo();
            
            Debug.WriteLine(ci.SerialPort);
            Debug.WriteLine(ci.SerialPortRate.ToString());
            Debug.WriteLine(ci.NetPort1.ToString());
            Debug.WriteLine(ci.NetPort2.ToString());
            Debug.WriteLine(ci.LinkIP);
            Debug.WriteLine(ci.TraceUDPPort.ToString());
            Assert.Pass();
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
        public void UpdateTaskTest()
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
            var bi = Sqliteman.GetBaseConfigure();
            Debug.WriteLine(bi.Name);
            Debug.WriteLine(bi.Version);
            Debug.WriteLine(bi.Copyright);
            Debug.WriteLine(bi.Pubtime);
            Debug.WriteLine(bi.Other);
            
            Assert.Pass();
        }

        [Test()]
        public void AddLogTest()
        {
            var cl = new CommandLog();
            cl.CommID = 113;
            cl.DestID = 22;
            cl.FilePath = @"test\sfddf";
            cl.SourceID = 1;
            cl.Type = true;
            Assert.GreaterOrEqual(Sqliteman.AddLog(cl),1);
        }

        [Test()]
        public void DeleteLogTest()
        {
            Sqliteman.DeleteLog(1);
            Assert.Pass();
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

    }
}

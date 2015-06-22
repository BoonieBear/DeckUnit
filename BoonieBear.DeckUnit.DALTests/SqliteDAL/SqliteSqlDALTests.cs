using System;
using System.Collections;
using System.Diagnostics;
using BoonieBear.DeckUnit.DAL;
using BoonieBear.DeckUnit.DAL.SqliteDAL;
using NUnit.Framework;
using TinyMetroWpfLibrary.Utility;
using BoonieBear.DeckUnit.BaseType;
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
            var al = new CommConfInfo
            {
                SerialPort = "com4",
                SerialPortRate = 19200,
                NetPort1 = 1000,
                NetPort2 = 8081,
                LinkIP = "192.168.2.111",
                TraceUDPPort = 10000,
            };
            Sqliteman.UpdateCommConfInfo(al);
            Assert.IsTrue(true);
        }

        [Test()]
        public void AddTaskTest()
        {
            var task = new Task();
            task.TaskID = 201404101212150113;
            task.TaskState= 1;
            task.SourceID = 1;
            task.DestID = 7;
            task.DestPort = 3;
            task.CommID = 1;
            bool[] b =
            {
                true, false, true, false, true, false, true, false, true, false, true, false, true, false, true,
                false,true, false, true, false, true, false, true, false, true, false, true, false, true, false, true,
                false,
            };
            task.ErrIndex = new BitArray(b);
            task.HasPara = true;
            task.ParaBytes = new byte[]{12,15};
            task.StarTime = DateTime.UtcNow;
            task.TotolTime =1245;
            task.LastTime = DateTime.UtcNow;
            task.RecvBytes = 12222;
            task.FilePath = "log\\re\\re";
            task.IsParsed = false;
            task.JSON = "";
            Assert.GreaterOrEqual(Sqliteman.AddTask(task), 1);
        }

        [Test()]
        public void UpdateTaskTest()
        {
            var task = new Task();
            task.TaskID = 201404140000000131;
            task.TaskState = 1;
            task.SourceID = 1;
            task.DestID = 7;
            task.DestPort = 3;
            task.CommID = 1;
            bool[] b =
            {
                true, false, true, false, true, false, true, false, true, false, true, false, true, false, true,
                false,
            };
            task.ErrIndex = new BitArray(b);
            task.HasPara = true;
            task.ParaBytes = new byte[] { 12, 15 };
            task.StarTime = DateTime.UtcNow;
            task.TotolTime = 3456;
            task.LastTime = DateTime.UtcNow;
            task.RecvBytes = 243435;
            task.FilePath = "log\\re\\re";
            task.IsParsed = false;
            task.JSON = "2222222222";
            Sqliteman.UpdateTask(task);
        }

        [Test()]
        public void DeleteTaskTest()
        {
            Sqliteman.DeleteTask(201404140000000131);
            Assert.Pass();
        }

        [Test()]
        public void GetTaskTest()
        {
            var task = Sqliteman.GetTask(201404140000000131);
            if(task==null)
                Assert.Pass("No Task!");
            Debug.WriteLine(task.TaskID);
            Debug.WriteLine(task.TaskState);
            Debug.WriteLine(task.SourceID);
            Debug.WriteLine(task.DestID);
            Debug.WriteLine(task.DestPort);
            Debug.WriteLine(task.CommID);
            int[] a = {0};
            task.ErrIndex.CopyTo(a, 0);
            Debug.WriteLine(a[0]);
            Debug.WriteLine(task.HasPara);
            Debug.WriteLine(StringHexConverter.ConvertCharToHex(task.ParaBytes, task.ParaBytes.Length));
            Debug.WriteLine(task.StarTime.AddHours(8));
            Debug.WriteLine(task.TotolTime);
            Debug.WriteLine(task.LastTime.AddHours(8));
            Debug.WriteLine(task.RecvBytes);
            Debug.WriteLine(task.FilePath);

            Debug.WriteLine(task.IsParsed);
            Debug.WriteLine(task.JSON);
            Assert.Pass();
        }

        [Test()]
        public void GetTaskLstTest()
        {
            var lst = Sqliteman.GetTaskLst("TaskInfo_DESTID = 7");
            foreach (var task in lst)
            {
                Debug.WriteLine("//////////////////////////////////");
                Debug.WriteLine(task.TaskID);
                Debug.WriteLine(task.TaskState);
                Debug.WriteLine(task.SourceID);
                Debug.WriteLine(task.DestID);
                Debug.WriteLine(task.DestPort);
                Debug.WriteLine(task.CommID);
                int[] a = { 0 };
                task.ErrIndex.CopyTo(a, 0);
                Debug.WriteLine(a[0]);
                Debug.WriteLine(task.HasPara);
                Debug.WriteLine(StringHexConverter.ConvertCharToHex(task.ParaBytes, task.ParaBytes.Length));
                Debug.WriteLine(task.StarTime.AddHours(8));
                Debug.WriteLine(task.TotolTime);
                Debug.WriteLine(task.LastTime.AddHours(8));
                Debug.WriteLine(task.RecvBytes);
                Debug.WriteLine(task.FilePath);

                Debug.WriteLine(task.IsParsed);
                Debug.WriteLine(task.JSON);
            }
            Assert.Pass();
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
            var bi = Sqliteman.GetCommandLog(3);
            Debug.WriteLine(bi.LogID);
            Debug.WriteLine(bi.LogTime);
            Debug.WriteLine(bi.CommID.ToString());
            Debug.WriteLine(bi.Type);
            Debug.WriteLine(bi.SourceID);
            Debug.WriteLine(bi.DestID);
            Debug.WriteLine(bi.FilePath);
            Assert.Pass();
        }

        [Test()]
        public void GetLogLstTest()
        {
            var lst = Sqliteman.GetLogLst("");
            foreach (var commandLog in lst)
            {
                Debug.WriteLine(commandLog.LogID);
                Debug.WriteLine(commandLog.LogTime);
                Debug.WriteLine(commandLog.CommID.ToString());
                Debug.WriteLine(commandLog.Type);
                Debug.WriteLine(commandLog.SourceID);
                Debug.WriteLine(commandLog.DestID);
                Debug.WriteLine(commandLog.FilePath);
            }
            Assert.Pass();
        }

        [Test()]
        public void GetModemConfigureTest()
        {
            var bi = Sqliteman.GetModemConfigure();
            Debug.WriteLine(bi.Com2Device);
            Debug.WriteLine(bi.Com3Device);
            Debug.WriteLine(bi.ID.ToString());
            Debug.WriteLine(bi.ModemType);
            Debug.WriteLine(bi.NetSwitch);
            Debug.WriteLine(bi.NodeType);
            Debug.WriteLine(bi.TransducerNum);
            Debug.WriteLine(bi.TransmiterType);
            Debug.WriteLine(bi.AccessMode);
            Assert.Pass();
        }

        [Test()]
        public void UpdateModemConfigureTest()
        {
            var al = new ModemConfigure
            {
                ID = 12,
                TransducerNum = 4,
                TransmiterType = 0,
                ModemType = 1,
                Com2Device = 2,
                Com3Device = 3,
                NetSwitch = false,
                NodeType = 1,
                AccessMode = 1,
            };
            Sqliteman.UpdateModemConfigure(al);
            Assert.Pass();
        }

    }
}

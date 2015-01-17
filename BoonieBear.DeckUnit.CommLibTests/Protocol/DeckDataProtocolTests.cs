using System;
using System.Diagnostics;
using BoonieBear.DeckUnit.Protocol.ProtocolSevices;
using NUnit.Framework;

namespace BoonieBear.DeckUnit.CommLibTests.Protocol
{
    [TestFixture()]
    public class DeckDataProtocolTests
    {
        private int sid;
        private string dbstring;
        private string tmpdatapath;
        [SetUp]
        public void Setup()
        {
            sid = 1;
            dbstring = "default.dudb";
            DeckDataProtocol.Init(sid, dbstring);
        }

        [TearDown]
        public void Close()
        {
            Debug.WriteLine(DeckDataProtocol.SecondTicks);
            DeckDataProtocol.Stop();
        }
        [Test()]
        public void InitTest()
        {
            Assert.IsTrue(DeckDataProtocol.Init(sid, dbstring));
        }

        [Test()]
        public void CreateNewTaskTest()
        {
            DeckDataProtocol.Init(sid, dbstring);
            var id = DeckDataProtocol.StartNewTask(62, 3, TaskType.Reset, null);
            Assert.Greater(id,0);
            
        }
        [Test()]
        public void ContinueTaskTest()
        {
            DeckDataProtocol.Init(sid, dbstring);
            var id = DeckDataProtocol.ContinueTask(201404151359100162);
            Assert.AreEqual(201404151359100162, id);
        }
        [Test()]
        public void TaskPackageTest()
        {
            DeckDataProtocol.Init(sid, dbstring);
            Assert.NotNull(DeckDataProtocol.TaskPackage(201404151359100162));
        }

        
        [TestCase( new Byte[]{1,0}, ExpectedResult = TaskStage.Continue,TestName = "ParseOK")]
        [TestCase(new Byte[]{1,3,0,1,0,0}, ExpectedResult = TaskStage.Continue, TestName = "ParseEnd")]
        [TestCase(new Byte[]{1, 2}, ExpectedResult = TaskStage.Waiting, TestName = "Parsewaiting")]
        [TestCase(new Byte[] { 1, 1 }, ExpectedResult = TaskStage.Failed, TestName = "ParseFailed")]
        [TestCase(new Byte[] { 1, 4,0,0,0,0 }, ExpectedResult = TaskStage.Finish, TestName = "ParseFinished")]
        [TestCase(new Byte[] { 1, 4, 1, 0, 0, 0 }, ExpectedResult = TaskStage.Continue, TestName = "ParseFinishedButHasError")]
        [TestCase(new Byte[] { 2, 0,1,1,0,1 }, ExpectedResult = TaskStage.Continue, TestName = "ParseData")]
        [TestCase(new Byte[] { 3, 2 }, ExpectedResult = TaskStage.Undefine, TestName = "ParseUndefine")]
        public TaskStage ParseDataTest(byte[] bytes)
        {
            DeckDataProtocol.ContinueTask(201404161625160162);
            string err;
            return DeckDataProtocol.ParseData(bytes, out err);
        }
        [Test()]
        public void CheckFileIntegrityTest()
        {
            Assert.IsTrue(DeckDataProtocol.CheckFileIntegrity(201404161625160162));
        }

        [Test()]
        public void BuildFileTest()
        {
            string err;
            DeckDataProtocol.ContinueTask(201404161625160162);
            Assert.IsTrue(DeckDataProtocol.BuildFile(201404161625160162, out err));
        }
    }
}

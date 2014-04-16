using System.Diagnostics;
using System.IO;
using System.Threading;
using BoonieBear.DeckUnit.CommLib.Protocol;
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

        [Test()]
        public void ParseDataTest()
        {

        }
        [Test()]
        public void CheckFileIntegrityTest()
        {

        }

        [Test()]
        public void BuildFileTest()
        {

        }
    }
}

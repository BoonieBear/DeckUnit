using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using BoonieBear.DeckUnit.TraceFileService;
using NUnit.Framework;
namespace BoonieBear.DeckUnit.TraceFileServiceTests
{
    [TestFixture()]
    public class TraceFileTests
    {
        TraceFile _tracerFile = null;
        [SetUp]
        public void Setup()
        {
            _tracerFile = TraceFile.GetInstance();
            _tracerFile.CreateFile("log", TraceType.String, "LOG", "txt", @"\log");
            _tracerFile.CreateFile("log1", TraceType.String, "LOG1", "txt1", @"\log1");
            _tracerFile.CreateFile("log2", TraceType.String, "LOG2", "txt", @"\log2");
            _tracerFile.CreateFile("log3", TraceType.String, "LOG3", "txt1", @"\log3");
            _tracerFile.CreateFile("ad", TraceType.Binary, "ad", "dat", @"\dat");
            _tracerFile.CreateFile("ad1", TraceType.Binary, "ad1", "dat1", @"\dat1");
            
        }

        [TearDown]
        public void End()
        {
            Assert.IsTrue(_tracerFile.Close());
        }

        [Test()]
        public void GetInstanceTest()
        {
            var trace = TraceFile.GetInstance();
            Assert.AreSame(trace, _tracerFile);
        }

        [Test()]
        public void RemoveTest()
        {
            _tracerFile.Remove("1");
            Debug.WriteLine(_tracerFile.Errormsg);
            Assert.IsNotNull(_tracerFile.Errormsg);
        }

       
    }
}

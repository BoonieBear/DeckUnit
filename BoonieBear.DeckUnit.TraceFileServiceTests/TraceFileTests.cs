using BoonieBear.DeckUnit.TraceFileService;
using NUnit.Framework;
namespace BoonieBear.DeckUnit.TraceFileServiceTests
{
    [TestFixture()]
    public class TraceFileTests
    {
        [Test()]
        public void InitializeTest()
        {
            var boxTraceFile = TraceFile.GetInstance();
            Assert.IsTrue(boxTraceFile.Initialize());
            
        }
    }
}

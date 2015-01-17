using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.CommLib.Serial;
using BoonieBear.DeckUnit.Protocol.ProtocolSevices;
using NUnit.Framework;

namespace BoonieBear.DeckUnit.CommLibTests
{
    [TestFixture()]
    public class SerialCommandTests
    {
        private SerialPort serial;
        [TestFixtureSetUp]
        public void Init()
        {
            serial = new SerialPort("com3", 9600);
            ACNProtocol.Init();
        }
        [Test]
        public void SerialSeviceTest()
        {
           

            var IServiceFactory = new ACNSerialServiceFactory();
            var AcnSerialsevice = IServiceFactory.CreateService();
            if (AcnSerialsevice.Init(serial))
            {

                Assert.IsTrue(AcnSerialsevice.Start());
            }
            else
            {
                Assert.Fail();
            }
        }
        [Test]
        public  void ACNSerialCMDTest()
        {
            

            var IServiceFactory = new ACNSerialServiceFactory();
            var AcnSerialsevice = IServiceFactory.CreateService();
            if (AcnSerialsevice.Init(serial))
            {

                if(AcnSerialsevice.Start())
                {
                    
                    Assert.IsTrue(SendcommandAsync(AcnSerialsevice));
                }
                else
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }
            
        }


        bool SendcommandAsync(ISerialService AcnSerialsevice)
        {
            var auto = new AutoResetEvent(false); 
            var commfactory = new ACNSerialCommHexCommandFactory(AcnSerialsevice.ReturnSerialPort());
            var setdebugmode = commfactory.CreateSerialComm(250, new byte[] { 0x01 }, null);
            AcnSerialsevice.Register(setdebugmode);
            Command.SendSerialAsync(setdebugmode).ContinueWith(t => AcnSerialsevice.UnRegister(setdebugmode));
            Thread.Sleep(500);
            var serialcommand = commfactory.CreateSerialComm(247, new byte[] { 247 }, null);
            AcnSerialsevice.Register(serialcommand);
            var ans = Command.SendSerialAsync(serialcommand);

            ans.ContinueWith(t =>
            {
                if (t.Result)
                {
                    RecvAsync(serialcommand, auto);
                }
                else
                {
                    Debug.WriteLine(Command.Error);
                    Assert.Fail();
                }

            });
            return auto.WaitOne(3000);
        }
        static void RecvAsync(SerialBaseComm serialcommand,AutoResetEvent auto)
        {
            var task = Command.RecvSerialAsync(serialcommand);
            task.ContinueWith(t =>
            {
                if (t.Result != null)
                {
                    if (t.Result.ParseOK)
                    {
                        Debug.Write(task.Result.Outstring);
                        auto.Set();
                    }
                    else
                    {
                        Debug.WriteLine(task.Result.ErrorMsg);
  
                    }
                    Assert.IsTrue(t.Result.ParseOK);
                }
                else
                {
                    Debug.WriteLine("超时");
               
                }
                Assert.Fail();
            });

        }
        

    }
}

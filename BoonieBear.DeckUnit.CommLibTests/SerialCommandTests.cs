using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.CommLib.Serial;
using BoonieBear.DeckUnit.CommLib.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace BoonieBear.DeckUnit.CommLibTests
{
    [TestClass()]
    public class SerialCommandTests
    {

        [TestMethod()]
        public void SerialSeviceTest()
        {
            var serial = new SerialPort("com3",9600);
            ACNProtocol.Init();

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
        [TestMethod()]
        public  void ACNSerialCMDTest()
        {
            var serial = new SerialPort("com3", 9600);
            ACNProtocol.Init();

            var IServiceFactory = new ACNSerialServiceFactory();
            var AcnSerialsevice = IServiceFactory.CreateService();
            if (AcnSerialsevice.Init(serial))
            {

                if(AcnSerialsevice.Start())
                {
                    string error = null;
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
            var serialcommand = commfactory.CreateSerialComm(ACNCommandMode.CmdIDMode, 247, null,null);
            AcnSerialsevice.Register(serialcommand);
            var ans =  Command.SendSerialAsync(serialcommand);
            
            ans.ContinueWith(t =>
            {
                if(t.Result)
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

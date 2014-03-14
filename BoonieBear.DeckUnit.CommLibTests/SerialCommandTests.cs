using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
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
            var serial = new SerialPort("com21",9600);
            ACNSerialProtocol.Init();

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
        public async Task ACNSerialCMDTest()
        {
            var serial = new SerialPort("com21", 9600);
            ACNSerialProtocol.Init();

            var IServiceFactory = new ACNSerialServiceFactory();
            var AcnSerialsevice = IServiceFactory.CreateService();
            if (AcnSerialsevice.Init(serial))
            {

                if(AcnSerialsevice.Start())
                {
                    string error = null;
                    var ans = SendcommandAsync(AcnSerialsevice);
                    var bans = await ans;
                   
                    Assert.IsTrue(bans);
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

        async Task<bool> SendcommandAsync(ISerialService AcnSerialsevice)
        {
            var commfactory = new ACNSerialCommHexCommandFactory(AcnSerialsevice.ReturnSerialPort());
            var serialcommand = commfactory.CreateSerialComm(ACNCommandMode.CmdIDMode, 247, null,null);
            AcnSerialsevice.Register(serialcommand);
            var ans = await Command.SendCommandAsync(serialcommand);
             
            if (ans)
            {
                var e = await Command.RecvDataAsync(serialcommand);
                if (e != null)
                {
                    if (e.ParseOK)
                    {
                        Debug.Write(e.Outstring);
                    }
                    else
                    {
                        Debug.WriteLine(e.ErrorMsg);
                    }
                }
                else
                {
                    Debug.WriteLine("超时");
                }
            }
            AcnSerialsevice.UnRegister(serialcommand);
            return ans;
        }
    }
}

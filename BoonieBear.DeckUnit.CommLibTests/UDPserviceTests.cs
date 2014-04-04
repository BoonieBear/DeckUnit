using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.CommLib.Protocol;
using BoonieBear.DeckUnit.CommLib.UDP;
using BoonieBear.DeckUnit.Utilities.JSON;
using NUnit.Framework;
using Newtonsoft.Json;

namespace BoonieBear.DeckUnit.CommLibTests
{
    [TestFixture]
    public class UDPserviceTests
    {
        UdpClient udpClient = null;
        private static AutoResetEvent autoReset = null;
        private static string str = null;
        [SetUp]
        public void Init()
        {
            if (udpClient==null)
                udpClient=new UdpClient(8080);
            autoReset = new AutoResetEvent(false);
            ACNProtocol.Init();
        }



        class MyClass:CommLib.IObserver<CustomEventArgs>
        {
            public void Handle(object sender, CustomEventArgs e)
            {
                if (e.ParseOK)
                {
                    if (e.Mode == CallMode.NoneMode)
                    {
                        str = e.Outstring;
                        autoReset.Set();
                    }
                    if (e.Mode == CallMode.DataMode)
                    {
                        var bytes = new byte[e.DataBufferLength];
                        Buffer.BlockCopy(e.DataBuffer,0,bytes,0,e.DataBufferLength);
                        ACNProtocol.GetData(bytes);
                        if (ACNProtocol.Parse())
                        {
                            var nodetree = StringListToTree.TransListToNodeWriteLineic(ACNProtocol.parselist);
                            var newtree = StringListToTree.RemoveFatherPointer(nodetree);
                            var jsonstr = JsonConvert.SerializeObject(newtree);
                            str = jsonstr;
                            Debug.Write(str);
                            autoReset.Set();
                        }
                        else
                        {
                            Assert.Fail();
                        }

                    }
                }
                else
                {
                    Debug.WriteLine(e.ErrorMsg);
                }
            }
        }

        
        [Test]
        public void SetUpDebugUDPServiceTest()
        {
            var servicefactory = new UDPDebugServiceFactory();
            var debugservice = servicefactory.CreateService();
            debugservice.Register(new MyClass() );
            if (debugservice.Init(udpClient))
            {
                if (debugservice.Start())
                {
                    if (autoReset.WaitOne(10000))
                    {
                        debugservice.Stop();
                        Assert.AreEqual("aaa",str);
                    }
                    else
                    {
                        debugservice.Stop();
                        Assert.Fail();
                    }
                }
                else
                {
   
                    Assert.Fail();
                }
            }
            else
                Assert.Fail();


        }
        [Test]
        public void SetUpDataServiceTest()
        {
            var servicefactory = new UDPDataServiceFactory();
            var debugservice = servicefactory.CreateService();
            debugservice.Register(new MyClass());
            if (debugservice.Init(udpClient))
            {
                if (debugservice.Start())
                {
                    if (autoReset.WaitOne(10000))
                    {
                        debugservice.Stop();
                        Assert.IsNotNull(str,"str=null");
                    }
                    else
                    {
                        debugservice.Stop();
                        Assert.Fail();
                    }
                }
                else
                {

                    Assert.Fail();
                }
            }
            else
                Assert.Fail();
        }
        [Test]
        public void BroadCastUdpTest()
        {
            var command = new UDPCommFactory(udpClient);
            var comm = command.CreateUDPComm(ACNCommandMode.CmdCharMode,  null, "something");
            Assert.IsTrue(comm.BroadCast());
        }
        [Test]
        public void BroadCastDataUdpTest()
        {
            var command = new UDPCommFactory(udpClient);
            var comm = command.CreateUDPComm(ACNCommandMode.CmdWithData, new byte[]{30,31}, "something");
            Assert.IsTrue(comm.BroadCast());
        }
    }
}

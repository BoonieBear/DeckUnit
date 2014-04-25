using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BoonieBear.DeckUnit.CommLib;
using BoonieBear.DeckUnit.CommLib.TCP;
using NUnit.Framework;


namespace BoonieBear.DeckUnit.CommLibTests
{
    [TestFixture]
    public class TCPTests
    {
        TcpClient shelltcpClient = null;
        TcpClient datatcpClient = null;
        private ITCPClientService shellservice;
        private ITCPClientService dataservice;
        private string MyExecPath;
        [TestFixtureSetUp]
        public void init()
        {
            shelltcpClient = new TcpClient();
            shelltcpClient.SendTimeout = 1000;
            MyExecPath = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            datatcpClient = new TcpClient();
            datatcpClient.SendTimeout = 1000;
            ITCPServiceFactory service = new TCPShellServiceFactory();
            shellservice = service.CreateService();
            service = new TCPDataServiceFactory();
            dataservice = service.CreateService();
        }
        [Test]
        public void TestTCPconnect()
        {
            
            if (shellservice.Init(shelltcpClient, IPAddress.Parse("192.168.2.212"), 8080) &&
                (dataservice.Init(datatcpClient, IPAddress.Parse("192.168.2.212"), 8081)))
            {
                Task.Factory.StartNew(()=>shellservice.ConnectSync()).ContinueWith(t =>
                {
                    Debug.WriteLine(shellservice.Connected.ToString());
                    Task.Factory.StartNew(() => dataservice.ConnectSync()).ContinueWith(d =>
                    {

                        Debug.WriteLine(dataservice.Connected.ToString());
                        Assert.IsTrue(shellservice.Connected && dataservice.Connected);
                    });

                });
            }
            else
            {
                Assert.Fail();
            }
            
            
        }
        [Test]
        public void TestTCPShellCommand()
        {
            if (shellservice.Init(shelltcpClient, IPAddress.Parse("192.168.2.212"), 8080) )
            {
                Task.Factory.StartNew(() => shellservice.ConnectSync()).ContinueWith(t =>
                {
                    Debug.WriteLine(shellservice.Connected.ToString());
                    var shellcmd = new ACNTCPShellCommand(shelltcpClient, "aaa");
                    Command.SendTCPAsync(shellcmd).ContinueWith(c => Assert.IsNull(Command.Error));
                    
                });
            }
            else
            {
                Assert.Fail();
            }
        }
        [Test]
        public void TestTCPDataDownload()
        {

            if (shellservice.Init(shelltcpClient, IPAddress.Parse("192.168.2.212"), 8080) &&
                (dataservice.Init(datatcpClient, IPAddress.Parse("192.168.2.212"), 8081)))
            {
                Task.Factory.StartNew(() => shellservice.ConnectSync()).ContinueWith(t =>
                {
                    if (shellservice.Start())
                    {
                        Task.Factory.StartNew(() => dataservice.ConnectSync()).ContinueWith(d =>
                        {
                            if(!dataservice.Connected)
                                Assert.Fail();
                            dataservice.Start();
                            Debug.WriteLine(shellservice.Connected.ToString());
                            var shellcmd = new ACNTCPShellCommand(shelltcpClient, "gd");
                            Command.SendTCPAsync(shellcmd).ContinueWith(c =>
                            {
                                var datacmd = new ACNTCPStreamCommand(datatcpClient, File.Open(MyExecPath + @"\..\..\testdata\Ch1AD.dat", FileMode.Open), reportprogress);
                                dataservice.Register(datacmd);
                                Command.SendTCPAsync(datacmd).ContinueWith(cd =>
                                {
                                    dataservice.UnRegister(datacmd);
                                    Debug.WriteLine("call UnRegister");
                                    shellservice.Stop();
                                    dataservice.Stop();
                                    Assert.IsNull(Command.Error);
                                });

                            });
                        });
                        
                    }
                    else
                    {
                        Assert.Fail("(dataservice.Start failed");
                    }
                });
            }
            else
            {
                Assert.Fail();
            }
        }

        public void reportprogress(int i)
        {
            Debug.Write(i.ToString());
        }
    }
}

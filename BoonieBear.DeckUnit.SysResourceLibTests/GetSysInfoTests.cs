using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BoonieBear.DeckUnit.SysResourceLib;

namespace BoonieBear.DeckUnit.SysResourceLibTests
{
    [TestClass()]
    public class GetSysInfoTests
    {
        [TestMethod()]
        public void GetMemoryUsageTest()
        {
            var itest = GetSystemInfo.CreateResourcesProbe();
            var usage = itest.GetMemoryUsage();
            Debug.WriteLine("Memoryusage = {0}%",usage);
            Assert.IsTrue(usage > 0 && usage<100);
        }

        [TestMethod()]
        public void GetDiskUsageTest()
        {
            var itest = GetSystemInfo.CreateResourcesProbe();
            var usage = itest.GetDiskUsage();
            Debug.WriteLine("diskusage = {0}%", usage);
            Assert.IsTrue(usage > 0 && usage < 100);
        }

        [TestMethod()]
        public void GetMacAddressTest()
        {
            var itest = GetSystemInfo.CreateResourcesProbe();
            var addr = itest.GetMacAddress();
            Debug.WriteLine("addr ="+ addr);
            Assert.IsTrue(Regex.IsMatch(addr, @"^([0-9a-fA-F]{2})(([/\s:-][0-9a-fA-F]{2}){5})$"));
        }

        [TestMethod()]
        public void GetIpAddressTest()
        {
            var itest = GetSystemInfo.CreateResourcesProbe();
            var addr = itest.GetIpAddress();
            Debug.WriteLine("ipaddr =" + addr.ToString());
            Assert.IsTrue(Regex.IsMatch(addr, @"((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)"));
        }
    }
}


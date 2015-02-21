using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BoonieBear.DeckUnit.TraceFileService;
using NUnit.Framework;
namespace BoonieBear.DeckUnit.TraceFileServiceTests
{
    [TestFixture()]
    public class csFileTests
    {
        [Test()]
        public void csFileTest()
        {
            var testfile = new csFile("", "");
            Assert.IsNotNull(testfile);
        }

        [Test()]
        public void CreateFullFileNameTest()
        {
            var testfile = new csFile("test", "txt");
            Debug.WriteLine(testfile.CreateFullFileName());
            Assert.IsNotNullOrEmpty(testfile.CreateFullFileName());
        }

        [Test()]
        public void CreateNonBlankTimeStringTest()
        {
            var testfile = new csFile();
            Debug.WriteLine(testfile.CreateNonBlankTimeString());
            Assert.IsNotNullOrEmpty(testfile.CreateNonBlankTimeString());
        }

        [Test()]
        public void CreateTimeStringTest()
        {
            var testfile = new csFile();
            Debug.WriteLine(testfile.CreateTimeString());
            Assert.IsNotNullOrEmpty(testfile.CreateTimeString());
        }

        [Test()]
        public void SetPathTest()
        {
            var testfile = new csFile();
            Debug.WriteLine(testfile.Di.FullName);
            Assert.IsNotNull(testfile.Di);
        }

        [Test()]
        public void csFileTest1()
        {
            var testfile = new csFile("newfile");
            Assert.AreSame("newfile",testfile.fileName);
        }

        [Test()]
        public void OpenForReadTest()
        {
            var testfile = new csFile("newfile");
            Assert.IsTrue(testfile.OpenForRead());
        }

        [Test()]
        public void OpenForReadTest1()
        {
            var testfile = new csFile();
            Assert.IsTrue(testfile.OpenForRead("newfile"));
        }
        [Test()]
        public void OpenForReadNonExistFileTest()
        {
            var testfile = new csFile();
            Assert.IsFalse(testfile.OpenForRead("nofile"));
        }
        [Test()]
        public void readLineTest()
        {
            var testfile = new csFile();
            if (testfile.OpenForRead("newfile"))
            {
                Assert.AreEqual(testfile.readLine(), "1234567890");
            }
            Assert.Fail("OpenForRead failed");
        }

        [Test()]
        public void writeLineTest()
        {
            var testfile = new csFile();
            if (testfile.OpenForWrite("newfile1"))
            {
               testfile.writeLine("0123456789");
            }
            Assert.Fail("OpenForWrite failed");
        }

        [Test()]
        public void CloseTest()
        {
            var testfile = new csFile();
            if (testfile.OpenForRead("newfile"))
            {
                testfile.Close();
                Assert.IsFalse(testfile.writeOpened);
            }
            Assert.Fail("OpenForRead failed");
        }

        [Test()]
        public void OpenForWriteTest()
        {
            var testfile = new csFile();
            Assert.IsTrue(testfile.OpenForWrite("newfile2"));
        }

        [Test()]
        public void OpenForWriteTest1()
        {
            var testfile = new csFile("newfile2");
            Assert.IsTrue(testfile.OpenForWrite());
        }

        [Test()]
        public void BinaryOpenWriteTest()
        {
            var testfile = new csFile("newfile3");
            Assert.IsTrue(testfile.BinaryOpenWrite());
        }

        [Test()]
        public void BinaryOpenWriteTest1()
        {
            var testfile = new csFile();
            Assert.IsTrue(testfile.BinaryOpenWrite("newfile3"));
        }

        [Test()]
        public void BinaryOpenReadTest()
        {
            var testfile = new csFile("newfile3");
            Assert.IsTrue(testfile.BinaryOpenRead());
        }

        [Test()]
        public void BinaryOpenReadTest1()
        {
            var testfile = new csFile();
            Assert.IsTrue(testfile.BinaryOpenRead("newfile3"));
        }

        [Test()]
        public void BinaryWriteTest()
        {
            var testfile = new csFile();
            if (testfile.BinaryOpenWrite("newfile3"))
            {
                byte[] bytes = new byte[2];
                testfile.BinaryWrite(bytes);
            }
            Assert.Fail("OpenForRead failed");
        }
    }
}

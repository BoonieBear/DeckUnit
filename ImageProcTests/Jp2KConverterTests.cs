using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using NUnit.Framework;
namespace ImageProc.Tests
{
    [TestFixture()]
    public class Jp2KConverterTests
    {
        [Test()]
        public void LoadImageTest()
        {
            Assert.IsTrue(Jp2KConverter.LoadImage("9.bmp"));
        }

        [Test()]
        public void LoadImageTest1()
        {
            Image img = new Bitmap("9.bmp");
            Assert.IsTrue(Jp2KConverter.LoadImage(img));
        }

        [Test()]
        public void LoadJp2kTest()
        {
            Assert.IsTrue(Jp2KConverter.LoadJp2k("9_1.jpc"));
        }


        [Test()]
        public void SaveImgTest()
        {
            if (Jp2KConverter.LoadJp2k("9_1.jpc"))
            {
                Assert.IsNotNull(Jp2KConverter.SaveImg("9_1.jpg"));
            }
        }

        [Test()]
        public void SaveJp2KTest()
        {
            if (Jp2KConverter.LoadImage("9.bmp"))
            {
                Assert.IsNotNull(Jp2KConverter.SaveJp2K("9_1.jpc"));
            }
            else
                Assert.Fail();
        }
    }
}

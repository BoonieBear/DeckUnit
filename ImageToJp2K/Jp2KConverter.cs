using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace ImageProc
{
    /// <summary>
    /// 将得到的图像转成jp2k格式，可以设置名字和大小，打包jasper类库
    /// </summary>
    public static class Jp2KConverter
    {
        private static bool bImgLoad = false;
        private static bool bJp2kLoad = false;

        /// some flag TBD"ACS.exe","-f",filename,"-t","bmp","-T","jpc","-F",jpcstr.GetBuffer(0),"-O","rate=0.021"
        public const string INPUT_FILENAME = "Tempin ";
        private const string INPUT_FILENAME_FLAG = "-f ";
        public const string OUTPUT_FILENAME = "Tempout ";
        private const string OUTPUT_FILENAME_FLAG ="-F ";
        private const string ENCODD_CONVERT = "-t bmp -T jpc ";
        private const string DECODD_CONVERT = "-t jpc -T bmp ";
        private const string OPT = "-O rate=";
        private const int MAX_SIZE = 16560;
        
        public static bool LoadImage(string imgfilepath)
        {
            bImgLoad = false;
            bJp2kLoad = false;
            Image img = new Bitmap(imgfilepath);
            return LoadImage(img);
        }

        public static bool LoadImage(Image image)
        {
            bImgLoad = false;
            bJp2kLoad = false;
            try
            {
                using (Bitmap source = new Bitmap(image))
                {
                    double Width = source.Width;
                    double Height = source.Height;
                    if(Width*Height>512*512*2)
                    {
                        double resize = Math.Sqrt(512*512*2/(float) (Width*Height));
                        Width *= resize;
                        Height *= resize;
                        //新建一个bmp图片
                        Image newimage = new Bitmap((int)Width, (int)Height);
                        //新建一个画板
                        Graphics newg = Graphics.FromImage(newimage);
                        //设置质量
                        newg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        newg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        newg.Clear(Color.White);
                        //画图
                        newg.DrawImage(source, new Rectangle(0, 0, newimage.Width, newimage.Height), new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
                        using (Bitmap bmp = new Bitmap((int)Width, (int)Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                        {
                            Graphics.FromImage(bmp).DrawImage(newimage, new Rectangle(0, 0, bmp.Width, bmp.Height));
                            bmp.Save(INPUT_FILENAME, ImageFormat.Bmp);
                        }
                        //释放资源
                        newg.Dispose();
                        newimage.Dispose();

                    }
                    else
                    {
                        using (Bitmap bmp = new Bitmap((int)Width, (int)Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                        {
                            Graphics.FromImage(bmp).DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height));
                            bmp.Save(INPUT_FILENAME, ImageFormat.Bmp);
                        }
                    }
                    
                }
                bImgLoad = true;
            }
            catch (Exception)
            {

                return false;
            }

            return bImgLoad;
        }

        public static bool LoadJp2k(string imgfilepath)
        {
            try
            {
                FileInfo fi = new FileInfo(imgfilepath);
                fi.CopyTo(INPUT_FILENAME, true);
                bJp2kLoad = true;
                return true;
            }
            catch (Exception)
            {
                
            }
            return false;
        }
        public static bool LoadJp2k(byte[] imgBytes)
        {
            try
            {
                FileStream fs = new FileStream(INPUT_FILENAME, FileMode.Create);
                fs.Write(imgBytes, 0, imgBytes.Length);
                fs.Close();
                bJp2kLoad = true;
                return true;
            }
            catch (Exception)
            {
                
                
            }
            return false;
        }

        private static bool ConvertToImg()
        {
            if(!bJp2kLoad)
                return false;
            string DecodeOpt = INPUT_FILENAME_FLAG + INPUT_FILENAME + DECODD_CONVERT + OUTPUT_FILENAME_FLAG +
                               OUTPUT_FILENAME;
            try
            {
                Process ps = new Process();
                ps.StartInfo.FileName = "jasper.exe";
                ps.StartInfo.Arguments = DecodeOpt;
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                ps.WaitForExit();
                if (ps.HasExited)
                {
                    ps.Close();
                    return true;
                }
            }
            catch (Exception)
            {
                
            }
            return false;
        }
        private static bool ConvertToJp2K()
        {
            if (!bImgLoad)
                return false;
            FileInfo fi = new FileInfo(INPUT_FILENAME);
            float ratio = (float) MAX_SIZE/(float) fi.Length;
            string EncodeOpt = INPUT_FILENAME_FLAG + INPUT_FILENAME + ENCODD_CONVERT + OUTPUT_FILENAME_FLAG +
                               OUTPUT_FILENAME + OPT + ratio.ToString("f4");
            try
            {
                Process ps = new Process();
                ps.StartInfo.FileName = "jasper.exe";
                ps.StartInfo.Arguments = EncodeOpt;
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                ps.WaitForExit();
                if (ps.HasExited)
                {
                    ps.Close();
                    return true;
                }
            }
            catch (Exception)
            {
                
            }
            return false;
        }
        /// <summary>
        /// 保存为jpg
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>图像object</returns>
        public static object SaveImg(string filename)
        {
            if (!bJp2kLoad)
                return null;
            if (ConvertToImg())
            {
                if (File.Exists(OUTPUT_FILENAME))
                {
                    Image img = new Bitmap(OUTPUT_FILENAME);
                    try
                    {
                        img.Save(filename, ImageFormat.Jpeg);
                        img = new Bitmap(filename);
                        return img;
                    }
                    catch (Exception)
                    {

                        return null;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 保存为jpc格式
        /// </summary>
        /// <returns></returns>
        public static byte[] SaveJp2K(string filename)
        {
            if (!bImgLoad)
                return null;
            if (ConvertToJp2K())
            {
                if (File.Exists(OUTPUT_FILENAME))
                {
                    FileInfo fi = new FileInfo(OUTPUT_FILENAME);
                    try
                    {
                        fi.CopyTo(filename,true);
                        BinaryReader br =new BinaryReader(fi.Open(FileMode.Open));
                        byte[] bytes =  br.ReadBytes((int)fi.Length);
                        br.Close();
                        return bytes;
                    }
                    catch (Exception)
                    {

                        return null;
                    }
                }
            }
            return null;
        }
    }

    public static class IMGTool
    {
        public static Bitmap CutImage(Image b, int StartX, int StartY, int iWidth, int iHeight)
        {
            if (b == null)
            {
                return null;
            }
            int w = b.Width;
            int h = b.Height;
            if (StartX >= w || StartY >= h)
            {

                return null;
            }
            if (StartX + iWidth > w)
            {
                iWidth = w - StartX;
            }
            if (StartY + iHeight > h)
            {
                // 高度过大时只截取到最大大小 
                iHeight = h - StartY;
            }
            try
            {
                Bitmap bmpOut = new Bitmap(iWidth, iHeight);
                Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight),
                    GraphicsUnit.Pixel);
                g.Dispose();
                return bmpOut;
            }
            catch
            {
                return null;
            }
        }
    }

}

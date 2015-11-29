using System;
using System.Windows;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
namespace BoonieBear.DeckUnit.WaveBox
{
    class AudioFrame
    {
        private double[] _waveLeft;
        private double[] wave;
        private int[] pointlist;//store point`s y data
        System.Drawing.Point[] p;
        private int SamplesPerSecond;
        //private FifoStream _streamMemory;
        private double[] _fftLeft;
        double min = double.MaxValue;
        double max = double.MinValue;
        //private SignalGenerator _signalGenerator;
        private int _ymax = 32767;
        public int Fmax = 4000;
        public int _bitsPerSample = 16;
        public int _SampleByte = 2;
        public ArrayList _fftLeftSpect = new ArrayList();
        private int slide=5;
        private bool bFFT = false;
        private Bitmap canvas;


        public AudioFrame(int audioSamplesPerSecond, int maxFrequecy, int timedomainlen, int amp, int BitsPerSample, bool EnableFFT)
        {
            SamplesPerSecond = audioSamplesPerSecond;
            wave = new double[timedomainlen];
            Buffer.SetByte(wave, 0, 0);
            _ymax = amp;
            Fmax = maxFrequecy;
            _bitsPerSample = BitsPerSample;
            _SampleByte = _bitsPerSample / 8;
            ShowFFT = EnableFFT;
            canvas = new Bitmap(2048,300);
            p = new System.Drawing.Point[canvas.Width];
            pointlist = new int[canvas.Width];
        }

        
        public int Ymax
        {
            get { return _ymax; }
            set { _ymax = value; }
        }

        public bool ShowFFT
        {
            get { return bFFT; }
            set { bFFT = value; }
        }

        /// <summary>
        /// Process 16 bit sample
        /// </summary>
        /// <param name="wave"></param>
        
        public void AddData(byte[] buf)
        {
            _waveLeft = new double[buf.Length / _SampleByte];
            int h = 0;
            for (int i = 0; i < buf.Length; i += _SampleByte)
            {
                _waveLeft[h] = (double)BitConverter.ToInt16(buf, i);
                h++;
            }
            if (ShowFFT)
            {
                _fftLeft = FourierTransform.FFT(ref _waveLeft);
                Monitor.Enter(_fftLeftSpect);
                if (_fftLeft!=null)
                    _fftLeftSpect.Add(_fftLeft);
                Monitor.Exit(_fftLeftSpect);
            }
            // move point array
            int width = canvas.Width;
            int height = canvas.Height;
            double center = height / 2;
            double scale = 0.5 * height / _ymax;  // a 16 bit sample has values from -32768 to 32767
            double distance = (double)wave.Length / (double)width;//sampling,int better 8192/1024
            int movecount =(int)(_waveLeft.Length/distance);
            Array.Copy(pointlist, movecount, pointlist, 0, pointlist.Length - movecount);
            //sampling the input data to point array
            for (int x = 0; x < movecount; x++)
            {
                pointlist[pointlist.Length - movecount+x] = (int)(center - (_waveLeft[(int)(distance * x)] * scale));
                
            }
        }
        //给功率谱加窗，以免下采样显示时丢失谱线，但会造成谱分辨率下降
         private void AddWindows(double[] b)
         {

         }

         [System.Runtime.InteropServices.DllImport("gdi32.dll")]
         public static extern bool DeleteObject(IntPtr hObject);

         private System.Windows.Media.ImageSource BitmapToImageSource(System.Drawing.Bitmap bitmap)
         {
             IntPtr ptr = bitmap.GetHbitmap();
             System.Windows.Media.ImageSource result =
                 System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                     ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
             //release resource
             DeleteObject(ptr);

             return result;
         }
         private System.Drawing.Bitmap WpfImageSourceToBitmap(BitmapSource s)
         {
             System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(s.PixelWidth, s.PixelHeight, PixelFormat.Format32bppArgb);
             System.Drawing.Imaging.BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
             s.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
             bmp.UnlockBits(data);
             return bmp;
         }
        /// <summary>
        /// Render time domain to PictureBox
        /// </summary>
        /// <param name="CanvasBox"></param>
         public void RenderTimeDomain(ref System.Windows.Controls.Image ImageBox)
        {
            if (wave == null)
                return;
            // Set up for drawing
            Graphics offScreenDC = Graphics.FromImage(canvas);
            offScreenDC.Clear(ColorTranslator.FromHtml("#FF0F3A6E"));
            Pen pen = new System.Drawing.Pen(Color.WhiteSmoke);
            for (int i = 0; i < canvas.Width; i++)
            {
                p[i] = new System.Drawing.Point(i, pointlist[i]);
            }
            offScreenDC.DrawCurve(pen, p);
            // Clean up
            ImageBox.Source = BitmapToImageSource(canvas);
            offScreenDC.Dispose();
            
            
        }
 

        /// <summary>
        /// Render waterfall spectrogram to PictureBox
        /// </summary>
        /// <param name="CanvasBox"></param>
         public void RenderSpectrogram(ref System.Windows.Controls.Image ImageBox)
        {
            if (ImageBox.Source == null)
            {
                Graphics g = Graphics.FromImage(canvas);
                g.Clear(ColorTranslator.FromHtml("#FF0F3A6E"));
                ImageBox.Source = BitmapToImageSource(canvas);
                g.Dispose();
                return;
            }
            Graphics offScreenDC = Graphics.FromImage(canvas);
            int width = canvas.Width;
            int height = canvas.Height;
            
            Monitor.Enter(_fftLeftSpect);
            int len = _fftLeftSpect.Count;
            if (width < len)
            {
                _fftLeftSpect.RemoveRange(0, len-width);
                len = len - width;
            }
            if (len == 0)
            {
                Monitor.Exit(_fftLeftSpect);
                return;
            }
                
          
            double range = 0;

            for (int y = 0; y < _fftLeftSpect.Count; y++)
            {
                for (int x = 0; x < ((double[])_fftLeftSpect[_fftLeftSpect.Count - y - 1]).Length; x++)
                {
                    double amplitude = ((double[])_fftLeftSpect[_fftLeftSpect.Count - y - 1])[x];
                    if (min > amplitude)
                    {
                        
                        min = amplitude;
                    }
                    if (max < amplitude)
                    {
                        max = amplitude;
                    }
                }
            }

            // get range
            if (min < 0 || max < 0)
                if (min < 0 && max < 0)
                    range = max - min;
                else
                    range = Math.Abs(min) + max;
            else
                range = max - min;

            // lock image
            PixelFormat format = canvas.PixelFormat;
            BitmapData data = canvas.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, format);
            int stride = data.Stride;
            int offset = stride - width * 4;

            try
            {
                unsafe
                {
                    //move rect to left
                    for (int x = 0; x < width - len * slide; x++)
                    {
                        byte* pixel = (byte*) data.Scan0.ToPointer();
                        pixel += 4*x;
                        for (int y = 0; y < height; y++, pixel += offset)
                        {
                            byte* pixelsrc = pixel + 4 * len * slide;

                            pixel[0] = pixelsrc[0];
                            pixel[1] = pixelsrc[1];
                            pixel[2] = pixelsrc[2];
                            pixel[3] = pixelsrc[3];
                            pixel += stride;

                        }

                    }

                    //fill the new data
                    for (int x = 0; x < len; x++)
                    {
                        double distance = ((double) ((double[]) _fftLeftSpect[len - x - 1]).Length*Fmax/SamplesPerSecond*
                                           2/(double) (height));
                        for (int i = 0; i < slide; i++)
                        {
                            byte* pixel = (byte*)data.Scan0.ToPointer();
                            pixel += stride - 4 * (i+1+x* slide);
                            for (int y = 0; y < height; y++, pixel += offset)
                            {

                                double amplitude =
                                    ((double[])_fftLeftSpect[len - x - 1])[(int)(distance * (height - 1 - y))];

                                int color = GetColor(min, max, range, amplitude);
                                if (color > 255) //最强db点
                                {

                                }
                                pixel[0] = (byte)color;
                                pixel[1] = (byte)0;
                                pixel[2] = (byte)color;
                                pixel[3] = (byte)color;
                                pixel += stride;
                            }
                        }
                    }

                    _fftLeftSpect.Clear();

                    Monitor.Exit(_fftLeftSpect);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            
            // unlock image
            canvas.UnlockBits(data);
            // Clean up
            offScreenDC.Dispose();
            ImageBox.Source = BitmapToImageSource(canvas);

        }


        /// <summary>
        /// Get color in the range of 0-255 for amplitude sample
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="range"></param>
        /// <param name="amplitude"></param>
        /// <returns></returns>
        private static int GetColor(double min, double max, double range, double amplitude)
        {
            double color;
            if (min != double.NegativeInfinity && min != double.MaxValue & max != double.PositiveInfinity && max != double.MinValue && range != 0)
            {
                if (min < 0 || max < 0)
                    if (min < 0 && max < 0)
                        color = (255 / range) * (Math.Abs(min) - Math.Abs(amplitude));
                    else
                        if (amplitude < 0)
                            color = (255 / range) * (Math.Abs(min) - Math.Abs(amplitude));
                        else
                            color = (255 / range) * (amplitude + Math.Abs(min));
                else
                    color = (255 / range) * (amplitude - min*2)*1.2 ;
            }
            else
                color = 0;
            if (color > 255)
                color = 255;
            if (color < 0)
                color = 0;
            return (int)color;
        }
        /*
        public void SetTitle(ref PictureBox picbox,string title)
        {
            Bitmap bitmap = new Bitmap(picbox.Width, picbox.Height);
            Graphics g = Graphics.FromImage(bitmap);
            SolidBrush sb = new SolidBrush(Color.Black);
            g.DrawString(title, new Font("Arial Regular", 20), sb, picbox.Width/2-20,5);
            picbox.Image = bitmap;
            g.Dispose();

        }
        //绘制Y轴上的刻度
        public void SetYAxis(ref PictureBox picbox)
        {
            Pen p1 = new Pen(Color.Black, 2);
            Pen p2 = new Pen(Color.Black, 1);
            SolidBrush sb = new SolidBrush(Color.Black);
            Bitmap bitmap = new Bitmap(picbox.Width, picbox.Height);
            Graphics g = Graphics.FromImage(bitmap);
            double scale = (double)Ymax * 2 / (picbox.Height/2);//给定的最大刻度与实际像素的比例关系
            //开始画时域
            //第一个刻度的两个端点
            int xl = 3, yl = picbox.Height - 1, xr = 6, yr = picbox.Height - 1;
            for (int j = 0; j < 9; j++)
            {

                g.DrawLine(p1, xl, yl - j * picbox.Height / 16, xr, yl - j * picbox.Height / 16);//刻度线
                if((j>0)&&(j<8))
                {
                    string tempy = (- Ymax + 0.25*j*Ymax).ToString("0");
                    g.DrawString(tempy, new Font("Arial Regular", 8), sb, xl + 5, yl - j * picbox.Height / 16 - 5);
                }
               
            }
            g.DrawString("smpl", new Font("Arial Regular", 8), sb, xl + 1, picbox.Height / 2 + 2);
            //开始画频域
            for (int j = 0; j < 10; j++)
            {
                yl = picbox.Height / 2 - 1;
                yr = picbox.Height / 2 - 1;
                g.DrawLine(p1, xl, yl - j * picbox.Height / 20, xr, yl - j * picbox.Height / 20);//刻度线
                if (j > 0) 
                {
                    string tempy = (j * Fmax / 10).ToString("0");
                    g.DrawString(tempy, new Font("Arial Regular", 8), sb, xl + 5, yl - j * picbox.Height / 20 - 6);
                }


            }
            g.DrawString("Hz", new Font("Arial Regular", 8), sb, xl + 1, 2);
            picbox.Image = bitmap;
            g.Dispose();

        }
        */
    }
}

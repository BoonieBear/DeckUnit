using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BoonieBear.DeckUnit.SysLogService;


namespace BoonieBear.DeckUnit.Utilities.Image
{
    public class ImageUtils 
    {
        private static ILogService _logger = LogService.GetLogger(typeof(ImageUtils));

        public static BitmapSource ConvertUIElementToBitmapSource(UIElement element)
        {
            var target = new RenderTargetBitmap((int)(element.RenderSize.Width), (int)(element.RenderSize.Height), 96, 96, PixelFormats.Pbgra32);
            var brush = new VisualBrush(element);
            var visual = new DrawingVisual();
            var drawingContext = visual.RenderOpen();
            drawingContext.DrawRectangle(brush, null, new Rect(new Point(0, 0), new Point(element.RenderSize.Width, element.RenderSize.Height)));
            drawingContext.Close();
            target.Render(visual);
            return target;
        }

        public static void SaveBitmapSource2File(string filename, BitmapSource image5)
        {
            try
            {
                if (filename == null)
                {
                    _logger.Warn("SaveBitmapSource2File： filename is null");
                    return;
                }
                if (image5 == null)
                {
                    _logger.Error(string.Format("Survey UI: Trying to saved a null image source from file {0}", filename));
                    return;
                }

                if (filename != string.Empty)
                {
                    using (FileStream stream5 = new FileStream(filename, FileMode.Create))
                    {
                        PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                        encoder5.Frames.Add(BitmapFrame.Create(image5));
                        encoder5.Save(stream5);
                        stream5.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("SaveBitmapSource2File", ex);
            }
        }

        public static System.Drawing.Bitmap ResizeImage(string fromFile, int maxWidth, int maxHeight, int quality = 100)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(fromFile);
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            System.Drawing.Bitmap newImage = new System.Drawing.Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            // Get an ImageCodecInfo object that represents the JPEG codec.
            System.Drawing.Imaging.ImageCodecInfo imageCodecInfo = ImageUtils.GetEncoderInfo(System.Drawing.Imaging.ImageFormat.Jpeg);

            // Create an Encoder object for the Quality parameter.
            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object. 
            System.Drawing.Imaging.EncoderParameters encoderParameters = new System.Drawing.Imaging.EncoderParameters(1);

            // Save the image as a JPEG file with quality level.
            System.Drawing.Imaging.EncoderParameter encoderParameter = new System.Drawing.Imaging.EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;
          //  newImage.Save(SavefilePath, imageCodecInfo, encoderParameters);
            return newImage;
        }

        /// <summary>
        /// Method to get encoder infor for given image format.
        /// </summary>
        /// <param name="format">Image format</param>
        /// <returns>image codec info.</returns>
        private static System.Drawing.Imaging.ImageCodecInfo GetEncoderInfo(System.Drawing.Imaging.ImageFormat format)
        {
            return System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

    }
}

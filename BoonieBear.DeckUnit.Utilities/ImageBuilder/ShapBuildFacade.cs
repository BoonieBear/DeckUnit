using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BoonieBear.DeckUnit.Utilities.FileLogger;
using BoonieBear.DeckUnit.Utilities.Image;

namespace BoonieBear.DeckUnit.Utilities.ImageBuilder
{


    public class ShapBuildFacade : INotifyPropertyChanged
    {
        private static ILogService _logger = LogService.GetLogger(typeof(ShapBuildFacade));
        private int _pixelWidth;
        private int _pixeHeight;
        private double _scale;
        public ShapBuildFacade(double width, double height, double scale)
        {
            _pixelWidth = (int)(width * scale);
            _pixeHeight = (int)(height * scale);
            _scale = scale;
            InitImageSource();
            GC.Collect();
        }

        #region propertys
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChagned(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private RenderTargetBitmap _imageSource;
        public RenderTargetBitmap ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                OnPropertyChagned("ImageSource");
            }
        }

        #endregion

        #region interface
        public void AddShap(ShapeData shape)
        {
            InitImageSource();
            DrawingContext drawingContext = _drawingVisual.RenderOpen();
            ShapeBulder builder = ShapBuilderFactory.Instance.GetShapBilder(shape.ShapType);
            Rect drawRect = DrawShape(shape, builder);
            builder.Drawing(drawingContext, drawRect);
            drawingContext.Close();
            if (ImageSource != null)
            {
                ImageSource.Render(_drawingVisual);
            }
        }

        public void ReDrawShaps(List<ShapeData> shaps)
        {
            this.Dispose();
            InitImageSource();
            DrawingContext drawingContext = _drawingVisual.RenderOpen();
            ShapeBuilderType[] typeSecquence = new ShapeBuilderType[4] 
            {
                ShapeBuilderType.RangeEllipse, //step1: draw all rangeindicator
                ShapeBuilderType.ArrowLine,    //step2: draw all dash ling with arrow
                ShapeBuilderType.Rectangle,    //step3： draw all autosampling point
                ShapeBuilderType.StartEllipse  //step4: draw start red ellipse
            };

            foreach (var type in typeSecquence)
            {
                for (int i = 0; i < shaps.Count;)
                {
                    ShapeData shape = shaps[i];
                    if (shape.ShapType != type)
                    {
                        i++;
                        continue;
                    }

                    ShapeBulder builder = ShapBuilderFactory.Instance.GetShapBilder(shape.ShapType);
                    Rect drawRect = DrawShape(shape, builder);
                    builder.Drawing(drawingContext, drawRect);
                    shaps.RemoveAt(i);
                }
            }
            drawingContext.Close();
            if (ImageSource != null)
            {
                ImageSource.Render(_drawingVisual);
            }
        }

        public void Dispose()
        {
            ImageSource = null;
        }
        #endregion

        #region helpers

        private void InitImageSource()
        {
            if (ImageSource == null)
            {
                GC.Collect();
                if (_pixelWidth > 0 && _pixeHeight > 0)
                {
                    ImageSource = new RenderTargetBitmap(_pixelWidth, _pixeHeight, 96, 96, PixelFormats.Default);
                }
            }
        }

        private Point CorrectPoint(Point point)
        {
            return new Point(CorrectPointX(point.X), CoorrectPointY(point.Y));
        }

        private int CorrectPointX(double x)
        {
            return Math.Max(0, Math.Min(ConvertToInt(x), _pixelWidth));
        }

        private int ConvertToInt(double dValue)
        {
            return (int)(dValue + 0.5);
        }

        private int CoorrectPointY(double y)
        {
            return Math.Max(0, Math.Min((int)y, _pixeHeight));
        }

        private DrawingVisual _drawingVisual = new DrawingVisual();
        private static readonly double RECTANGLE_SIZE = 8;
        private static readonly double STARTRADIUS = 5;
        //private static readonly double StartEllipse_WIDTH = 18;
        private Rect DrawShape(ShapeData shape, ShapeBulder builder)
        {
            Rect drawRect = new Rect(0, 0, 0, 0);
            switch (shape.ShapType)
            {
                case ShapeBuilderType.Rectangle:
                    drawRect = new Rect(CorrectPointX((shape.Position.X) * _scale - RECTANGLE_SIZE / 2),
                                        CoorrectPointY((shape.Position.Y) * _scale - RECTANGLE_SIZE / 2), 
                                        RECTANGLE_SIZE, 
                                        RECTANGLE_SIZE);
                    break;
                case ShapeBuilderType.ArrowLine:
                     drawRect = new Rect(shape.Start, shape.End);
                        ((ArrowLineBuilder)builder).Start = CorrectPoint(new Point((shape.Start.X) * _scale, shape.Start.Y * _scale));
                        ((ArrowLineBuilder)builder).End = CorrectPoint(new Point(shape.End.X * _scale, shape.End.Y * _scale));
                    break;
                case ShapeBuilderType.StartEllipse:
                    drawRect = new Rect(0, 0, 0, 0);
                     ((EllipseBuilder)builder).Position = new Point(CorrectPointX((shape.Position.X) * _scale),
                                                                    CoorrectPointY((shape.Position.Y) * _scale));
                     ((EllipseBuilder)builder).Radius = STARTRADIUS;
                    break;
                case ShapeBuilderType.RangeEllipse:
                    drawRect = new Rect(0,0,0,0);
                    ((GridLineEllipse)builder).Position = new Point(CorrectPointX((shape.Position.X) * _scale),
                                                                    CoorrectPointY((shape.Position.Y) * _scale));
                    ((GridLineEllipse)builder).Radius = ConvertToInt(shape.Radius * _scale);
                    ((GridLineEllipse)builder).ShapeBrush = shape.Brush;
                    ((GridLineEllipse)builder).PixelWidth = _pixelWidth;
                    ((GridLineEllipse)builder).PixelHeight = _pixeHeight;
                    break;
                default:
                    break;
            }
            return drawRect;
        }

        public void SaveImageFile(string filePath)
        {
            if (ImageSource != null)
            {
                ImageUtils.SaveBitmapSource2File(filePath, ImageSource);
                _logger.Debug(string.Format("SaveImageFile: Image file Path is {0}", filePath));
            }
        }
        #endregion

    }
}

using System.Windows.Media;

namespace BoonieBear.DeckUnit.Utilities.ImageBuilder
{
    public class ShapBuilderFactory
    {
        private ShapBuilderFactory() { }
 
        private static ShapBuilderFactory _Instance;
        public static ShapBuilderFactory Instance
        {
            get 
            {
                if (_Instance == null)
                {
                    _Instance = new ShapBuilderFactory();
                }
                return _Instance; 
            }
            
        }
         
        public ShapeBulder GetShapBilder(ShapeBuilderType type)
        {
            ShapeBulder builder = null;
            switch (type)
            {
                case ShapeBuilderType.Rectangle:
                    builder = RectangleShapBuilder;
                    break;
                case ShapeBuilderType.ArrowLine:
                    builder = ArrowLineShapBuilder;
                    break;
                case ShapeBuilderType.StartEllipse:
                    builder = StartEllipseShapeBuilder;
                    break; 
                case ShapeBuilderType.RangeEllipse:
                    builder = RangeEllipseShapeBuilder;
                    break;
                default:
                    break;
            }
            return builder;
        }

        private  RectangleBuilder _rectangleShapBuilder;
        private RectangleBuilder RectangleShapBuilder
        {
            get 
            {
                if (_rectangleShapBuilder == null)
                {
                    _rectangleShapBuilder = new RectangleBuilder();
                    _rectangleShapBuilder.ShapeBrush = Brushes.Blue;
                }
                return _rectangleShapBuilder;
            }
        }

        private ArrowLineBuilder CreateDefaultArrowLineBilder()
        {
            ArrowLineBuilder builder = new ArrowLineBuilder();
            builder.ShapeBrush = Brushes.Red;
            DashStyle dash = new DashStyle(new double[] {3,3}, 0);
            builder.DrawPen = new Pen() { Brush = Brushes.Gray, Thickness = 2, DashStyle = dash };
            builder.ArrowHeadWidth = 10;
            builder.ArrowHeadLength = 10;
            return builder;
        }

        private ArrowLineBuilder _ArrowLineShapBuilder;
        private ArrowLineBuilder ArrowLineShapBuilder
        {
            get 
            {
                if (_ArrowLineShapBuilder == null)
                {
                    _ArrowLineShapBuilder = CreateDefaultArrowLineBilder();
                }
                return _ArrowLineShapBuilder;
            }
        }

        //private ArrowLineBuilder _eraseArrowLineShapBuilder;
        //private ArrowLineBuilder EraseArrowLineShapBuilder
        //{
        //    get
        //    {
        //        if (_eraseArrowLineShapBuilder == null)
        //        {
        //            _eraseArrowLineShapBuilder = CreateDefaultArrowLineBilder(true);
        //            _eraseArrowLineShapBuilder.ShapeBrush = Brushes.Transparent;
        //        }
        //        return _eraseArrowLineShapBuilder;
        //    }
        //}


        private EllipseBuilder _StartEllipseShapeBuilder;
        private EllipseBuilder StartEllipseShapeBuilder
        {
            get
            {
                if (_StartEllipseShapeBuilder == null)
                {
                    _StartEllipseShapeBuilder = new EllipseBuilder();
                    _StartEllipseShapeBuilder.ShapeBrush = Brushes.Red;
                }
                return _StartEllipseShapeBuilder; 
            }
        }

        private GridLineEllipse _RangeEllipseShapeBuilder;
        public GridLineEllipse RangeEllipseShapeBuilder
        {
            get
            {
                if (_RangeEllipseShapeBuilder == null)
                {
                    _RangeEllipseShapeBuilder = new GridLineEllipse();
                    _RangeEllipseShapeBuilder.ShapeBrush = new SolidColorBrush(Color.FromRgb(1, 50, 187));
                    _RangeEllipseShapeBuilder.Space = 10;
                    _RangeEllipseShapeBuilder.DrawPen = new Pen() { Brush = Brushes.Gray, Thickness = 1 };
                }
                return _RangeEllipseShapeBuilder;
            }
        }
        //private StartEllipseBuilder _eraseStartEllipseShapeBuilder;
        //private StartEllipseBuilder EraseStartEllipseShapeBuilder
        //{
        //    get
        //    {
        //        if (_eraseStartEllipseShapeBuilder == null)
        //        {
        //            _eraseStartEllipseShapeBuilder = new StartEllipseBuilder();
        //            _eraseStartEllipseShapeBuilder.ShapeBrush = Brushes.Transparent;
        //        }
        //        return _eraseStartEllipseShapeBuilder;
        //    }
        //}
        
    }
}

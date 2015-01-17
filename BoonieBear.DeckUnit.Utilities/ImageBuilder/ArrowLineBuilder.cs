using System.Windows;
using System.Windows.Media;

namespace BoonieBear.DeckUnit.Utilities.ImageBuilder
{
    public class ArrowLineBuilder : ShapeBulder
    {
        private double _ArrowHeadLength;
        public double ArrowHeadLength
        {
            get { return _ArrowHeadLength; }
            set { _ArrowHeadLength = value; }
        }

        private Point _start;
        public Point Start
        {
            get { return _start; }
            set { _start = value; }
        }

        private Point _end;
        public Point End
        {
            get { return _end; }
            set { _end = value; }
        }
        
        
        private double _ArrowHeadWidth;
        public double ArrowHeadWidth
        {
            get { return _ArrowHeadWidth; }
            set { _ArrowHeadWidth = value; }
        }

        public override void Drawing(System.Windows.Media.DrawingContext drawingContext, Rect area)
        {
            drawingContext.DrawLine(DrawPen, Start, End);
            DrawLine(drawingContext, Start, End);
            drawingContext.DrawGeometry(ShapeBrush, null, MakeArrowGeometry(Start, End));
        }

        private Geometry MakeArrowGeometry(Point start, Point end)
        {
            GeometryGroup group = new GeometryGroup();
            Vector startDir = end - start;
            startDir.Normalize();
            Point basePoint = end - (startDir * ArrowHeadLength);
            Vector crossDir = new Vector(-startDir.Y, startDir.X);

            Point[] arrowHeadPoints = new Point[3];
            arrowHeadPoints[0] = end;
            arrowHeadPoints[1] = basePoint - (crossDir * (ArrowHeadWidth / 2));
            arrowHeadPoints[2] = basePoint + (crossDir * (ArrowHeadWidth / 2));
            PathFigure arrowHeadFig = new PathFigure();
            arrowHeadFig.IsClosed = true;
            arrowHeadFig.IsFilled = true;
            arrowHeadFig.StartPoint = arrowHeadPoints[0];
            arrowHeadFig.Segments.Add(new LineSegment(arrowHeadPoints[1], true));
            arrowHeadFig.Segments.Add(new LineSegment(arrowHeadPoints[2], true));
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(arrowHeadFig);
            group.Children.Add(pathGeometry);
            return group;
        }

        private void DrawLine(System.Windows.Media.DrawingContext drawingContext,Point start, Point end) 
        {
            Vector startDir = end - start;
            startDir.Normalize();
            Point endPoint = end - (startDir * ArrowHeadLength);
            Point startPoint = endPoint - (startDir * ArrowHeadLength);
            Pen pen = new Pen(ShapeBrush, DrawPen.Thickness);
            drawingContext.DrawLine(pen, startPoint, endPoint);
        }
        
    }
}

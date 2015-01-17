using System;
using System.Windows;
using System.Windows.Media;

namespace BoonieBear.DeckUnit.Utilities.ImageBuilder
{
    public class GridLineEllipse : EllipseBuilder
    {
        public int Space { get; set; }
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public override void Drawing(System.Windows.Media.DrawingContext drawingContext, System.Windows.Rect area)
        {
             Pen pen = new Pen(ShapeBrush, DrawPen.Thickness);
             //int Divisions = (int)(Radius / Space);
             Rect bound = GetGirdLineBound();
  
             int LineStartX =  bound.Left % Space == 0 ? (int)bound.Left  : ((int)(bound.Left / Space)+1)*Space;
             for (double x = LineStartX; x <= bound.Right; x+=Space)
             {
                 double y = Math.Sqrt(Radius * Radius - (Position.X-x)*(Position.X-x));
                 Point startPoint = new Point(x, Position.Y-y);
                 Point endPoint = new Point(x, Position.Y+y);
                 drawingContext.DrawLine(pen, startPoint, endPoint);
             }

             int LineStartY= bound.Top % Space == 0 ? (int)bound.Top : ((int)(bound.Top / Space) + 1) * Space;
             for (double y = LineStartY; y <= bound.Bottom; y += Space)
             {
                 double x = Math.Sqrt(Radius * Radius - (Position.Y - y) * (Position.Y - y));
                 Point startPoint = new Point(Position.X - x, y);
                 Point endPoint = new Point(Position.X + x, y);
                 drawingContext.DrawLine(pen, startPoint, endPoint);
             }
        }

        private Rect GetGirdLineBound()
        {
            double startX = (int)(Position.X - Radius);
            double startY = (int)(Position.Y - Radius);
            double endX = (int)(Position.X + Radius);
            double endY = (int)(Position.Y + Radius);
            double boundStartX = Math.Max(0, Math.Min(startX, PixelWidth));
            double boundStartY = Math.Max(0, Math.Min(startY, PixelHeight));
            double boundEndX = Math.Max(0, Math.Min(endX, PixelWidth));
            double boundEndY = Math.Max(0, Math.Min(endY, PixelHeight));

            Rect bound = new Rect(new Point(boundStartX, boundStartY), new Point(boundEndX, boundEndY));
            return bound;
        }
    }
}

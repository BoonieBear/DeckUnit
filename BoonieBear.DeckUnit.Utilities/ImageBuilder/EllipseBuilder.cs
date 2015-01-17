using System.Windows;
namespace BoonieBear.DeckUnit.Utilities.ImageBuilder
{
    public class EllipseBuilder : ShapeBulder
    {
        public Point Position { get; set; }
        public double Radius { get; set; }
        
        public override void Drawing(System.Windows.Media.DrawingContext drawingContext, System.Windows.Rect area)
        {
            drawingContext.DrawEllipse(ShapeBrush, DrawPen, Position, Radius, Radius);
        }
    }
}

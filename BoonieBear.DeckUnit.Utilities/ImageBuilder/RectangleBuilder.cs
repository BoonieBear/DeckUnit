using System.Windows;

namespace BoonieBear.DeckUnit.Utilities.ImageBuilder
{
    public class RectangleBuilder : ShapeBulder
    {
        public override void Drawing(System.Windows.Media.DrawingContext drawingContext, Rect area)
        {
            drawingContext.DrawRectangle(ShapeBrush, DrawPen, area);
        }
    }
}

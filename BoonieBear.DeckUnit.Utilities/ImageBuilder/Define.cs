using System.Windows;
using System.Windows.Media;

namespace BoonieBear.DeckUnit.Utilities.ImageBuilder
{
    public enum ShapeBuilderType
    {
        Rectangle,
        ArrowLine,
        StartEllipse,
        RangeEllipse,
    }

    public class ShapeData
    {
        public Point Position { get; set; }
        public ShapeBuilderType ShapType { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public double Radius { get; set; }
        public Brush Brush { get; set; }
    }
}

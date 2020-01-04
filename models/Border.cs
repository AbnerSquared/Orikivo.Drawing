namespace Orikivo.Drawing
{
    public class Border
    {
        public BorderEdge FillEdge { get; set; } = BorderEdge.Outside;
        public GammaColor Color { get; set; }
        public int Width { get; set; }
    }
}

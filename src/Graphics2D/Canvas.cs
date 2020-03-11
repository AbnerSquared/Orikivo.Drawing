namespace Orikivo.Drawing.Graphics2D
{
    public class Canvas
    {
        // individual pixels.
        public Grid<System.Drawing.Color> Pixels { get; set; }
        public int Width { get; }
        public int Height { get; }
        public Pen Pen { get; set; }

        public void PenDown() { }
        public void PenUp() { }
        public void Clear() { }
        
        public void Stamp(System.Drawing.Image image)
        { }
    }
}

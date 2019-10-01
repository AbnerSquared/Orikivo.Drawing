using System.Drawing;

namespace Orikivo.Poxel
{
    public class CharPointInfo
    {
        public CharPointInfo(char c, int x, int y, int width, int height, int xOffset = 0, int yOffset = 0)
        {
            Char = c;
            Pos = new Point(x, y);
            Size = new Size(width, height);
            Offset = new Offset(xOffset, yOffset);
        }

        public CharPointInfo(char c, Point p, Size s, Offset offset = null)
        {
            Char = c;
            Pos = p;
            Size = s;
            Offset = new Offset(0, 0);
        }

        public char Char { get; }
        public Point Pos { get; }
        public Size Size { get; }

        public Offset Offset { get; }
    }
}

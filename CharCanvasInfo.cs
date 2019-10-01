using System.Drawing;

namespace Orikivo.Poxel
{
    public class CharCanvasInfo
    {
        internal CharCanvasInfo(Bitmap sprite, char c, Point p, Size s, Offset offset = null)
        {
            Sprite = sprite;
            Char = c;
            if (c == '\n')
                IsNewline = true;
            Pos = p;
            Size = s;
            Offset = offset ?? Offset.Empty;

        }
        public bool IsNewline { get; } = false;
        // if specified, shows the sprite to display.
        public Bitmap Sprite { get; }

        // the character that this is written for
        public char Char { get; }

        // the position of where this character is placed in relation to the canvas
        public Point Pos { get; }

        // the size of the character
        public Size Size { get; }
        // the global offset used on the character.
        public Offset Offset { get; }
    }
}

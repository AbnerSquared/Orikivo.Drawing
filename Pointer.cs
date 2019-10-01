using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Poxel
{
    // used to help draw things
    public class Pointer
    {
        public Pointer(int leftPadding = 0, int topPadding = 0, int? maxWidth = null, int? maxHeight = null)
        {
            _xOffset = leftPadding;
            _yOffset = topPadding;
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;
            
        }

        private int _xOffset = 0;
        private int _yOffset = 0;
        private int? _maxWidth = null;
        private int? _maxHeight = null;

        // a list of all the times X was reset.
        public List<int> Rows { get; } = new List<int>();

        // the largest row
        public (int X, int Y) LastPos { get; private set; } = (0,0);
        public Point Pos => new Point(X, Y);
        public int Width => Rows.OrderByDescending(x => x).First();

        private int _height { get; set; } = 0;
        public int Height => _yOffset + _height;

        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public void MoveX(int len)
        {
            LastPos = (X, LastPos.Y);
            X += len;
            Console.WriteLine($"-- Shifted X by {len} --");
        }
        public void MoveY(int len)
        {
            LastPos = (LastPos.X, Y);
            Y += len;
            Console.WriteLine($"-- Shifted Y by {len} --");
            if (Y > _height)
                _height = Y;
        }

        public void ResetX()
        {
            Rows.Add(X);
            Console.WriteLine($"-- Row of length '{X}' added --");
            X = _xOffset;
        }

        public void ResetY()
        {
            Y = _yOffset;
        }

        public Size Size => new Size(Width, Height);

    }
}

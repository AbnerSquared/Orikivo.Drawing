﻿using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Drawing
{
    public class GradientLayer : DrawableLayer
    {
        public Dictionary<float, Color> Markers { get; set; } = new Dictionary<float, Color>();

        public GradientColorHandling ColorHandling { get; set; } = GradientColorHandling.Blend;
        public int Width { get; set; }
        public int Height { get; set; }
        public Direction Direction { get; set; } = Direction.Right;

        protected override Bitmap GetBaseImage()
            => ImageHelper.CreateGradient(Markers, Width, Height, Direction, ColorHandling);
    }
}

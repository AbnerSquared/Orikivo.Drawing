using System;
using System.Drawing;

namespace Orikivo.Drawing
{
    public class Frame : IDisposable
    {
        public Frame(Bitmap image, TimeSpan? length = null)
        {
            Image = image;
            Length = length;
        }

        private bool _disposed;
        public Bitmap Image { get; }
        public TimeSpan? Length { get; set; }

        public DrawableConfig Config { get; set; } // TODO: Apply DrawableConfig.

        public void Dispose()
        {
            if (!_disposed)
            {
                Image.Dispose();
                _disposed = true;
            }
        }
    }
}

using System;
using System.IO;
using Orikivo.Drawing.Encoding;

namespace Orikivo.Drawing
{
    public abstract class Animator : IDisposable
    {
        public abstract MemoryStream Compile(TimeSpan frameLength, Quality quality = Quality.Bpp8);
        public virtual void Dispose() { }
    }
}

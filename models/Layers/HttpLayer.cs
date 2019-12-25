using System;
using System.Drawing;

namespace Orikivo.Drawing
{
    /// <summary>
    /// An <see cref="DrawableLayer"/> that retrieves its source from an <see cref="Uri"/>.
    /// </summary>
    public class HttpLayer : DrawableLayer
    {
        public string Url { get; set; }
        protected override Bitmap GetBaseImage()
            => BitmapUtils.GetHttpImage(Url);
    }
}

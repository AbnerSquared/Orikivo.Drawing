using System.Drawing;

namespace Orikivo.Drawing
{
    public class DrawableConfig
    {
        public GammaColorMap Colors { get; set; }
        public Gamma? BackgroundColorIndex { get; set; }
        public float Opacity { get; set; }
        public Size Size { get; set; }
        public Padding Padding { get; set; }
    }
}

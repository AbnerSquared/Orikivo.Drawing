using System.Collections.Generic;

namespace Orikivo.Drawing
{
    public class PixelGraphicsConfig
    {
        public static PixelGraphicsConfig Default = new PixelGraphicsConfig
        {
            // TODO: Handle CharMap deserialization with JsonCharArrayConverter
            // TODO: Allow importing of fonts automatically from a directory
            FontDirectory = "../assets/fonts/",
            Colors = GammaColorMap.Default,
            CacheChars = true
        };

        public char[][][][] CharMap { get; set; } // Create default char map
        public List<FontFace> Fonts { get; set; } = new List<FontFace>();
        public GammaColorMap Colors { get; set; } = GammaColorMap.Default;
        public string FontDirectory { get; set; }
        public bool CacheChars { get; set; }
    }
}

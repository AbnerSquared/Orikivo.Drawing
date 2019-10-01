using System;
using System.Drawing;

namespace Orikivo.Poxel
{
    public class ObjectColorMap
    {
        // get default colors
        // public static ObjectColorMap Default => new ObjectColorMap();
        public static int Capacity = 8;
        public ObjectColorMap(Color[] colors)
        {
            if (colors.Length != Capacity)
                throw new Exception("In order to generate a proper color map, there must only be 8 specified colors.");
            Values = colors;
        }
        public Color[] Values { get; }
        
        public Color GetValue(ColorBrightness brightness)
            => Values[(int)brightness];
    }
}

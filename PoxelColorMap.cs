using System;
using System.Drawing;

namespace Orikivo.Poxel
{
    public class PoxelColorMap
    {
        public static int Capacity = 8;
        public PoxelColorMap(Color[] colors)
        {
            if (colors.Length != 8)
                throw new Exception("There must be 8 different colors.");
            Values = colors;
        }
        public Color[] Values;
        
        public Color GetValue(ColorBrightness brightness)
            => Values[(int)brightness];
    }
}

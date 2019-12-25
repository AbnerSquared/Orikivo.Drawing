using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Orikivo.Drawing
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Color32
    {
        public Color32(IntPtr pSourcePixel)
        {
            this = (Color32)Marshal.PtrToStructure(pSourcePixel, typeof(Color32));
        }

        [FieldOffset(0)]
        public byte Blue;

        [FieldOffset(1)]
        public byte Green;

        [FieldOffset(2)]
        public byte Red;

        [FieldOffset(3)]
        public byte Alpha;

        [FieldOffset(0)]
        public int Argb;

        public Color Color => Color.FromArgb(Alpha, Red, Green, Blue);
    }
}

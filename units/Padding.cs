namespace Orikivo.Poxel
{
    /// <summary>
    /// An object containing the definitions of the whitespace surrounding a Bitmap.
    /// </summary>
    public struct Padding
    {
        public static Padding FontDefault => new Padding(right: 1);
        public static Padding Empty => new Padding(0);

        public Padding(int lrtb)
        {
            Left = Right = Top = Bottom = lrtb;
        }

        public Padding(int left = 0, int right = 0, int top = 0, int bottom = 0)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }
    }
}

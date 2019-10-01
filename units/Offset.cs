namespace Orikivo.Poxel
{
    /// <summary>
    /// A property that overrides the origin point of a Bitmap.
    /// </summary>
    public struct Offset
    {
        public static Offset Empty => new Offset(0, 0);
        public Offset(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; } // how many pixels up or down the chars should be placed.
    }
}

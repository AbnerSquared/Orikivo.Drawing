namespace Orikivo.Poxel
{
    public class Offset
    {
        public static Offset Empty => new Offset(0, 0);
        internal Offset(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; } = 0;
        public int Y { get; set; } = 0; // how many pixels up or down the chars should be placed.
    }
}

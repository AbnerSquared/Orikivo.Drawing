namespace Orikivo.Poxel
{
    // free space surrounding the object.
    public class Padding
    {
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

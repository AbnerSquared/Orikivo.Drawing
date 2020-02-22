namespace Orikivo.Drawing
{
    public struct RegionF
    {
        public RegionF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static bool Contains(float x, float y, float width, float height, float u, float v)
            => u <= x && v <= y && u < x + width && v < y + height;

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
}

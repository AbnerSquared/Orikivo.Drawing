namespace Orikivo.Drawing
{
    public struct RegionF
    {
        public static bool Contains(float x, float y, float width, float height, float u, float v)
            => u <= x && v <= y && u < x + width && v < y + height;
    }
}

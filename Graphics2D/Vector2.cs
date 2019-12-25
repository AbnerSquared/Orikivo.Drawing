namespace Orikivo.Drawing
{
    public struct Vector2
    {
        public static Vector2 Zero = new Vector2(0.0f, 0.0f);
        public static Vector2 One = new Vector2(1.00f, 1.00f);

        public Vector2(float x = 0.00f, float y = 0.00f)
        {
            X = x;
            Y = y;
        }

        public float X { get; set; }
        public float Y { get; set; }
    }
}

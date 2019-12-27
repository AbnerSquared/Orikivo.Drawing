namespace Orikivo.Drawing.Graphics2D
{
    /// <summary>
    /// A transform object within a 2D space.
    /// </summary>
    public class FlatTransform
    {
        public static FlatTransform Default = new FlatTransform(Vector2.Zero, 0, Vector2.One);

        public FlatTransform(Vector2 position, float rotation, Vector2? scale = null)
        {
            Position = position;
            Rotation = rotation % 360;
            Scale = scale ?? Vector2.One;
        }

        public Vector2 Position { get; set; }

        public float Rotation { get; set; }

        public Vector2 Scale { get; set; }
    }
}

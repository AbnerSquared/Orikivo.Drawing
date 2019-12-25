using System.Drawing;

namespace Orikivo.Drawing
{
    public struct Transform2D
    {
        public static Transform2D Default = new Transform2D(PointF.Empty, 0, Vector2.One);
        public Transform2D(PointF position, float rotation, Vector2? scale = null)
        {
            Position = position;
            Rotation = rotation % 360;
            Scale = scale ?? Vector2.One;
        }

        public PointF Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
    }
}

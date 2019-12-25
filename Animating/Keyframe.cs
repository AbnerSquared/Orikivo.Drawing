using System.Drawing;

namespace Orikivo.Drawing
{
    // used to pinpoint a specific transformation on a 
    public struct Keyframe
    {
        public static Keyframe GetDefault(long tick) => new Keyframe(tick, 1.0f, null);

        public Keyframe(long tick, float opacity = 1.0f, Transform2D? transform = null)
        {
            Tick = tick;
            Opacity = opacity;
            Transform = transform ?? Transform2D.Default;
        }

        public Keyframe(long tick, float opacity, PointF position, float rotation, Vector2 scale)
        { 
            Tick = tick;
            Opacity = opacity;
            Transform = new Transform2D(position, rotation, scale);
        }

        public long Tick { get; }
        public Transform2D Transform { get; }
        public float Opacity { get; }

        public PointF Position => Transform.Position;

        public float Rotation => Transform.Rotation;

        public Vector2 Scale => Transform.Scale;
    }
}

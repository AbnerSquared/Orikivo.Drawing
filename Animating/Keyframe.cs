using Orikivo.Drawing.Graphics2D;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents a transform that applies to a <see cref="TimelineLayer"/> within a <see cref="TimelineAnimator"/>.
    /// </summary>
    public struct Keyframe
    {
        public static Keyframe GetDefault(long tick) => new Keyframe(tick, 1.0f, null);

        public Keyframe(long tick, float opacity = 1.0f, FlatTransform transform = null)
        {
            Tick = tick;
            Opacity = opacity;
            Transform = transform ?? FlatTransform.Default;
        }

        public Keyframe(long tick, float opacity, Vector2 position, float rotation, Vector2 scale)
        { 
            Tick = tick;
            Opacity = opacity;
            Transform = new FlatTransform(position, rotation, scale);
        }

        public long Tick { get; }
        public FlatTransform Transform { get; }
        public float Opacity { get; }

        public Vector2 Position => Transform.Position;

        public float Rotation => Transform.Rotation;

        public Vector2 Scale
        {
            get => Transform.Scale;
            set => Transform.Scale = value;
        }
    }
}

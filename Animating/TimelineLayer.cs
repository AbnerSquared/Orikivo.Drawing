using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Drawing
{
    public class TimelineLayer : IDisposable
    {
        private bool Disposed = false;
        public TimelineLayer(Bitmap image, List<Keyframe> keyframes, long startTick, long endTick, Keyframe? initialKeyframe = null)
        {
            Image = image;
            Keyframes = keyframes;
            StartTick = startTick;
            EndTick = endTick;
            InitialKeyframe = initialKeyframe ?? Keyframe.GetDefault(startTick);
        }

        public Bitmap Image { get; }
        // the range at which this object is visible
        public long StartTick { get; }
        public long EndTick { get; }
        public long Length => EndTick - StartTick;

        public IReadOnlyList<Keyframe> Keyframes { get; }
        public Keyframe InitialKeyframe { get; }

        private Keyframe GetLastKeyframe(long currentTick)
        {
            return currentTick > StartTick ? 
                Keyframes.Where(x => x.Tick < currentTick)
                .OrderBy(x => Math.Abs(x.Tick - currentTick))
                .First()
                : InitialKeyframe;
        }

        private Keyframe GetNextKeyframe(long currentTick)
        {
            return Keyframes.Where(x => x.Tick >= currentTick)
                .OrderBy(x => Math.Abs(x.Tick - currentTick))
                .First();
        }

        public Keyframe GetLayerKeyframe(long currentTick)
            => GetLayerKeyframe(GetLastKeyframe(currentTick), GetNextKeyframe(currentTick), currentTick);

        private Keyframe GetLayerKeyframe(Keyframe last, Keyframe next, long currentTick)
        {
            float progress = RangeF.Convert(StartTick, EndTick, 0.0f, 1.0f, currentTick);

            float currentOpacity = RangeF.Convert(0.0f, 1.0f, last.Opacity, next.Opacity, progress);
            
            float currentRotation = RangeF.Convert(0.0f, 1.0f, last.Rotation, next.Rotation, progress);
            
            PointF currentPosition = new PointF(RangeF.Convert(0.0f, 1.0f, last.Position.X,
                                                               next.Position.X, progress),
                                                RangeF.Convert(0.0f, 1.0f, last.Position.Y,
                                                               next.Position.Y, progress));
            
            Vector2 currentScale = new Vector2(RangeF.Convert(0.0f, 1.0f, last.Scale.X, next.Scale.X, progress),
                                               RangeF.Convert(0.0f, 1.0f, last.Scale.Y, next.Scale.Y, progress));

            Keyframe current = new Keyframe(currentTick, currentOpacity,
                currentPosition, currentRotation, currentScale);

            return current;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Image.Dispose();
                Disposed = true;
            }
        }
    }
}

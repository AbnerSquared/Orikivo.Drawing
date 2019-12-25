using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Orikivo.Drawing.Encoding;

namespace Orikivo.Drawing
{
    public class TimelineAnimator : IDisposable
    {
        private bool Disposed = false;
        // TicksToSeconds converter
        public long Ticks { get; set; }
        public Size Viewport { get; set; }
        
        public List<TimelineLayer> Layers { get; set; } = new List<TimelineLayer>();

        private List<Frame> CompileFrames()
        {
            List<Frame> frames = new List<Frame>();
            for (long t = 0; t < Ticks; t++)
            {
                Bitmap frame = new Bitmap(Viewport.Width, Viewport.Height);
                using (Graphics g = Graphics.FromImage(frame))
                {
                    // compile layer, draw to current frame.
                    for (int i = 0; i < Layers.Count; i++)
                    {
                        Keyframe keyframe = Layers[i].GetLayerKeyframe(t);
                        Bitmap layer = GraphicsUtils.ApplyTransform(Viewport, Layers[i].Image, keyframe.Transform, keyframe.Opacity);
                        GraphicsUtils.ClipAndDrawImage(g, layer, Point.Empty);
                    }
                }

                frames.Add(new Frame(frame));
            }

            return frames;
        }

        public MemoryStream Compile(TimeSpan frameLength, Quality quality = Quality.Bpp8)
        {
            if (Disposed)
                throw new ObjectDisposedException("Layers");

            List<Frame> frames = CompileFrames();

            MemoryStream animation = new MemoryStream();
            using (GifEncoder encoder = new GifEncoder(animation, Viewport))
            {
                encoder.FrameLength = frameLength;
                encoder.Quality = quality;

                foreach (Frame frame in frames)
                {
                    encoder.EncodeFrame(frame.Image, frameLength: frame.Length);
                }
            }
            animation.Position = 0;

            return animation;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                foreach (TimelineLayer layer in Layers)
                    layer.Dispose();

                Disposed = true;
            }
        }
    }
}

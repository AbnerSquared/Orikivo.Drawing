using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Orikivo.Drawing.Encoding;

namespace Orikivo.Drawing
{
    public class FrameAnimator : Animator
    {
        public double DefaultFrameLength { get; set; }
        public double? RepeatCount { get; set; }
        public Size Viewport { get; set; }
        public List<Frame> Frames { get; }

        public void UpdateFrame(int index, Frame frame)
        {
            Frames[index] = frame;
        }

        public void AddFrame(Frame frame)
        {
            Frames.Add(frame);
        }

        public void AddFrames(List<Frame> frames)
        {
            Frames.AddRange(frames);
        }

        public void RemoveFrame(int index)
        {
            Frames.RemoveAt(index);
        }

        public override MemoryStream Compile(TimeSpan frameLength, Quality quality = Quality.Bpp8)
        {
            MemoryStream animation = new MemoryStream();
            using (GifEncoder encoder = new GifEncoder(animation, Viewport))
            {
                encoder.FrameLength = frameLength;
                encoder.Quality = quality;

                foreach (Frame frame in Frames)
                {
                    encoder.EncodeFrame(frame.Image, frameLength: frame.Length);
                }
            }
            animation.Position = 0;

            return animation;
        }
        public override void Dispose() { }
    }
}

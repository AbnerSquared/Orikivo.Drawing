using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Orikivo.Drawing.Graphics3D;
using System.Drawing.Drawing2D;
using static System.MathF;

namespace Orikivo.Drawing
{

    public enum DegreeMode // for an extensive Math class
    {
        Radians = 1,
        Degrees = 2
    }
    public static class GraphicsUtils
    {
        private static readonly Size Bounds16_9 = new Size(400, 225); // 16:9
        private static readonly Size Bounds4_3 = new Size(400, 300); // 4:3
        private static readonly Size Bounds1_1 = new Size(300, 300); // 1:1
        private static readonly Size Bounds1_2 = new Size(400, 200); // 1:2
        private static readonly Size Bounds2_1 = new Size(150, 300); // 2:1 

        private static readonly Size Thumbs16_9 = new Size(80, 45); // 16:9
        private static readonly Size Thumbs4_3 = new Size(80, 60); // 4:3
        private static readonly Size Thumbs1_1 = new Size(80, 80); // 1:1
        private static readonly Size Thumbs1_2 = new Size(80, 40); // 1:2
        private static readonly Size Thumbs2_1 = new Size(40, 80); // 2:1

        //public static Bitmap ForceColors(Bitmap bmp, GammaColorMap colors)
        //{

        //}

        // Create a version that can read the bitmap.
        private static Size GetRotationSize(int oldWidth, int oldHeight, float angle)
        {
            float gamma = 90;
            float beta = 180 - angle - gamma;

            float c1 = oldHeight;

            float a1 = (c1 *
                Sin(Utils.Radians(angle)) /
                Sin(Utils.Radians(gamma)));

            float b1 = (c1 * 
                Sin(Utils.Radians(beta)) /
                Sin(Utils.Radians(gamma)));

            float c2 = oldWidth;

            float a2 = (c2 *
                Sin(Utils.Radians(angle)) /
                Sin(Utils.Radians(gamma)));

            float b2 = (c2 *
                Sin(Utils.Radians(beta)) /
                Sin(Utils.Radians(gamma)));

            int width = (int)Floor(b2 + a1);
            int height = (int)Floor(b1 + a2);

            return new Size(width, height);
        }

        public static Bitmap Rotate(Bitmap bmp, float angle)
        {
            Size rot = GetRotationSize(bmp.Width, bmp.Height, angle);
            Bitmap rotated = new Bitmap(rot.Width, rot.Height);
            using (Graphics g = Graphics.FromImage(rotated))
            {
                g.TranslateTransform(rotated.Width / 2, rotated.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-(rotated.Width / 2), -(rotated.Height / 2));
                g.DrawImage(bmp, new Point(
                    (rot.Width - bmp.Width) / 2,
                    (rot.Height - bmp.Height) / 2));
            }

            return rotated;
        }

        public static Bitmap Resize(Bitmap bmp, int width, int height)
        {
            Rectangle destination = new Rectangle(0, 0, width, height);
            Bitmap result = new Bitmap(width, height);

            result.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.None;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (ImageAttributes wraps = new ImageAttributes())
                {
                    wraps.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(bmp, destination, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, wraps);
                }
            }

            return result;
        }

        public static Bitmap Scale(Bitmap bmp, float widthScale, float heightScale)
            => Resize(bmp,
                (int)Floor(bmp.Width * widthScale),
                (int)Floor(bmp.Height * heightScale));

        public static Bitmap SetOpacity(Bitmap bmp, float opacity)
        {
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);

            using (Graphics g = Graphics.FromImage(result))
            {
                ColorMatrix m = new ColorMatrix();
                m.Matrix33 = opacity;

                ImageAttributes attributes = new ImageAttributes();

                attributes.SetColorMatrix(m,
                    ColorMatrixFlag.Default,
                    ColorAdjustType.Bitmap);

                g.DrawImage(bmp,
                    new Rectangle(0, 0,
                    result.Width,
                    result.Height),
                    0, 0, bmp.Width, bmp.Height,
                    GraphicsUnit.Pixel, attributes);
            }

            return result;
        }

        public static Bitmap ApplyTransform(Size viewport, Bitmap bmp, Transform2D transform, float opacity = 1.0f)
        {
            Bitmap result = new Bitmap(viewport.Width, viewport.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(result))
            {
                using (Bitmap edited = ApplyTransform(bmp, transform, opacity))
                {
                    // POSITION
                    Point position = Point.Truncate(transform.Position);
                    if (position.X > viewport.Width && position.Y > viewport.Height)
                    {
                        if (position.X < 0 || position.X + edited.Width > viewport.Width ||
                            position.Y < 0 || position.Y + edited.Height > viewport.Height)
                        {
                            Rectangle cropRect = ClampRectangle(Point.Empty,
                                                                viewport,
                                                                position,
                                                                edited.Size);

                            using (Bitmap crop = BitmapUtils.Crop(edited, cropRect))
                                ClipAndDrawImage(g, crop, position);
                        }
                        else
                            ClipAndDrawImage(g, edited, position);

                        // TODO: Create the generic color conversion into a GammaColorMap.
                    }
                    else
                        ClipAndDrawImage(g, edited, position);
                }
            }

            return result;
        }


        public static Bitmap ApplyTransform(Bitmap bmp, Transform2D transform, float opacity = 1.0f)
        {
            // SCALE
            using (Bitmap scaled = Scale(bmp, transform.Scale.X, transform.Scale.Y))
            {
                // ROTATE
                using (Bitmap rotated = Rotate(scaled, transform.Rotation))
                {
                    // OPACITY
                    using (Bitmap alpha = SetOpacity(rotated, opacity))
                    {
                        return alpha;
                    }
                }
            }
        }
        public static Bitmap CreateBitmap(Color color, int width, int height)
            => CreateBitmap(new Grid<Color>(width, height, color).Values);

        public static Bitmap CreateBitmap(Grid<GammaColor> colors)
            => CreateBitmap(colors.Cast<Color>().Values);

        public static Bitmap CreateBitmap(Color[,] colors)
        {
            int width = colors.GetLength(1);
            int height = colors.GetLength(0);

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            unsafe
            {
                BitmapData source = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                int bitsPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int pixelHeight = source.Height;
                int byteWidth = source.Width * bitsPerPixel; // is == width
                byte* ptr = (byte*)source.Scan0;

                // NOTE: a for statement, all done at once.
                Parallel.For(0, pixelHeight, y =>
                {
                    byte* row = ptr + (y * source.Stride);
                    for (int x = 0; x < byteWidth; x = x + bitsPerPixel)
                    {
                        Color color = colors[y, x / bitsPerPixel];

                        // A
                        // row[x + 3] = color.A;

                        // R
                        row[x + 2] = color.R;

                        // G
                        row[x + 1] = color.G;

                        // B
                        row[x] = color.B;
                    }
                });

                bmp.UnlockBits(source);
            }

            return bmp;
        }

        // This converts every single pixel on this image to the colors specified.
        // Why not just do a simple force create color map
        // from to
        private static byte[] GetBpp8Bytes(byte[] bytes, int width, int height, GammaColorMap colors, ref int stride)
        {
            byte[] bpp8 = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                int input = y * stride;
                int output = y * width;

                for (int x = 0; x < width; x++)
                {
                    byte r = bytes[input + 2];
                    byte g = bytes[input + 1];
                    byte b = bytes[input];
                    GammaColor color = new GammaColor(r, g, b);
                    bpp8[output] = (byte) GammaColor.ClosestMatchAt(color, colors);
                    input += 4;
                    output++;
                }
            }

            stride = width;
            return bpp8;
        }

        /// <summary>
        /// Returns the size of the specified <see cref="ImageRatio"/> and <see cref="DiscordMedia"/>.
        /// </summary>
        public static Size GetRatioDims(ImageRatio ratio, DiscordMedia media)
        {
            bool isThumb = media == DiscordMedia.Thumbnail;
            return ratio switch
            {
                ImageRatio.Widescreen => isThumb ? Thumbs16_9 : Bounds16_9,
                ImageRatio.Wide => isThumb ? Thumbs2_1 : Bounds2_1,
                ImageRatio.Rectangle => isThumb ? Thumbs4_3 : Bounds4_3,
                ImageRatio.Square => isThumb ? Thumbs1_1 : Bounds1_1,
                ImageRatio.Tall => isThumb ? Thumbs1_2 : Bounds1_2,
                _ => throw new ArgumentException("The ratio type specified is not a valid ratio.")
            };
        }

        public static void ClipAndDrawImage(Graphics graphics, Bitmap image, System.Drawing.Point point)
            => ClipAndDrawImage(graphics, image, new Rectangle(point, image.Size));

        public static void ClipAndDrawImage(Graphics graphics, Bitmap image, Rectangle clip)
        {
            graphics.SetClip(clip);
            graphics.DrawImage(image, clip);
            graphics.ResetClip();
        }

        internal static CharMapIndex GetCharIndex(char c, char[][][][] map)
        {
            (int? I, int? X, int? Y) = new ValueTuple<int?, int?, int?>();
            
            foreach(char[][][] page in map)
            {
                if (page.Any(x => x.Any(y => y.Contains(c))))
                {
                    I = map.ToList().IndexOf(page);
                    foreach (char[][] row in page)
                    {
                        if (row.Any(x => x.Contains(c)))
                        {
                            Y = page.ToList().IndexOf(row);
                            foreach(char[] item in row)
                            {
                                if (item.Contains(c))
                                {
                                    X = row.ToList().IndexOf(item);
                                    return CharMapIndex.FromResult(c, I, X, Y);
                                }
                            }
                        }
                    }
                }
            }

            return CharMapIndex.FromResult(c, I, X, Y);
        }

        public static int ClampLength(int origin, int offset, int innerLength, int length)
        {
            return innerLength - (Math.Abs(Math.Min(origin + offset, 0)) + Math.Max(offset + innerLength - length, 0));
        }

        private static int GetClampedPoint(int origin, int offset)
        {
            return offset < 0 ? Math.Abs(origin + offset) : 0;
        }

        public static Rectangle ClampRectangle(System.Drawing.Point origin, Size size, System.Drawing.Point offset, Size innerSize)
        {
                                                                                         // origin = (0, 0)
                                                                                         // offset = (-2, -5)
                                                                                         // innerSize = (8, 7)
                                                                                         // size = (12, 12)

            int x = GetClampedPoint(origin.X, offset.X);                                 // -2 < 0 ? abs(0 + (-2)) : 0
                                                                                         // abs(0 + (-2))
                                                                                         // abs(0 - 2)
                                                                                         // abs(-2)
                                                                                         // 2

            int y = GetClampedPoint(origin.Y, offset.Y);                                 // -5 < 0 ? abs(0 + (-5)) : 0
                                                                                         // abs(0 + (-5))
                                                                                         // abs(0 - 5)
                                                                                         // abs(-5)
                                                                                         // 5

            int width = ClampLength(origin.X, offset.X, innerSize.Width, size.Width);    // origin = 0, offset = -2, innerLength = 8, length = 12
                                                                                         // 8 - (Math.Abs(Math.Min(0 + (-2), 0)) + Math.Max(-2 + 8 - 12, 0))
                                                                                         // 8 - (Math.Abs(Math.Min(-2, 0)) + Math.Max(6 - 12, 0))
                                                                                         // 8 - (Math.Abs(-2) + Math.Max(-6, 0))
                                                                                         // 8 - (2 + (0))
                                                                                         // 8 - 2 + 0
                                                                                         // 6

            int height = ClampLength(origin.Y, offset.Y, innerSize.Height, size.Height); // origin = 0, offset = -5, innerLength = 7, length = 12
                                                                                         // 7 - (abs(min(0 + (-5), 0)) + max(-5 + 7 - 12, 0))
                                                                                         // 7 - (abs(min(-5, 0)) + max(2 - 12, 0))
                                                                                         // 7 - (abs(-5) + max(-10, 0)
                                                                                         // 7 - (5 + 0)
                                                                                         // 7 - 5
                                                                                         // 2

            return new Rectangle(x, y, width, height); // (2, 5, 6, 2)
        }
    }
}

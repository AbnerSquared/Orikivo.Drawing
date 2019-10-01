using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Orikivo.Poxel
{
    public static class BitmapUtils
    {
        public static ColorMap[] CreateColorMaps(Color[] fromColors, Color[] toColors)
        {
            if (fromColors == null || toColors == null)
                throw new Exception("A color array is null.");
            if (fromColors.Length != toColors.Length)
                throw new Exception("The 'from' and 'to' color arrays must be the same length.");

            (Color from, Color to)[] colors = { };
            for (int i = 0; i < fromColors.Length; i++)
                colors[i] = (fromColors[i], toColors[i]);

            return CreateColorMaps(colors);
        }

        public static ColorMap[] CreateColorMaps(params (Color from, Color to)[] colors)
        {
            if (!(colors?.Length > 0))
                throw new Exception("At least one color map value must be specified.");
            
            ColorMap[] colorMaps = { };
            for (int i = 0; i < colors.Length; i++)
            {
                ColorMap colorMap = new ColorMap();
                colorMap.OldColor = colors[i].from;
                colorMap.NewColor = colors[i].to;
                colorMaps[i] = colorMap;
            }

            return colorMaps;
        }

        // remember that a discord image container has a border radius of 3px
        // to prevent pixels from looking wonky, add a minimum padding of 4px

        public static Bitmap Remap(Bitmap bmp, ColorMap[] mapTable)
        {
            using (Bitmap canvas = new Bitmap(bmp.Width, bmp.Height))
            {
                using (Graphics graphics = Graphics.FromImage(canvas))
                {

                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetRemapTable(mapTable, ColorAdjustType.Bitmap);
                    graphics.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height),
                        0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                }
                return canvas;
            }
        }

        // figure out a way to figure out the length outside of .GetPixel()

        public static int GetNonEmptyWidth(Bitmap bmp)
        {
            // the full length.
            int len = bmp.Width;
            // instead of storing the binary values, store the length
            int[] rows = { }; // true if full width
            for (int y = 0; y < bmp.Height; y++)
            {
                int xLen = 0;
                for (int x = 0; x < len; x++)
                {
                    if (bmp.GetPixel(x, y).A == 0)
                        continue;
                    xLen = x + 1; // index starts at 0
                }
                rows[y] = xLen;
            }

            // return the row's highest number
            return rows.OrderByDescending(x => x).First();
        }

        public static Bitmap GetHttpImage(string url)
        {
            //string toPath = Directory.CreateDirectory(@".\tmp\").FullName;
            //string path = toPath + Path.GetFileName(toPath);
            //using (WebClient webClient = new WebClient()) // download version.
            //    webClient.DownloadFile(new Uri(url), path);

            using (WebClient webClient = new WebClient())
                using (Stream stream = webClient.OpenRead(url))
                    return new Bitmap(stream);
        }

        // creates an outline around the bitmap given, with an option to include the bitmap it was drawn on or not
        public static Bitmap DrawOutline(Bitmap bmp, int width, Color color, bool drawOnBitmap = true)
        {
            List<(int px, int py)> validPoints = new List<(int px, int py)>();
            for (int y = 0; y < bmp.Height; y++)
            {
                Console.WriteLine("\n\n-- Peeking at new Y --\n\n");
                for (int x = 0; x < bmp.Width; x++)
                {
                    Console.WriteLine("\n\n-- Peeking at new X --\n\n");
                    // you want to ignore all empty pixels
                    if (bmp.GetPixel(x, y).A == 0)
                        continue;

                    
                    int minX = x - width;
                    int minY = y - width;
                    
                    int maxX = x + width;
                    int maxY = y + width;

                    for (int m = minX; m <= maxX; m++)
                    {
                        Console.WriteLine($"-- x:{m} --");
                        // ignore all values out of bounds
                        if (m < 0)
                            continue;
                        if (m > bmp.Width) // we know the rest of the values outside of the width will be bad
                            break;
                        for (int n = minY; n <= maxY; n++)
                        {
                            if (n < 0)
                                continue;
                            if (n > bmp.Height)
                                break;

                            if (bmp.GetPixel(m, n).A == 0)
                                if (!validPoints.Contains((m, n)))
                                {
                                    Console.WriteLine($"-- ({m},{n}) --");
                                    validPoints.Add((m, n));
                                }
                        }

                    }
                }
            }

            if (!drawOnBitmap)
            {
                // you can't return a using(), as it's disposed before it can save
                Bitmap tmp = new Bitmap(bmp.Width, bmp.Height);
                foreach ((int px, int py) in validPoints)
                {
                    Console.WriteLine("-- Placing point --");
                    tmp.SetPixel(px, py, color);
                }

                    return tmp;
            }

            foreach ((int px, int py) in validPoints)
                bmp.SetPixel(px, py, color); // set the pixels of all valid pixels to the color specified.

            return bmp;
        }

        private static ImageCodecInfo GetCodecInfo(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;
            return null; // no matching codec found.
        }

        public static void Save(Bitmap bmp, string path, ImageFormat format)
        {
            using (bmp)
            {
                Encoder encoder = Encoder.Quality;
                EncoderParameter[] args = { new EncoderParameter(encoder, 100) };
                EncoderParameters parameters = new EncoderParameters(args.Length);
                for (int i = 0; i < args.Length; i++)
                    parameters.Param[i] = args[i];
                bmp.Save(path, GetCodecInfo(format), parameters); // bmp can be disposed, as it's simply being stored
            }
        }
    }
}

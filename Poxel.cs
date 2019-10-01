using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Orikivo.Poxel
{

    // internal version of Graphics
    public class Poxel : IDisposable
    {
        public Poxel()
        {
            // auto-load the array map into this.    
        }
        private CharMap _map;
        public List<FontFace> Fonts { get; private set; } = new List<FontFace>();

        // imports a new font face from a path. must point to a .json file.
        public bool ImportFont(string jsonPath)
        {
            throw new NotImplementedException();
        }


        public static Bitmap GetChar(char c, FontFace font, bool useNonEmptyWidth = true)
        {
            // derive from array map
            (int i, int x, int y) = PoxelUtils.GetCharIndex(c, new CharMap().Value/*_map.Value*/);

            using (Bitmap bmp = new Bitmap(font.SheetUrls[i]))
            {
                Rectangle crop = new Rectangle(font.Width * x, font.Height * y, font.Width, font.Height);
                Bitmap tmp = bmp.Clone(crop, bmp.PixelFormat);
                if (!font.IsMonospace && useNonEmptyWidth)
                {
                    crop.Width = BitmapUtils.GetNonEmptyWidth(tmp);
                    tmp.Dispose(); // no longer needed in this area, as the crop will be a new area.
                    return bmp.Clone(crop, bmp.PixelFormat);
                }

                return tmp;
            }
        }

        public Bitmap DrawString(string text, FontFace font, Color color, OutlineProperties outlineProperties = null, CanvasProperties canvasProperties = null)
        {
            StringCanvas canvas = new StringCanvas(font, text, canvasProperties?.Padding, useNonEmptyWidth: true);

            Bitmap bmp = new Bitmap(canvas.Width, canvas.Height);
            int leftPadding = canvasProperties?.Padding?.Left ?? 0;
            Point pointer = new Point(leftPadding, canvasProperties?.Padding?.Top ?? 0);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                foreach(CharCanvasInfo c in canvas.Chars)
                {
                    if (c.IsNewline)
                    {
                        pointer.Y += c.Size.Height;
                        pointer.X = leftPadding;
                        continue;
                    }

                    pointer.X = c.Pos.X;
                    pointer.Y = c.Pos.Y;
                    
                    if (c.Sprite != null)
                    {
                        using (Bitmap sprite = c.Sprite)
                        {
                            Rectangle rect = new Rectangle(pointer, c.Size);
                            graphics.SetClip(rect);
                            graphics.DrawImage(sprite, rect);
                            graphics.ResetClip();
                        }
                    }

                    pointer.X += c.Size.Width;

                    /* handle offsets here */
                }
            }

            // recolor bitmap here, and handle outlines here.
            return bmp;
        }

        // gets rid of all rendered objects
        public void Dispose()
        {

        }
    }
}

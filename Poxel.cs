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

        public static Bitmap Crop(string localPath, int width, int height, CropPoint pos, bool disposeOnCrop = true)
            => Crop(localPath, pos.IsGridCrop ? pos.X * width : pos.Y, pos.IsGridCrop ? pos.Y * height : pos.Y, width, height, disposeOnCrop);

        // generic cropp
        public static Bitmap Crop(string localPath, int x, int y, int width, int height, bool disposeOnCrop = true)
            => Crop(new Bitmap(localPath), x, y, width, height, disposeOnCrop);

        public static Bitmap Crop(Bitmap bmp, int x, int y, int width, int height, bool disposeOnCrop = false)
            => Crop(bmp, new Rectangle(x, y, width, height), disposeOnCrop);

        public static Bitmap Crop(Bitmap bmp, Rectangle crop, bool disposeOnCrop = false)
        {
            Bitmap tmp = bmp.Clone(crop, bmp.PixelFormat);
            if (disposeOnCrop)
                bmp.Dispose();
            return tmp;
        }

        // gets the sprite of a character for a font face.
        internal static Bitmap GetChar(char c, FontFace font, bool useNonEmptyWidth = true)
        {
            // catch invalid characters here.
            if (CharEmptyInfo.IsEmptyChar(c) || c == '\n')
                return null;
            // derive from array map
            (int i, int x, int y) = PoxelUtils.GetCharIndex(c, new CharMap().Value/*_map.Value*/);

            using (Bitmap bmp = new Bitmap(font.SheetUrls[i]))
            {
                Rectangle crop = new Rectangle(font.Width * x, font.Height * y, font.Width, font.Height);
                Bitmap tmp = Crop(bmp, crop);
                if (!font.IsMonospace && useNonEmptyWidth)
                {
                    crop.Width = BitmapUtils.GetNonEmptyWidth(tmp);
                    tmp.Dispose(); // no longer needed in this area, as the crop will be a new area.
                    return Crop(bmp, crop);
                }

                return tmp;
            }
        }

        // draws an image, ensuring that it's placed correctly with a clip
        private static void DrawImage(Graphics graphics, Bitmap bmp, Rectangle clip)
        {
            graphics.SetClip(clip); // sets the boundaries to ensure pixel by pixel perfection.
            graphics.DrawImage(bmp, clip);
            graphics.ResetClip(); // removes the boundaries set when drawing the image
        }

        // prevent normal drawing objects from being used here.
        // poxel only manipulates with Sprite, Sheet, CroppedSprite, FontFace, PoxelObject
        // ObjectColorMap, OutlineProperties, CanvasProperties, StringCanvas, EmbedMediaRatio
        // EmbedMediaType, and ColorBrightness
        // TO_ADD: other custom components, like BarType
        public Bitmap DrawString(string content, FontFace font, Color color, OutlineProperties outlineProperties = null, CanvasProperties options = null)
        {
            // the string canvas gathers all of the information about where each character goes, alongside their sprites, if any.
            // this also determines the size of the bitmap that will be drawn.
            StringCanvas canvas = new StringCanvas(content, font, options?.Padding, useNonEmptyWidth: true, extendOnOffset: options?.ExtendOnOffset ?? false);

            Bitmap bmp = new Bitmap(canvas.Width, canvas.Height); // creates an empty drawing surface to work with.
            Point pointer = new Point(canvas.Padding.Left, canvas.Padding.Top);
            int yOffset = 0; // the largest y offset in place.
            bool hasOffset = false; // is set to true if any of the characters have a y offset.
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                // TO_DO: create an auto line break if the next char to be placed goes outside of the maximum width.
                foreach(CharCanvasInfo c in canvas.Chars)
                {
                    if (c.IsNewline)
                    {
                        if (options?.ExtendOnOffset ?? false && hasOffset)
                        {
                            pointer.Y += yOffset;
                            hasOffset = false;
                            yOffset = 0; // resets the largest offset from the previous row.
                        }
                        // the height already calculates for font height and padding.
                        pointer.Y += c.Size.Height; // a new line has been met, therefore breaking the current row, moving it down.
                        // this is where any y offsets would also be set if the row is stretched on offsets.
                        pointer.X = canvas.Padding.Left;
                        continue;
                    }

                    pointer.X = c.Pos.X; // sets the pointer to the position specified by the character.
                    pointer.Y = c.Pos.Y;

                    /* handle offsets here: you want to put the cursor in the right spot before placing the image. */
                    // this also handles if offsets stretch out the next line.
                    if (c.Offset.Y > 0)
                    {
                        hasOffset = true;
                        pointer.Y += c.Offset.Y;
                        if (c.Offset.Y > yOffset)
                            yOffset = c.Offset.Y; // sets the new largest offset.
                    }

                    if (c.Sprite != null)
                        using (Bitmap sprite = c.Sprite) // if the sprite exists, use it to place and dispose.
                            DrawImage(graphics, sprite, new Rectangle(pointer, c.Size));

                    pointer.X += c.Size.Width; // this already accounts for width/padding.
                    if (c.Offset.Y > 0) // this catches negative offsets..?
                        pointer.Y -= c.Offset.Y; // just in case there was an offset.
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

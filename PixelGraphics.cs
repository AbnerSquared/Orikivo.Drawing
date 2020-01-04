using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Orikivo.Drawing
{
    /// <summary>
    /// A custom <see cref="Graphics"/> class that supports pixelated imagery.
    /// </summary>
    public class PixelGraphics : IDisposable // LAYER MASKING, OUTLINES
    {
        private readonly Graphics _graphics;
        private static readonly string _defaultFontDirectory = "../assets/fonts/";
        private readonly bool _cacheable = true;

        private Image _image;


        public PixelGraphics(int width, int height, PixelFormat pixelFormat = PixelFormat.Format32bppArgb, PixelGraphicsConfig config = null)
        {
            _image = new Bitmap(width, height, pixelFormat);
            config ??= PixelGraphicsConfig.Default;
            CharMap = config.CharMap;
            Colors = config.Colors;
        }
        public PixelGraphics(Image image, PixelGraphicsConfig config = null)
        {
            _image = image;
            config ??= PixelGraphicsConfig.Default;
            CharMap = config.CharMap;
            Colors = config.Colors;   
        }

        public PixelGraphics(PixelGraphicsConfig config = null)
        {
            config ??= PixelGraphicsConfig.Default;
            CharMap = config.CharMap;
            Colors = config.Colors;
        }

        private char[][][][] CharMap { get; }
        private string FontDirectory { get; }
        public GammaColorMap Colors { get; set; }

        private Dictionary<char, Bitmap> CharCache { get; set; } = new Dictionary<char, Bitmap>();
        
        public CanvasOptions DefaultOptions { get; set; } = null;

        public List<FontFace> Fonts { get; private set; } = new List<FontFace>();
        public FontFace CurrentFont => Fonts[_currentFontIndex];

        private int _currentFontIndex = 0;
        public int CurrentFontIndex
        {
            get => _currentFontIndex;
            set
            {
                if (value >= Fonts.Count || value == 0)
                    return;

                _currentFontIndex = value;
            }
        }

        public void SetFont(FontFace font)
        {
            ImportFont(font);
            CurrentFontIndex = Fonts.IndexOf(font);
        }

        public void ImportFont(string path)
            => ImportFont(FontFace.FromPath(path));

        public void ImportFont(FontFace font)
        {
            if (!Fonts.Contains(font))
                Fonts.Add(font);
        }

        internal Bitmap GetRawChar(char c, FontFace font = null)
        {
            font ??= CurrentFont;
            
            if (EmptyCharInfo.IsEmptyChar(c) || c == '\n')
                return null;

            CharMapIndex i = GraphicsUtils.GetCharIndex(c, CharMap);

            if (!i.IsSuccess || !font.SheetUrls.Keys.Contains(i.Page))
            {
                // if (!font.HideBadUnicode) TODO: This should return the UNKNOWN unicode sprite.
                //    return new Bitmap(); 
                // else
                return null;
            }

            using (Bitmap bmp = new Bitmap($"{_defaultFontDirectory}{font.SheetUrls[i.Page]}")) // TODO: Handle path assignment
            {
                int x = font.CharWidth * i.Column;
                int y = font.CharHeight * i.Row;

                Rectangle crop = new Rectangle(x, y, font.CharWidth, font.CharHeight);
                Bitmap tmp = BitmapHandler.Crop(bmp, crop);

                return tmp;
            }
        }

        internal Bitmap GetChar(char c, FontFace font = null, bool useNonEmptyWidth = true)
        {
            font ??= CurrentFont;

            if (_cacheable)
                if (CharCache.ContainsKey(c))
                    return CharCache[c];

            Bitmap bmp = GetRawChar(c, font);

            if (bmp == null)
                return bmp;

            if (!font.IsMonospace && useNonEmptyWidth) // NOTE: I don't even think this is needed.
            {
                // might be too taxing
                return BitmapHandler.Crop(bmp, new Rectangle(0, 0, BitmapHandler.GetNonEmptyWidth(bmp), font.CharHeight));
            }
            

            if (_cacheable)
                CharCache[c] = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);

            return bmp;
        }

        private Dictionary<char, Bitmap> GetChars(string content, FontFace font = null, bool useNonEmptyWidth = true)
        {
            font ??= CurrentFont;
            char[] chars = content.ToCharArray();

            if (!(chars?.Length > 0))
                throw new Exception("One char must be specified at minimum.");

            chars = chars.Where(x => !EmptyCharInfo.IsEmptyChar(x) && x != '\n').ToArray();

            List<char> existingChars = new List<char>();

            foreach (char c in chars)
            {
                if (existingChars.Contains(c))
                    continue;

                existingChars.Add(c);
            }

            chars = existingChars.ToArray();

            Dictionary<char, Bitmap> charMap = new Dictionary<char, Bitmap>();

            for (int i = 0; i < chars.Length; i++)
            {
                if (charMap.ContainsKey(chars[i]))
                    continue;


                /* Safe Dispose if null
                 using (Bitmap tmp = GetChar(chars[i], font, useNonEmptyWidth))
                {
                    if (tmp != null)
                        charMap[chars[i]] = tmp.Clone(new Rectangle(Point.Empty, tmp.Size), tmp.PixelFormat);
                }
                 
                 
                 */
                charMap[chars[i]] = GetChar(chars[i], font, useNonEmptyWidth);
            }

            return charMap;
        }

        // make two versions, one with rendered sprites, and one without.
        // store the sprites and length at the same time to reduce redraw time.
        // make .MaxHeight .MaxWidth .Width .Height
        // if Width is specified, the canvas will be that width regardless
        // if .MaxWidth is specified instead, the canvas can expand up to that width.
        // TODO: Merge GetChars() and CreateText() together.
        // TODO: Scrap AutoWidth, and simply use IsMonospace
        private TextBox CreateText(Dictionary<char, Bitmap> spriteMap, string content, FontFace font, Padding? textPadding = null, int? maxWidth = null, int? maxHeight = null, bool useNonEmptyWidth = true, bool extendOnOffset = false)
        {
            bool autoWidth = useNonEmptyWidth && !font.IsMonospace;
            Padding padding = textPadding ?? Padding.Empty;


            if (!extendOnOffset)
                extendOnOffset = font.Customs?.Any(x => x.Offset != null) ?? extendOnOffset;

            Pointer cursor = new Pointer(maxWidth: maxWidth, maxHeight: maxHeight);

            List<char> chars = content.ToList();
            List<CharObject> charObjects = new List<CharObject>();

            int cursorHeight = font.Padding.Height + font.CharHeight;
            int yMaxOffset = 0;
            int charIndex = 0;

            foreach (char c in chars)
            {
                Padding charPadding = font.Padding;

                int? spriteWidth = spriteMap.ContainsKey(c) ? spriteMap[c]?.Width : null;

                int drawWidth = font.GetCharWidth(c);

                if (autoWidth && spriteMap.ContainsKey(c))
                    drawWidth = spriteMap[c]?.Width ?? drawWidth;

                int cursorWidth = font.Padding.Width + drawWidth;

                // LINE BREAKS
                if (c == '\n')
                {
                    charObjects.Add(new CharObject(
                        null,
                        c,
                        new Point(padding.Left + cursor.X, padding.Top + cursor.Y),
                        new Size(0, font.CharHeight)));

                    cursor.ResetX();
                    cursor.MoveY(cursorHeight + 1);
                    charIndex++;
                    continue;
                }

                // EMPTY CHARS
                if (EmptyCharInfo.IsEmptyChar(c))
                {
                    int emptyWidth = autoWidth ? font.GetEmptyWidth(c) : font.CharWidth + font.Padding.Width;

                    charObjects.Add(new CharObject(
                        null,
                        c,
                        new Point(padding.Left + cursor.X, padding.Top + cursor.Y),
                        new Size(emptyWidth, font.CharHeight)));

                    cursor.MoveX(emptyWidth);
                    charIndex++;
                    continue;
                }

                yMaxOffset += Math.Max(font.GetCharOffset(c).Y, 0);

                bool hasCharBefore = chars.TryGetElementAt(charIndex - 1, out char beforeChar);
                bool hasCharAfter = chars.TryGetElementAt(charIndex + 1, out char afterChar);

                if (autoWidth)
                {
                    if (charIndex == 0)
                    {
                        cursorWidth -= font.Padding.Left;
                        charPadding.Left = 0;
                    }
                    else if (hasCharBefore)
                    {
                        cursorWidth -= font.Padding.Left;
                        charPadding.Left = 0;
                    }

                    if (charIndex == chars.Count - 1)
                    {
                        cursorWidth -= font.Padding.Right;
                        charPadding.Right = 0;
                    }
                    else if (hasCharAfter)
                    {
                        if (afterChar == '\n' || EmptyCharInfo.IsEmptyChar(afterChar))
                        {
                            cursorWidth -= font.Padding.Right;
                            charPadding.Right = 0;
                        }
                    }
                }

                if (spriteMap[c] != null)
                {
                    charObjects.Add(
                        new CharObject(
                            spriteMap[c],
                            c,
                            new Point(padding.Left + cursor.X, padding.Top + cursor.Y),
                            new Size(drawWidth, font.CharHeight),
                            font.Padding,
                            font.GetCharOffset(c)));

                    cursor.MoveX(cursorWidth);
                }
                charIndex++;
            }

            int height = cursor.Height + font.CharHeight;

            if (extendOnOffset)
                height += yMaxOffset;  // if extending on offsets, add it to the total height.

            return new TextBox(content, padding, cursor.Width, height, charObjects);
        }

        public Bitmap DrawString(string content, Color color, CanvasOptions options = null)
            => DrawString(content, CurrentFont, color, options);

        public Bitmap DrawString(string content, CanvasOptions options = null)
            => DrawString(content, CurrentFont, Colors?[Gamma.Max] ?? GammaColor.GammaGreen, options);

        public Bitmap DrawString(string content, FontFace font, CanvasOptions options = null)
            => DrawString(content, font, Colors?[Gamma.Max] ?? GammaColor.GammaGreen, options);
        public Bitmap DrawString(string content, FontFace font, Color color, CanvasOptions options = null)
        {
            options ??= DefaultOptions;
            TextBox text = CreateText(GetChars(content, font, options?.UseNonEmptyWidth ?? true), content, font, options?.Padding,
                useNonEmptyWidth: options?.UseNonEmptyWidth ?? true, extendOnOffset: options?.ExtendOnOffset ?? false);

            Bitmap bmp = new Bitmap(text.BitmapWidth, text.BitmapHeight);

            if (options?.BackgroundColor.HasValue ?? false)
                bmp = BitmapHandler.Fill(bmp, options.BackgroundColor.Value);

            Point pointer = new Point(text.Padding.Left, text.Padding.Top);

            int yOffset = 0; // the largest y offset in place.

            bool hasOffset = false; // is set to true if any of the characters have a y offset.

            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                // TO_DO: create an auto line break if the next char to be placed goes outside of the maximum width.
                foreach (CharObject c in text.Chars)
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
                        pointer.Y += c.Size.Height; // + 1: Pads the bottom with a graceful pixel.

                        // this is where any y offsets would also be set if the row is stretched on offsets.
                        pointer.X = text.Padding.Left;

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
                    {
                        using (Bitmap sprite = c.Sprite) // if the sprite exists, use it to place and dispose.
                            GraphicsUtils.ClipAndDrawImage(graphics, sprite, new Rectangle(pointer, c.Size));
                    }

                    pointer.X += c.Size.Width + c.Padding.Right; // this already accounts for width/padding.

                    if (c.Offset.Y > 0) // this catches negative offsets..?
                        pointer.Y -= c.Offset.Y; // just in case there was an offset.
                }
            }

            // recolor bitmap here, and handle outlines here.

            bmp = BitmapHandler.SetColorMaps(bmp, BitmapHandler.CreateColorMaps((Color.White, color)));
            return bmp;
        }

        public Bitmap DrawSolid(GammaColor color, int width, int height)
        {
            return GraphicsUtils.CreateBitmap(new Grid<Color>(width, height, color).Values); //BitmapHandler.Fill(new Bitmap(width, height, PixelFormat.Format32bppArgb), color);
        }

        // blending between two colors.
        // THIS CAN BE DONE.
        //public void DrawGradient(GammaColor from, GammaColor to, int width, int height, AngleF angle)

        // float is from 0.00f to 1.00f, where 0.50 is the midpoint of the specified angle.
        //public void DrawGradient(Dictionary<float, GammaColor> colorKeyframes, int width, int height, AngleF angle)

        //public void DrawGradient(GammaColorMap colors, int width, int height, AngleF angle)
        
        //public void DrawLine(Point from, Point to, int thickness, GammaColor color)

        public Bitmap DrawFillable(GammaColor background, GammaColor foreground, int width, int height, float progress, AngleF angle)
        {
            float a = RangeF.Convert(0.0f, 1.0f, 0.0f, width, progress); // x fill

            // TODO: account for direction, using a 2D rotation matrix???
            Bitmap result = new Bitmap(width, height);

            // TODO: Utilize Grid<Color> to create the fillable, due to the angle markers.
            using (Graphics g = Graphics.FromImage(result))
            {
                int fillWidth = (int)Math.Floor(a);

                Rectangle fillClip = new Rectangle(0, 0, fillWidth, height);
                using (SolidBrush fore = new SolidBrush(foreground))
                {
                    g.SetClip(fillClip);
                    g.FillRectangle(fore, fillClip);
                    g.ResetClip();
                }

                Rectangle emptyClip = new Rectangle(fillWidth, 0, width - fillWidth, height);
                using (SolidBrush back = new SolidBrush(background))
                {
                    g.SetClip(emptyClip);
                    g.FillRectangle(back, emptyClip);
                    g.ResetClip();
                }
            }

            return result;
        }

        // gets rid of all rendered objects
        public void Dispose()
        {

        }
    }
}

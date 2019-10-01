using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Poxel
{
    // keeps track of the specified lengths of each character for a string to be written in a specified font
    public class StringCanvas //: IDisposable
    {
        // make two versions, one with rendered sprites, and one without.
        // store the sprites and length at the same time to reduce redraw time.
        public StringCanvas(FontFace font, string text, Padding padding = null, int? maxWidth = null, int? maxHeight = null, bool useNonEmptyWidth = false, bool extendOnOffset = false)
        {
            int charHeight = font.Padding.Top + font.Height + font.Padding.Bottom;
            CharSpriteMap sprites = new CharSpriteMap(font, text, useNonEmptyWidth);
            CharLengthMap charWidths = (useNonEmptyWidth && !font.IsMonospace) ?
                new CharLengthMap(sprites.Values.Select(x => (x.c, x.bmp.Width)).ToArray()) : null; // used if the text allows for it
            int defaultCharWidth = font.Padding.Left + font.Width + font.Padding.Right;
            int charWidth = defaultCharWidth;
            List<char> chars = text.ToList();
            int yOffset = 0;
            Pointer cur = new Pointer(maxWidth: maxWidth, maxHeight: maxHeight);
            List<CharCanvasInfo> pointInfo = new List<CharCanvasInfo>();
            foreach (char c in chars)
            {
                charWidth = (useNonEmptyWidth && !font.IsMonospace) ? font.Padding.Left + charWidths[c] + font.Padding.Right : defaultCharWidth;
                // handle newline
                if (c == '\n')
                {
                    pointInfo.Add(new CharCanvasInfo(null, c, new Point((padding?.Left ?? 0) + cur.Pos.X, (padding?.Top ?? 0) + cur.Pos.Y), new Size(0, charHeight))); // CharCanvasInfo.IsNewline, CharCanvasInfo.Sprite, CharCanvasInfo.Pos, CharCanvasInfo.Size
                    cur.ResetX();
                    // handle char offsets.
                    if (!extendOnOffset)
                        yOffset = 0;

                    cur.MoveY(charHeight + yOffset);
                    yOffset = 0;
                    continue;
                }

                // handle empty chars
                if (font.EmptyChars[c] != null)
                {
                    pointInfo.Add(new CharCanvasInfo(null, c, new Point((padding?.Left ?? 0) + cur.Pos.X, (padding?.Top ?? 0) + cur.Pos.Y), new Size(font.EmptyChars[c].Length, charHeight)));
                    cur.MoveX(font.EmptyChars[c].Length);
                    continue;
                }
                else if (CharEmptyInfo.IsEmptyChar(c))
                    throw new Exception("A char is missing a specified empty width.");

                // if there's a y offset that's greater than 0, capture.
                yOffset = (extendOnOffset && font.Offsets[c] != null) ? Math.Max(font.Offsets[c].Offset.Y, 0) : 0;

                int charIndex = chars.IndexOf(c);

                if (charIndex == 0 || chars.TryGetElementAt(charIndex - 1) == '\n') // if this is the first char in a row or the first char.
                    charWidth -= font.Padding.Left;

                if (charIndex == chars.Count - 1 || chars.TryGetElementAt(charIndex + 1) == '\n') // if this is the last char in a row or the last char
                    charWidth -= font.Padding.Right;

                pointInfo.Add(new CharCanvasInfo(sprites[c], c,
                    new Point((padding?.Left ?? 0) + cur.Pos.X, (padding?.Top ?? 0) + cur.Pos.Y),
                    new Size(charWidth, charHeight),
                    font.Offsets[c]?.Offset));
                // otherwise, the char is simply normal.
                cur.MoveX(charWidth); // ignore padding set before if the first char, or after if the last char.
            }

            Width = (padding?.Left ?? 0) + cur.Width + (padding?.Right ?? 0);
            Height = (padding?.Top ?? 0) +  cur.Height + (padding?.Bottom ?? 0);
            Chars = pointInfo;
        }

        public bool Disposed { get; private set; } = false;

        public int Width { get; }
        public int Height { get; }

        public List<CharCanvasInfo> Chars { get; }
        /*
        public void Dispose()
        {
            if (Disposed)
                return;
            CharSprites.Dispose();
        }
        */
    }
}

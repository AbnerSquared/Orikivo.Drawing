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
        // make .MaxHeight .MaxWidth .Width .Height
        // if Width is specified, the canvas will be that width regardless
        // if .MaxWidth is specified instead, the canvas can expand up to that width.
        public StringCanvas(string content, FontFace font, Padding? canvasPadding = null, int? maxWidth = null, int? maxHeight = null, bool useNonEmptyWidth = true, bool extendOnOffset = false)
        {
            Padding padding = canvasPadding ?? Padding.Empty;
            int charHeight = font.Padding.Top + font.Height + font.Padding.Bottom;
            CharSpriteMap sprites = new CharSpriteMap(content, font, useNonEmptyWidth);
            CharLengthMap charWidths = (useNonEmptyWidth && !font.IsMonospace) ?
                new CharLengthMap(sprites.Values.Select(x => (x.c, x.bmp.Width)).ToArray()) : null; // used if the text allows for it
            int defaultCharWidth = font.Padding.Left + font.Width + font.Padding.Right;
            int charWidth = defaultCharWidth;
            List<char> chars = content.ToList();
            int yOffset = 0;
            Pointer cur = new Pointer(maxWidth: maxWidth, maxHeight: maxHeight);
            List<CharCanvasInfo> pointInfo = new List<CharCanvasInfo>();
            foreach (char c in chars)
            {
                charWidth = (useNonEmptyWidth && !font.IsMonospace) ? font.Padding.Left + charWidths[c] + font.Padding.Right : defaultCharWidth;
                // handle newline
                if (c == '\n')
                {
                    pointInfo.Add(new CharCanvasInfo(null, c, new Point(padding.Left + cur.Pos.X, padding.Top + cur.Pos.Y), new Size(0, charHeight))); // CharCanvasInfo.IsNewline, CharCanvasInfo.Sprite, CharCanvasInfo.Pos, CharCanvasInfo.Size
                    cur.ResetX();
                    // offsets are now handled in Poxel.DrawString()
                    // handle total accumulative y offset to determine width.

                    cur.MoveY(charHeight);
                    continue;
                }

                // handle empty chars
                if (font.Empties[c] != null)
                {
                    pointInfo.Add(new CharCanvasInfo(null, c, new Point(padding.Left + cur.Pos.X, padding.Top + cur.Pos.Y), new Size(font.Empties[c].Length, charHeight)));
                    cur.MoveX(font.Empties[c].Length);
                    continue;
                }
                else if (CharEmptyInfo.IsEmptyChar(c))
                    throw new Exception("A char is missing a specified empty width.");

                // if there's a y offset that's greater than 0, capture that.
                yOffset += Math.Max(font.Offsets[c].Offset.Y, 0);

                int charIndex = chars.IndexOf(c);

                if (charIndex == 0 || chars.TryGetElementAt(charIndex - 1) == '\n') // if this is the first char in a row or the first char.
                    charWidth -= font.Padding.Left;

                if (charIndex == chars.Count - 1 || chars.TryGetElementAt(charIndex + 1) == '\n') // if this is the last char in a row or the last char
                    charWidth -= font.Padding.Right;

                pointInfo.Add(new CharCanvasInfo(sprites[c], c,
                    new Point(padding.Left + cur.Pos.X, padding.Top + cur.Pos.Y),
                    new Size(charWidth, charHeight),
                    font.Offsets[c].Offset));
                // otherwise, the char is simply normal.
                cur.MoveX(charWidth); // ignore padding set before if the first char, or after if the last char.
            }

            Padding = padding;
            Width = padding.Left + cur.Width + padding.Right;
            
            Height = padding.Top +  cur.Height + padding.Bottom;
            if (extendOnOffset) // you want to make sure to calculate the offset
                Height += yOffset; // if extending on offsets, add it to the total height.
            Chars = pointInfo;
        }

        public bool Disposed { get; private set; } = false;

        public Padding Padding { get; }
        public int Width { get; } // this includes the padding.
        public int Height { get; } // this includes the padding.

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

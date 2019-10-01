﻿using System;
using System.Drawing;
using System.Linq;

namespace Orikivo.Poxel
{
    public class CharSpriteMap : IDisposable
    {
        public CharSpriteMap(string content, FontFace font, bool useNonEmptyWidth = true) : this(useNonEmptyWidth, font, content.ToCharArray()) { }
        private CharSpriteMap(bool useNonEmptyWidth, FontFace font, params char[] chars)
        {
            (char c, Bitmap bmp)[] charMap = { };
            if (!(chars?.Length > 0))
                throw new Exception("One char must be specified at minimum.");

            // removes all instances of characters that aren't drawn onto a map.
            chars = chars.Where(x => !(CharEmptyInfo.IsEmptyChar(x) || x == '\n')).ToArray();
            for (int i = 0; i < chars.Length; i++)
                    charMap[i] = (chars[i], Poxel.GetChar(chars[i], font));

            Values = charMap;
        }
        public (char c, Bitmap bmp)[] Values { get; }

        public bool Disposed { get; private set; } = false;

        public Bitmap this[char c]
        {
            get
            {
                if (Disposed) // you can't get a disposed bitmap
                    return null;
                try
                {
                    return Values.First(x => x.c == c).bmp;
                }
                catch (ArgumentNullException) { return null; }
            }
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            foreach ((char c, Bitmap bmp) in Values)
                bmp.Dispose();

            Disposed = true;
        }
    }
}

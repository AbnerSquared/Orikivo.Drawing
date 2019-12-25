﻿using System.Collections.Generic;

namespace Orikivo.Drawing
{
    /// <summary>
    /// A constructor for <see cref="FontFace"/> used to create custom fonts.
    /// </summary>
    public class FontFaceBuilder
    {
        public Unit Ppu { get; set; }
        public Padding Padding { get; set; }
        public Dictionary<int, string> SheetUrls { get; set; }
        public List<EmptyCharInfo> Empties { get; set; }
        public List<CustomCharInfo> Customs { get; set; }
        public bool IsUnicodeSupported { get; set; }
        public bool IsMonospace { get; set; }
        public bool HideBadUnicode { get; set; }
    }
}

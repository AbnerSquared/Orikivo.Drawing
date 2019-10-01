using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Orikivo.Poxel
{
    // char[][][] CharMap ; used to label and identify all valid chars that can be used on orikivo's poxel engine.
    public class FontFace
    {
        internal FontFace(Unit ppu, FontTag tags, Dictionary<int, string> sheetUrls, EmptyCharContainer empties, Padding? padding = null, OffsetCharContainer offsets = null)
        {
            Ppu = ppu;
            Padding = padding ?? Padding.FontDefault;
            IsUnicodeSupported = tags.HasFlag(FontTag.UnicodeSupported);
            IsMonospace = tags.HasFlag(FontTag.Monospace);
            SheetUrls = sheetUrls;
            Offsets = offsets ?? OffsetCharContainer.Empty;
            Empties = empties;
        }

        public Unit Ppu { get; }
        public Padding Padding { get; } // the padding to use for each character.
        public int Width => Ppu.Width;
        public int Height => Ppu.Height;
        public bool IsUnicodeSupported { get; }
        public bool IsMonospace { get; } // if the font face is cropped based on whitespace
        public Dictionary<int, string> SheetUrls { get; } // focus on font types later
        public OffsetCharContainer Offsets { get; } // a dictionary of defined offsets for a character when being placed.
        public EmptyCharContainer Empties { get; } // a dictionary that handles what to do with empty chars.
    }
}

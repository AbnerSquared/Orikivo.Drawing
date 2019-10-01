using System;
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
        public static Padding DefaultPadding => new Padding(right: 1);
        public Unit Ppu { get; }
        // the padding to use for each character.
        public Padding Padding { get; }

        public int Width => Ppu.Width;
        public int Height => Ppu.Height;

        public bool IsUnicodeSupported { get; }

        // if the font face is cropped based on whitespace
        public bool IsMonospace { get; }

        // index, path
        public Dictionary<int, string> SheetUrls { get; } // focus on font types later
        public OffsetCharContainer Offsets { get; } // a dictionary of defined offsets for a character when being placed.
        public EmptyCharContainer EmptyChars { get; } // a dictionary that handles what to do with empty chars.
    }

    public interface ICharContainer<T> where T : ICharInfo
    {
        List<T> Values { get; }
        T this[char c] { get; }
    }

    public class OffsetCharContainer : ICharContainer<CharOffsetInfo>
    {
        public List<CharOffsetInfo> Values { get; }
        public CharOffsetInfo this[char c]
        {
            get
            {
                try
                {
                    return Values.First(x => x.ContainsChar(c));
                }
                catch(ArgumentNullException)
                {
                    return null;
                }
            }
        }
    }

    public class EmptyCharContainer : ICharContainer<CharEmptyInfo>
    {
        public List<CharEmptyInfo> Values { get; }
        public CharEmptyInfo this[char c]
        {
            get
            {
                try {
                    // make a catch for multiple results
                    return Values.First(x => x.ContainsChar(c));
                }
                catch(ArgumentNullException)
                {
                    return null;
                }
            }
        }
    }

    public class CharEmptyInfo : ICharInfo
    {
        public static List<CharEmptyInfo> DefaultEmpties
            => new List<CharEmptyInfo> { new CharEmptyInfo(1, '​'), new CharEmptyInfo(4, ' ') };

        public static bool IsEmptyChar(char c)
            => _emptyChars.Contains(c);

        // the only types of empty chars that are allowed.
        private static char[] _emptyChars =
        {
            '​' /* zero-width space */,
            ' ' /* default space */,   
        };
        public CharEmptyInfo(int len, params char[] chars)
        {
            if (!(chars?.Length > 0))
                throw new Exception("One char must be specified at minimum.");
            if (chars.Any(x => !(_emptyChars.Contains(x))))
                throw new Exception("The char given is not a valid empty char.");

            Chars = chars;
            Length = len;
        }
        public char[] Chars { get; }
        public int Length { get; }

        public bool ContainsChar(char c)
            => Chars.Contains(c);
    }

    public interface ICharInfo
    {
        char[] Chars { get; }
        bool ContainsChar(char c);
    }

    public class CharOffsetInfo : ICharInfo
    {
        public CharOffsetInfo(Offset offset, params char[] chars)
        {
            if (!(chars?.Length > 0))
                throw new Exception("One char must be specified at minimum.");

            Chars = chars;
            Offset = offset;
        }

        public char[] Chars { get; }
        public Offset Offset { get; }

        public bool ContainsChar(char c)
            => Chars.Contains(c);
    }

}

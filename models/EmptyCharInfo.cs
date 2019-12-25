using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Drawing
{
    public class EmptyCharInfo
    {
        public static List<EmptyCharInfo> DefaultEmpties
            => new List<EmptyCharInfo> { new EmptyCharInfo(1, '​'), new EmptyCharInfo(4, ' ', '⠀') };

        public static bool IsEmptyChar(char c)
            => _emptyChars.Contains(c);

        // These are all of the valid empty characters.
        private static readonly char[] _emptyChars =
        {
            '​', /* Zero-Width Space */
            ' ', /* Space */
            '⠀' /* Braille Empty */
        };

        [JsonConstructor]
        public EmptyCharInfo(char[] chars, int width)
        {
            Chars = chars;
            Width = width;
        }

        public EmptyCharInfo(int width, params char[] chars)
        {
            if (!(chars.Length > 0))
                throw new Exception("At least one Char has to be specified.");

            if (chars.Any(x => !IsEmptyChar(x)))
                throw new Exception($"One of the Char values given is not an empty character.");

            Chars = chars;
            Width = width;
        }

        [JsonProperty("chars")]
        public char[] Chars { get; }

        [JsonProperty("width")]
        public int Width { get; }
    }
}

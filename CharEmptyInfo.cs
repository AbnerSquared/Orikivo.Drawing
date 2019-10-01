using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Poxel
{
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

}

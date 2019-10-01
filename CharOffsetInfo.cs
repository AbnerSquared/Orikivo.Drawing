using System;
using System.Linq;

namespace Orikivo.Poxel
{
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

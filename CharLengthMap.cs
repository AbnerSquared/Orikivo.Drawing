using System;

namespace Orikivo.Poxel
{
    public class CharLengthMap
    {
        public CharLengthMap(params (char c, int len)[] values)
        {
            if (!(values?.Length > 0))
                throw new Exception("There must at least be one specified value.");
            Values = values;
        }

        public (char c, int len)[] Values { get; }
        public int this[char c]
        {
            get
            {
                return Values.First(x => x.c == c).len;
            }
        }

        public bool ContainsChar(char c)
            => Values.Any(x => x.c == c);
    }
}

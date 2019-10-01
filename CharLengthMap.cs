using System;
using System.Linq;

namespace Orikivo.Poxel
{
    public class CharLengthMap
    {
        public CharLengthMap(params (char Char, int Length)[] values)
        {
            if (!(values?.Length > 0))
                throw new Exception("There must at least be one specified value.");
            for (int i = 0; i < values.Length; i++)
                Values[i] = new CharPair<int>(values[i].Char, values[i].Length);
        }

        public CharPair<int>[] Values { get; }
        public int this[char c]
        {
            get
            {
                return Values.First(x => x.Char == c).Value;
            }
        }

        public bool ContainsChar(char c)
            => Values.Any(x => x.Char == c);
    }
}

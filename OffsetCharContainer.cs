using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Poxel
{
    public class OffsetCharContainer : ICharContainer<CharOffsetInfo>
    {
        public static OffsetCharContainer Empty => new OffsetCharContainer();
        internal OffsetCharContainer(params CharOffsetInfo[] offsets)
        {
            Values = new List<CharOffsetInfo>();
            if (offsets != null)
                for (int i = 0; i < offsets.Length; i++)
                    Values.Add(offsets[i]);
        }

        // maybe create a default offset here for unset characters.
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
                    // if empty, just return an empty offset for that character.
                    return new CharOffsetInfo(Offset.Empty, c);
                }
            }
        }
    }
}

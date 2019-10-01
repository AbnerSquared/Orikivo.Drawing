using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Poxel
{
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
}

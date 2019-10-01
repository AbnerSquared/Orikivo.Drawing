using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Poxel
{
    public static class ListExtensions
    {
        public static T TryGetElementAt<T>(this List<T> list, int index)
        {
            if (list.Count - index >= 1)
                return list.ElementAt(index);
            return default;
        }
    }
}

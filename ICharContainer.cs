using System.Collections.Generic;

namespace Orikivo.Poxel
{
    public interface ICharContainer<T> where T : ICharInfo
    {
        List<T> Values { get; }
        T this[char c] { get; }
    }
}

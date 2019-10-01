namespace Orikivo.Poxel
{
    public interface ICharMap<T>
    {
        (char, T)[] Values { get; }
    }
}

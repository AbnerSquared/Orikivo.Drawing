namespace Orikivo.Poxel
{
    public interface ICharPair<T>
    {
        char Char { get; }
        T Value { get; }
    }
}

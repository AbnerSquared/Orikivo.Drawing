namespace Orikivo.Poxel
{
    public interface ICharInfo
    {
        char[] Chars { get; }
        bool ContainsChar(char c);
    }

}

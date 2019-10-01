namespace Orikivo.Poxel
{
    public class CharPair<T> : ICharPair<T>
    {
        internal CharPair(char c, T value)
        {
            // catch null chars
            Char = c;
            Value = value;
        }
        public char Char { get; }
        public T Value { get; }
    }
}

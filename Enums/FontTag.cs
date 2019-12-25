namespace Orikivo.Drawing
{
    /// <summary>
    /// A collection of modifiers that a <see cref="FontFace"/> might have.
    /// </summary>
    [System.Flags]
    public enum FontTag
    {
        /// <summary>
        /// If the font characters are consistently the same width.
        /// </summary>
        Monospace = 1,

        /// <summary>
        /// If the font has support for Unicode icons.
        /// </summary>
        UnicodeSupported = 2
    }
}

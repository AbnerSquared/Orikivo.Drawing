using System;

namespace Orikivo.Poxel
{
    /// <summary>
    /// A collection of modifiers that a font face might have.
    /// </summary>
    [Flags]
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

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents a collection of modifiers that a <see cref="FontFace"/> might have.
    /// </summary>
    [System.Flags]
    public enum FontTag
    {
        /// <summary>
        /// Marks a <see cref="FontFace"/> as monospace, which ensures that all characters are equal in width.
        /// </summary>
        Monospace = 1,

        /// <summary>
        /// Marks a <see cref="FontFace"/> as Unicode supported.
        /// </summary>
        UnicodeSupported = 2,

        // https://en.wikipedia.org/wiki/Subscript_and_superscript#/media/File:Sub_super_num_dem.svg
        /// <summary>
        /// Marks a <see cref="FontFace"/> to support subscript and superscript text.
        /// </summary>
        SubscriptSupported = 3,

        /// <summary>
        /// Marks a <see cref="FontFace"/> to be required as functional, as it is a root font.
        /// </summary>
        Required = 4 // NOTE: This might be set in PixelGraphics, or anything so, using FontDictionary + orikos.json
            // This way, the required fonts are hardcoded.
    }
}

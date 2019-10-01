namespace Orikivo.Poxel
{
    // the reason ColorBrightness focuses off of a static 8 colors
    // is to make sure that everything transitions
    // it also simplifies creating templates, as each color can be set.
    public enum ColorBrightness
    {
        /// <summary>
        /// The darkest color within a color map.
        /// </summary>
        Min = 0, // color1
        Dimmer = 1, // color2
        Dim = 2, // color3
        StandardDim = 3, // color4
        Standard = 4, // color5
        Bright = 5, //color6
        Brighter = 6, // color7
        /// <summary>
        /// The brightest color within a color map.
        /// </summary>
        Max = 7 // color8
    }
}

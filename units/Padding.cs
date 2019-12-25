using Newtonsoft.Json;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents extraneous whitespace that will surround a <see cref="System.Drawing.Image"/>.
    /// </summary>
    public struct Padding
    {
        public static Padding Char => new Padding(right: 1);

        public static Padding Empty => new Padding(0);

        public Padding(int lrtb)
        {
            Left = Right = Top = Bottom = lrtb;
        }

        [JsonConstructor]
        public Padding(int left = 0, int right = 0, int top = 0, int bottom = 0)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        [JsonProperty("left")]
        public int Left { get; set; }

        [JsonProperty("right")]
        public int Right { get; set; }

        [JsonProperty("top")]
        public int Top { get; set; }

        [JsonProperty("bottom")]
        public int Bottom { get; set; }

        /// <summary>
        /// The literal width of the existing <see cref="Padding"/>.
        /// </summary>
        [JsonIgnore]
        public int Width => Left + Right;

        /// <summary>
        /// The literal height of the existing <see cref="Padding"/>.
        /// </summary>
        [JsonIgnore]
        public int Height => Top + Bottom;
    }
}

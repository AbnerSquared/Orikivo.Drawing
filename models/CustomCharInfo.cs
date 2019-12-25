using Newtonsoft.Json;
using System.Drawing;

namespace Orikivo.Drawing
{
    // allows you to set the custom width, height, and offset for a group of characters
    public class CustomCharInfo
    {
        [JsonConstructor]
        public CustomCharInfo(char[] chars, int? width = null, int? height = null, System.Drawing.Point? offset = null)
        {
            Chars = chars;
            Width = width;
            Height = height;
            Offset = offset;
        }

        [JsonProperty("chars")]
        public char[] Chars { get; }

        [JsonProperty("width")]
        public int? Width { get; }

        [JsonProperty("height")]
        public int? Height { get; }

        [JsonProperty("offset")]
        public System.Drawing.Point? Offset { get; }
    }
}

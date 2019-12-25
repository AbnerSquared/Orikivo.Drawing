using Newtonsoft.Json;
using System.Drawing;

namespace Orikivo.Drawing
{
    public class SheetOverride
    {
        public static SheetOverride Empty = new SheetOverride(0, 0);

        [JsonConstructor]
        public SheetOverride(int row, int column, Point? offset = null, int? width = null,
            int? height = null)
        {
            Row = row;
            Column = column;
            Offset = offset ?? Point.Empty;
            Width = width;
            Height = height;
        }

        [JsonProperty("row")]
        public int Row { get; }

        [JsonProperty("col")]
        public int Column { get; }

        [JsonProperty("offset")]
        public Point Offset { get; }

        [JsonProperty("width")]
        public int? Width { get; }

        [JsonProperty("height")]
        public int? Height { get; }
    }
}

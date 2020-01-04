using Newtonsoft.Json;
using System.Drawing;

namespace Orikivo.Drawing
{
    /// <summary>
    /// An image reference that can be referenced by a unique identifier.
    /// </summary>
    public class Sprite
    {
        [JsonConstructor]
        public Sprite(string url, string id = null)
        {
            Id = id;
            Path = url;

            using (Bitmap source = GetImage())
            {
                Width = source.Width;
                Height = source.Height;
            }
        }

        /// <summary>
        /// The unique identifier for the existing <see cref="Sprite"/>.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The local path that points to the <see cref="Image"/> referenced for this <see cref="Sprite"/>.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; }

        public int Width { get; }

        public int Height { get; }

        /// <summary>
        /// Returns the <see cref="Bitmap"/> specified by <see cref="Path"/>.
        /// </summary>
        public Bitmap GetImage()
            => new Bitmap(Path);
    }
}

using System.Drawing;

namespace Orikivo.Poxel
{
    // a sprite that was cropped automatically using the size of the image and position.
    public class CroppedSprite
    {
        internal CroppedSprite(string sheetUrl, int x, int y, int width, int height)
        {
            ParentUrl = sheetUrl;

        }

        public string ParentUrl; // a image path holding a ton of images

        public Bitmap Source => Poxel.Crop(ParentUrl, Width, Height, Pos);
        public int Width { get; }
        public int Height { get; }
        public CropPoint Pos { get; } // the position of where it was cropped.
    }
}

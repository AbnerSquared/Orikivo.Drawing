namespace Orikivo.Poxel
{
    // a sprite that was cropped automatically using the size of the image and position.
    public class CroppedSprite
    {
        public CroppedSprite(string sheetUrl, int x, int y, int width, int height)
        {
            SheetUrl = sheetUrl;

        }

        public string SheetUrl; // a image path holding a ton of images
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }
    }
}

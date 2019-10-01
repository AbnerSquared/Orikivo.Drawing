namespace Orikivo.Poxel
{
    // if the crop is merely specifing the point
    public struct CropPoint
    {
        internal CropPoint(int x, int y, bool isGridCrop = true)
        {
            X = x;
            Y = y;
            IsGridCrop = isGridCrop;
        }
        // a bool defining if the sprite was cropped from a grid, or if it was handcropped.
        public bool IsGridCrop { get; }
        public int X { get; }
        public int Y { get; }
    }
}

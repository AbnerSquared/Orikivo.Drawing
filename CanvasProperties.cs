namespace Orikivo.Poxel
{
    public class CanvasProperties
    {
        // determines if the row's padding underneath is stretched to contain the offset.
        public bool ExtendOnOffset { get; set; }
        public int Width { get; set; } // if empty, is automatically set based on the action.
        public int Height { get; set; }
        public Padding Padding { get; set; }
    }
}

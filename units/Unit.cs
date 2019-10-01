namespace Orikivo.Poxel
{
    // separated from Size to support JsonProperty
    /// <summary>
    /// A class representing size for a PoxelObject.
    /// </summary>
    public class Unit
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

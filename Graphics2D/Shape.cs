namespace Orikivo.Drawing.Graphics2D
{
    /// <summary>
    /// Represents a base polygon that has assured defineable points.
    /// </summary>
    public abstract class Shape
    {
        public Vector2[] Points { get; protected set; }
    }
}

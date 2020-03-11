namespace Orikivo.Drawing.Graphics2D
{
    public class Arc
    {

        public Vector2 Origin { get; }
        public float Radius { get; }
        public AngleF Angle { get; }
        public float Length => GetLength();

        private float GetLength()
        {
            return Angle.Radians * Radius;
        }
    }
}

using System;

namespace Orikivo.Drawing.Graphics2D
{
    public class Fraction
    {
        public float Numerator { get; set; }
        public float Denominator { get; set; }

        public float Value => Numerator / Denominator;
    }

    /// <summary>
    /// Represents two <see cref="Vector2"/> points.
    /// </summary>
    public class Line
    {
        public Line(float ax, float ay, float bx, float by)
        {
            Points = new Vector2[2];
            A = new Vector2(ax, ay);
            B = new Vector2(bx, by);
        }

        public Line(Vector2 a, float length, AngleF direction)
        {
            Points = new Vector2[2];

            Vector2 b = new Vector2(a.X + (length * MathF.Cos(direction.Radians)),
                a.Y + (length * MathF.Sin(direction.Radians)));

            A = a;
            B = b;
        }

        public Line(Vector2 a, Vector2 b)
        {
            Points = new Vector2[2];

            A = a;
            B = b;
        }

        public Vector2[] Points { get; protected set; }

        public Vector2 A
        {
            get => Points[0];
            set => Points[0] = value;
        }

        public Vector2 B
        {
            get => Points[1];
            set => Points[1] = value;
        }

        public float DeltaX => B.X - A.X;
        public float DeltaY => B.Y - A.X;

        public float Slope => DeltaY / DeltaX;

        public float GetLength()
            => CalcF.Distance(A, B);

        public AngleF GetDirection()
            => MathF.Atan(Slope);

        public Vector2 ToVector2()
            => CalcF.PolarToParametric(GetLength(), GetDirection());

        public Vector2 GetMidpoint()
            => new Vector2((A.X + B.X) / 2, (A.Y + B.Y) / 2);

        public float GetY(float x)
            => IsHorizontal() ? 0
            : IsVertical() ? float.NaN
            : Slope * (x - A.X) + A.Y;

        public Line GetPerpendicular(Vector2 p)
        {
            float x = (p.Y - A.Y + (Slope * A.X) + (p.X / Slope)) / (Slope + (1 / Slope));

            return new Line(p, new Vector2(x, GetY(x)));
        }

        

        public Vector2 GetIntersection(Line b)
        {
            float x = GetIntersectX(b);

            if (GetY(x) != b.GetY(x))
                throw new ArgumentException("The y values of each line do not match.");

            return new Vector2(x, GetY(x));
            
        }

        public bool IsHorizontal()
            => A.Y == B.Y;

        public bool IsVertical()
            => A.X == B.X;

        public bool Intersects(Line b)
        {
            float x = GetIntersectX(b);

            return GetY(x) == b.GetY(x);
        }

        // true if this point equals any position on the line
        public bool Contains(Vector2 p)
            => IsHorizontal() ? p.Y == A.Y
            : IsVertical() ? p.X == A.X
            : p.Y == GetY(p.X);

        // true if this point equals any position on the line AND the point is within the specified segment.
        public bool SegmentContains(Vector2 p)
            => ((A.X <= p.X && p.X <= B.X  &&  A.Y <= p.Y && p.Y <= B.Y)
            || (A.X >= p.X && p.X >= B.X  &&  A.Y >= p.Y && p.Y >= B.Y))
            && Contains(p);

        private float GetIntersectX(Line b)
            => (b.A.Y - A.Y + (Slope * A.X) - (b.Slope * b.A.X)) / (Slope - b.Slope);
    }
}

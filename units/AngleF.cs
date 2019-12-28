using Orikivo.Drawing.Graphics2D;
using static System.MathF;

namespace Orikivo.Drawing
{
    // might be useless, we'll see
    public struct AngleF
    {
        public const float MinValue = 0.0f;
        public const float MaxValue = 360.0f - float.Epsilon;

        private static float Wrap(float f)
            => f % 360.0f;

        public static AngleF Zero => 0.0f;
        
        public AngleF(float f)
        {
            _degrees = Wrap(f);
        }

        private float _degrees;

        // the 0 to 1 variant
        public float Normal => RangeF.Convert(RangeF.Degree, RangeF.Percent, Degrees);

        public float Degrees
        {
            get => _degrees;
            set => _degrees = Wrap(value);
        }

        public float Radians
        {
            get => CalculatorF.Radians(Degrees);
            set => Degrees = CalculatorF.Degrees(value);
        }

        public override bool Equals(object obj)
        {
            if (obj is AngleF)
                return ((AngleF)obj) == this;
            else if (obj is float)
                return (float)obj == this;
            else
                return false;
        }

        public override int GetHashCode()
            => (int) Floor(Degrees * 1000.0f); // 3 point precision is kept by multiplying it by 1000.0f;

        public static bool operator ==(AngleF a, AngleF b)
            => a.Degrees == b.Degrees;

        public static bool operator !=(AngleF a, AngleF b)
            => a.Degrees != b.Degrees;

        public static AngleF operator +(AngleF a, AngleF b)
            => a.Degrees + b.Degrees;

        public static AngleF operator -(AngleF a, AngleF b)
            => a.Degrees - b.Degrees;

        public static bool operator ==(AngleF a, float b)
            => a.Degrees == Wrap(b);

        public static bool operator ==(float a, AngleF b)
            => b.Degrees == Wrap(a);

        public static bool operator !=(AngleF a, float b)
            => a.Degrees != Wrap(b);

        public static bool operator !=(float a, AngleF b)
            => b.Degrees != Wrap(a);

        public static AngleF operator +(AngleF a, float b)
            => a.Degrees + Wrap(b);

        public static AngleF operator +(float a, AngleF b)
            => b.Degrees + Wrap(a);

        public static AngleF operator -(AngleF a, float b)
            => a.Degrees - Wrap(b);

        public static AngleF operator -(float a, AngleF b)
            => Wrap(a) - b.Degrees;

        public static AngleF operator *(AngleF a, float b)
        {
            float t = b % 1.0f;
            b = t == 0.0f ? 1.0f : t;

            return a.Degrees * b;
        }

        public static AngleF operator *(float a, AngleF b)
        {
            float t = a % 1.0f;
            a = t == 0.0f ? 1.0f : t;

            return b.Degrees * a;
        }

        public static AngleF operator /(AngleF a, float b)
        {
            float t = b % 1.0f;
            b = t == 0.0f ? 1.0f : t;

            return a.Degrees / b;
        }

        public static implicit operator float(AngleF a)
            => a.Degrees;

        public static implicit operator int(AngleF a)
            => (int) Floor(a.Degrees);

        public static implicit operator AngleF(float f)
            => new AngleF(f);
    }
}

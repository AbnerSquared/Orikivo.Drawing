namespace Orikivo.Drawing
{
    // TODO: Implement markers, which allow sub-ranges, returning the ends of where a value is in between markers.
    public struct RangeF
    {
        public static float Convert(RangeF from, RangeF to, float value)
            => Convert(from.Min, from.Max, to.Min, to.Max, value);

        public static float Convert(float fromMin, float fromMax, float toMin, float toMax, float value)
        {
            float from = fromMax - fromMin;
            float to = toMax - toMin;

            if (from == 0)
                return toMin;

            return (((value - fromMin) * to) / from) + toMin;
        }

        public static float Clamp(RangeF range, float value)
            => Clamp(range.Min, range.Max, value);

        public static float Clamp(float min, float max, float value)
        {
            return value > max ? max : value < min ? min : value;
        }

        public static bool Contains(float min, float max, float value, bool inclusiveMin = true, bool inclusiveMax = true)
        {
            return (inclusiveMin ? value >= min : value > min) && (inclusiveMax ? value <= max : value < max);
        }

        public RangeF(float max, bool inclusiveMin = true, bool inclusiveMax = true)
        {
            Min = 0f;
            Max = max;
            InclusiveMin = inclusiveMin;
            InclusiveMax = inclusiveMax;
        }

        public RangeF(float min, float max, bool inclusiveMin = true, bool inclusiveMax = true)
        {
            Min = min;
            Max = max;
            InclusiveMin = inclusiveMin;
            InclusiveMax = inclusiveMax;
        }

        public static RangeF Percent => new RangeF(0.00f, 1.00f);
        public static RangeF Angle => new RangeF(0.00f, 360.00f, true, false);
        public static RangeF Byte => new RangeF(0, 255);
        public static RangeF DayHours => new RangeF(0.00f, 24.00f, true, false);
        public static RangeF TimeOffset => new RangeF(-14.00f, 14.00f);

        public float Min { get; }

        public float Max { get; }

        public bool InclusiveMin { get; }

        public bool InclusiveMax { get; }

        public float Clamp(float value)
            => Clamp(Min, Max, value);

        public float Convert(RangeF range, float value)
            => Convert(range.Min, range.Max, Min, Max, value);

        public float Convert(float min, float max, float value)
            => Convert(min, max, Min, Max, value);

        public bool All(params float[] values)
        {
            foreach (float value in values)
                if (!Contains(value))
                    return false;

            return true;
        }

        public bool Contains(float value)
            => Contains(Min, Max, value, InclusiveMin, InclusiveMax);
    }
}

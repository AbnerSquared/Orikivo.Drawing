using System;

namespace Orikivo.Drawing
{
    public class TimeCycle
    {
        public static float GetHourFloatValue(DateTime time)
        {
            float remHour = RangeF.Percent.Convert(0, 59, time.Minute);
            float hour = time.Hour + remHour;
            return hour;
        }

        private TimeCycle() { }

        // public static TimeCycle FromTimeZone(TimeZone zone) {}

        public static TimeCycle Utc = new TimeCycle { Dawn = 7.00f, Sunrise = 8.00f, Meridian = 12.00f, Sunset = 16.00f, Dusk = 17.00f, NightStart = 18.00f, NightEnd = 6.00f };

        public float Dawn { get; private set; }
        public float Sunrise { get; private set; }
        public float Meridian { get; private set; }
        public float Sunset { get; private set; }
        public float Dusk { get; private set; }
        public float NightStart { get; private set; }
        public float NightEnd { get; private set; }
    }
}

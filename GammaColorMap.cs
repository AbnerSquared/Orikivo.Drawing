using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Drawing
{
    // GUIDE: https://css-tricks.com/8-digit-hex-codes/
    public class GammaColorMap
    {
        public const int RequiredLength = 8;
        
        // NOTE: The color values must go from darkest to brightest in order to properly function.
        public static GammaColorMap Default =    new GammaColorMap(0x000000, 0x242424, 0x494949, 0x6D6D6D, 0x929292, 0xB6B6B6, 0xDBDBDB, 0xFFFFFF);
        public static GammaColorMap NeonRed =    new GammaColorMap(0x5C1F49, 0x722451, 0x882959, 0x9F2E61, 0xB53367, 0xCB396D, 0xE13D75, 0xF8427D);
        public static GammaColorMap GammaGreen = new GammaColorMap(0x0C525F, 0x1A6A6E, 0x28827D, 0x369A8C, 0x44B29B, 0x52CAAA, 0x60E2B9, 0x6EFAC8);

        // NOTE: These are already gradient color maps. They should not be allowed to fuse.
        public static GammaColorMap Alconia =    new GammaColorMap(0x08141E, 0x0F2A3F, 0x20394F, 0xF6D6BD, 0xC3A38A, 0x997577, 0x816271, 0x4E495F); // Nyx8: https://lospec.com/palette-list/nyx8

        // NOTE: These color maps contain unique properties. (The Alpha component is 0x80 (50%).)
        public static GammaColorMap Glass =      new GammaColorMap(0x1C595E, 0x2B6C74, 0x31807F, 0x469996, 0x52A6A1, 0x87D0C8, 0xA8E7DF, 0xDEFFFA);

        // NOTE: These four color maps transition between each other to create a timely color system.
        public static GammaColorMap Sunrise = new GammaColorMap(0x15123D, 0x261948, 0x381B51, 0x4F285D, 0x653366, 0x894B78, 0xA96786, 0xC5748D);
        public static GammaColorMap Meridian = new GammaColorMap(0x004F99, 0x0070B1, 0x008DC3, 0x2AAFDB, 0x5ED5ED, 0x7FE3EF, 0x99EDF5, 0xDBFCFC);
        public static GammaColorMap Dusk = new GammaColorMap(0x681467, 0x7F1A70, 0xA21E7B, 0xDB308E, 0xE95490, 0xEF6C94, 0xF87C90, 0xF59098);
        public static GammaColorMap Night = new GammaColorMap(0x020A21, 0x091934, 0x0D213D, 0x00314F, 0x194A5A, 0x346061, 0x397773, 0x2E8982);

        public static GammaColorMap FromUtcNow()
            => FromTime(DateTime.UtcNow);

        public static GammaColorMap FromTime(DateTime time)
            => FromHour(TimeCycle.GetHourFloatValue(time));

        public static GammaColorMap FromHour(float hour) // TODO: Incorporate offsets, and incorporate Range.Markers (when ready).
        {
            TimeCycle cycle = TimeCycle.Utc;

            if (hour <= cycle.NightEnd || hour > cycle.NightStart)
                return Night;

            if (hour > cycle.NightEnd && hour <= cycle.Dawn)
                return Merge(Night, Sunrise, GetHourStrength(cycle.NightEnd, cycle.Dawn, hour));

            if (hour > cycle.Dawn && hour <= cycle.Sunrise)
                return Merge(Sunrise, Dusk, GetHourStrength(cycle.Dawn, cycle.Sunrise, hour));

            if (hour > cycle.Sunrise && hour <= cycle.Meridian)
                return Merge(Dusk, Meridian, GetHourStrength(cycle.Sunrise, cycle.Meridian, hour));

            if (hour > cycle.Meridian && hour <= cycle.Sunset)
                return Merge(Meridian, Dusk, GetHourStrength(cycle.Meridian, cycle.Sunset, hour));

            if (hour > cycle.Sunset && hour <= cycle.Dusk)
                return Merge(Dusk, Sunrise, GetHourStrength(cycle.Sunset, cycle.Dusk, hour));

            if (hour > cycle.Dusk && hour <= cycle.NightStart)
                return Merge(Sunrise, Night, GetHourStrength(cycle.Dusk, cycle.NightStart, hour));

            throw new Exception("The hour float value given is out of range.");
        }

        private static float GetHourStrength(float from, float to, float hour)
            => RangeF.Percent.Convert(0.00f, to - from, hour - from);

        /// <summary>
        /// Merges two <see cref="GammaColorMap"/> values together, merging the foreground value with the background value by a specified strength.
        /// </summary>
        public static GammaColorMap Merge(GammaColorMap background, GammaColorMap foreground, float strength)
        {
            List<GammaColor> colors = new List<GammaColor>();

            for (int g = 0; g < RequiredLength; g++)
                colors.Add(GammaColor.Merge(background.Values[g], foreground.Values[g], strength));

            return new GammaColorMap(colors.ToArray());
        }

        public GammaColorMap(params int[] rgbValues)
        //    => new GammaColorMap(rgbValues.Select(x => new GammaColor((uint)x)).ToArray());
        {
            if (rgbValues.Length != RequiredLength)
                throw new ArgumentException("A GammaColorMap requires eight unique color values.");

            Values = rgbValues.Select(x => new GammaColor((uint)x)).ToArray();
        }

        public GammaColorMap(params long[] argbValues)
        //    => new GammaColorMap(rgbaValues.Select(x => new GammaColor(x)).ToArray());
        {
            if (argbValues.Length != RequiredLength)
                throw new ArgumentException("A GammaColorMap requires eight unique color values.");

            Values = argbValues.Select(x => new GammaColor(x)).ToArray();
        }

        public GammaColorMap(params GammaColor[] colors)
        {
            if (colors == null)
                throw new ArgumentException("A GammaColorMap requires an existing list of colors.");

            if (colors.Length != RequiredLength)
                throw new ArgumentException("A GammaColorMap requires eight unique color values.");

            Values = colors;
        }

        public GammaColor[] Values { get; }
        
        public GammaColor GetValue(Gamma gamma)
            => Values[(int)gamma];

        public GammaColor this[Gamma g]
            => GetValue(g);
    }
}

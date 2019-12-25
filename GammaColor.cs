﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Drawing
{
    // an immutable color
    public struct GammaColor
    {
        public static GammaColor NeonRed = new GammaColor(0xF8427D);
        public static GammaColor GammaGreen = new GammaColor(0x6EFAC8);

        private const int A_SHIFT = 24;
        private const int R_SHIFT = 16;
        private const int G_SHIFT = 8;
        private const int B_SHIFT = 0;

        private const float R_LUMINANCE = 0.2126f;
        private const float G_LUMINANCE = 0.7152f;
        private const float B_LUMINANCE = 0.0722f;

        public static GammaColor FromCmyk(float c, float m, float y, float k)
        {
            RangeF bounds = RangeF.Percent;

            if (!bounds.All(c, m, y, k))
                throw new ArgumentException("One of the specified float values are out of range.");

            byte r = GetCmy(c, k);
            byte g = GetCmy(m, k);
            byte b = GetCmy(y, k);

            return new GammaColor(r, g, b);
        }

        private static byte GetCmy(float v, float k)
        {
            return (byte) Math.Floor(255 * (1 - v) * (1 - k));
        }

        // TODO: Make this method efficient.
        public static GammaColor FromHex(string hex)
        {
            hex = hex.TrimStart('#');

            string format = "0x{0}";

            byte r = byte.Parse(string.Format(format, hex[0] + hex[1]));
            byte g = byte.Parse(string.Format(format, hex[2] + hex[3]));
            byte b = byte.Parse(string.Format(format, hex[4] + hex[5]));

            return new GammaColor(r, g, b);
        }

        public static GammaColor FromHsl(float h, float s, float l)
        {
            if (!RangeF.Angle.Contains(h) || !RangeF.Percent.All(s, l))
                throw new ArgumentException("One of the specified float values are out of range.");

            float c = (1 - Math.Abs((2 * l) - 1)) * s;
            float x = c * (1 - Math.Abs((h / 60.00f) % 2 - 1));
            float m = l - (c / 2);

            return CreateFromChmx(c, h, m, x);
        }

        public static GammaColor FromHsv(float h, float s, float v)
        {
            if (!RangeF.Angle.Contains(h) || !RangeF.Percent.All(s, v))
                throw new ArgumentException("One of the specified float values are out of range.");

            float c = v * s;
            float x = c * (1 - Math.Abs((h / 60.00f) % 2 - 1));
            float m = v - c;

            return CreateFromChmx(c, h, m, x);

        }

        private static GammaColor CreateFromChmx(float c, float h, float m,  float x)
        {
            float rF = 0.00f;
            float gF = 0.00f;
            float bF = 0.00f;

            if (RangeF.Contains(0.00f, 60.00f, h, true, false))
            {
                rF = c;
                gF = x;
                bF = 0;
            }
            else if (RangeF.Contains(60.00f, 120.00f, h, true, false))
            {
                rF = x;
                gF = c;
                bF = 0;
            }
            else if (RangeF.Contains(120.00f, 180.00f, h, true, false))
            {
                rF = 0;
                gF = c;
                bF = x;
            }
            else if (RangeF.Contains(180.00f, 240.00f, h, true, false))
            {
                rF = 0;
                gF = x;
                bF = c;
            }
            else if (RangeF.Contains(240.00f, 300.00f, h, true, false))
            {
                rF = x;
                gF = 0;
                bF = c;
            }
            else if (RangeF.Contains(300.00f, 360.00f, h, true, false))
            {
                rF = c;
                gF = 0;
                bF = x;
            }

            byte r = (byte)Math.Floor((rF + m) * 255);
            byte g = (byte)Math.Floor((gF + m) * 255);
            byte b = (byte)Math.Floor((bF + m) * 255);

            return new GammaColor(r, g, b);
        }

        /// <summary>
        /// Returns a <see cref="GammaColor"/> that is the conversion of the specified <see cref="GammaColor"/> to grayscale.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static GammaColor Grayscale(GammaColor value)
        {
            byte rgb = (byte) Math.Floor(R_LUMINANCE * value.R + G_LUMINANCE * value.G + B_LUMINANCE * value.B);
            return new GammaColor(rgb, rgb, rgb);
        }

        // Keep the opacity of the initial alpha
        public static GammaColor Merge(GammaColor background, GammaColor foreground, float strength)
        {
            // TODO: Ensure range of 0.0 to 1.0
            if (strength > 1.00f || strength < 0.00f)
                throw new Exception("The specified merge strength must be within the range of 0.00f to 1.00f.");

            byte r = GetMergeValue(background.R, foreground.R, strength);
            byte g = GetMergeValue(background.G, foreground.G, strength);
            byte b = GetMergeValue(background.B, foreground.B, strength);

            return new GammaColor(r, g, b);
        }

        private static byte GetMergeValue(byte b, byte f, float strength)
            => (byte)Math.Floor((b * (1.00f - strength)) + (f * strength));

        /// <summary>
        /// Gets the distance between two <see cref="GammaColor"/> values.
        /// </summary>
        public static int Distance(GammaColor from, GammaColor to)
        {
            int r = to.R - from.R;
            int g = to.G - from.G;
            int b = to.B - from.B;
            return r * r + g * g + b * b;
        }

        /// <summary>
        /// Returns the average <see cref="GammaColor"/> between two <see cref="GammaColor"/> values.
        /// </summary>
        public static GammaColor Average(GammaColor a, GammaColor b)
        {
            byte red = GetAverage(a.R, b.R);
            byte green = GetAverage(a.G, b.G);
            byte blue = GetAverage(a.B, b.B);

            return new GammaColor(red, green, blue);
        }

        /// <summary>
        /// Returns the average <see cref="GammaColor"/> for an array of <see cref="GammaColor"/> values.
        /// </summary>
        public static GammaColor Average(GammaColor[] colors)
        {
            byte r = GetAverage(colors.Select(x => x.R).Cast<int>());
            byte g = GetAverage(colors.Select(x => x.G).Cast<int>());
            byte b = GetAverage(colors.Select(x => x.B).Cast<int>());

            return new GammaColor(r, g, b);
        }

        private static byte GetAverage(IEnumerable<int> values)
        {
            if (values.Count() == 0)
                throw new ArgumentException("There must at least be one specified value in order to get an average.");

            if (values.Count() == 1)
                return (byte)values.ElementAt(0);

            return (byte)Math.Floor(values.Sum() / (double)values.Count());
        }

        // TODO: Create GetAverage method for all integer types.
        private static byte GetAverage(params int[] values)
            => GetAverage((IEnumerable<int>)values);

        /// <summary>
        /// Returns a <see cref="GammaColor"/> from the difference between two <see cref="GammaColor"/> values.
        /// </summary>
        public static GammaColor Difference(GammaColor a, GammaColor b)

        {
            byte red = (byte) Math.Abs(a.R - b.R);
            byte green = (byte) Math.Abs(a.G - b.G);
            byte blue = (byte) Math.Abs(a.B - b.B);

            return new GammaColor(red, green, blue);
        }

        /// <summary>
        /// Returns a <see cref="GammaColor"/> that is the opposite of the specified <see cref="GammaColor"/>.
        /// </summary>
        public static GammaColor Negative(GammaColor value)
        {
            byte r = (byte) (255 - value.R);
            byte g = (byte) (255 - value.G);
            byte b = (byte) (255 - value.B);

            return new GammaColor(r, g, b);
        }

        /// <summary>
        /// Returns the closest matching <see cref="GammaColor"/> for the specified value from an array of <see cref="GammaColor"/> values.
        /// </summary>
        public static GammaColor ClosestMatch(GammaColor value, GammaColor[] colors)
        {
            return colors[ClosestMatchAt(value, colors)];
        }

        /// <summary>
        /// Returns the index of the closest matching index of the specified <see cref="GammaColor"/> from an array of <see cref="GammaColor"/> values.
        /// </summary>
        public static int ClosestMatchAt(GammaColor value, GammaColor[] colors)
        {
            int leastIndex = 0;
            int least = int.MaxValue;

            for (int i = 0; i < colors.Length; i++)
            {
                int dist = Distance(value, colors[i]);

                if (dist >= least)
                    continue;

                leastIndex = i;
                least = dist;

                if (dist == 0)
                    return i;
            }

            return leastIndex;
        }

        /// <summary>
        /// Returns the index of the closest matching index of the specified <see cref="GammaColor"/> from a <see cref="GammaColorMap"/>.
        /// </summary>
        public static int ClosestMatchAt(GammaColor value, GammaColorMap colors)
            => ClosestMatchAt(value, colors.Values);

        /// <summary>
        /// Returns the closest matching <see cref="GammaColor"/> for the specified value from a <see cref="GammaColorMap"/>.
        /// </summary>
        public static GammaColor ClosestMatch(GammaColor value, GammaColorMap colors)
            => ClosestMatch(value, colors.Values);

        [JsonConstructor]
        internal GammaColor(long rgba)
        {
            Value = rgba;
        }

        public GammaColor(uint rgb)
        {
            Value = MakeArgb(rgb, 255);
        }

        public GammaColor(byte r, byte g, byte b)
        {
            Value = MakeArgb(r, g, b, 255);
        }

        public GammaColor(byte r, byte g, byte b, byte a)
        {
            Value = MakeArgb(r, g, b, a);
        }

        [JsonProperty("raw_value")]
        public long Value { get; }

        [JsonIgnore]
        public byte A => (byte)(Value >> A_SHIFT);

        [JsonIgnore]
        public byte R => (byte)(Value >> R_SHIFT);

        [JsonIgnore]
        public byte G => (byte)(Value >> G_SHIFT);

        [JsonIgnore]
        public byte B => (byte)(Value >> B_SHIFT);

        private static long MakeArgb(uint rgb, byte a)
            => MakeArgb((byte)(rgb >> R_SHIFT), (byte)(rgb >> G_SHIFT), (byte)(rgb >> B_SHIFT), a);

        private static long MakeArgb(byte r, byte g, byte b, byte a)
            => (long) unchecked((uint)(r << R_SHIFT | g << G_SHIFT | b << B_SHIFT | a << A_SHIFT)) & 0xffffffff;

        private static uint MakeRgb(byte r, byte g, byte b)
            => unchecked((uint)(r << R_SHIFT | g << G_SHIFT | b << B_SHIFT)) & 0xffffff;

        public static implicit operator Color(GammaColor c)
            => Color.FromArgb((int)c.Value);

        public static explicit operator GammaColor(Color c)
            => new GammaColor((uint)c.ToArgb());

        public override string ToString()
            => A < 255 ? string.Format("#{0:X8}", Value) : string.Format("#{0:X6}", MakeRgb(R, G, B));
    }
}
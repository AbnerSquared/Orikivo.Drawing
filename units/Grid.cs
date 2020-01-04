﻿using System;

namespace Orikivo.Drawing
{
    /// <summary>
    /// A generic grid that allows for simple multi-dimensional manipulations.
    /// </summary>
    public class Grid<T>
    {
        // TODO: Make numeric methods for Grid<int>, Grid<long>, Grid<float>, etc...
        // public static float Multiply(Grid<float> a, Grid<float> b);

        public Grid(System.Drawing.Size size, T defaultValue = default)
        {
            Values = new T[size.Height, size.Width];

            // TODO: Figure out why this no longer works.
            //if (defaultValue != default)
                Clear(defaultValue);
        }

        public Grid(int width, int height, T defaultValue = default)
        {
            Values = new T[height, width];

            //if (defaultValue != default)
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        Values[y, x] = defaultValue;
        }

        public T[,] Values { get; } // TODO: Maybe add support for T[][] (Jagged Arrays?)

        public int Width => Values.GetLength(1);
        public int Height => Values.GetLength(0);

        /// <summary>
        /// Sets the value of a grid coordinate by a specified <see cref="System.Drawing.Point"/>.
        /// </summary>
        public void SetValue(T value, System.Drawing.Point p)
            => SetValue(value, p.X, p.Y);

        public Grid<TResult> Cast<TResult>()
        {
            // TODO: Change how this method is handled, as it currently doesn't work as intended.
            Grid<TResult> result = new Grid<TResult>(Width, Height);

            result.ForEachValue((int x, int y) => GetValue(x, y).CastObject<TResult>());

            return result;
        }

        /// <summary>
        /// Sets the value of a grid coordinate by a specified x and y position.
        /// </summary>
        public void SetValue(T value, int x, int y)
        {
            Values[y, x] = value;
        }

        /// <summary>
        /// Returns the row of the existing <see cref="Grid{T}"/> by a specified row index.
        /// </summary>
        public T[] GetRow(int y) // IEnumerable<T>
        {
            T[] row = new T[Width];

            for (int x = 0; x < Width; x++)
                row[x] = Values[y, x]; // yield return Values[y, x]; TODO: Compare if T[] or IEnumerable<T> is better.

            return row;
        }

        public void SetRow(int y, T value)
        {
            for (int x = 0; x < Width; x++)
                Values[y, x] = value;
        }

        /// <summary>
        /// Returns the column of the existing <see cref="Grid{T}"/> by a specified column index.
        /// </summary>
        public T[] GetColumn(int x)
        {
            T[] column = new T[Height];

            for (int y = 0; y < Height; y++)
                column[y] = Values[y, x];

            return column;
        }

        public void SetColumn(int x, T value)
        {
            for (int y = 0; y < Height; y++)
                Values[y, x] = value;
        }


        public T GetValue(System.Drawing.Point p)
            => GetValue(p.X, p.Y);

        public T GetValue(int x, int y)
        {
            return Values[y, x];
        }

        public bool TryGetValue(int x, int y, out T value)
        {
            value = default;

            if (!RangeF.Contains(0, Width, x, true, false) || !RangeF.Contains(0, Height, y, true, false))
                return false;

            value = Values[y, x];
            return true;
        }

        /// <summary>
        /// Clears the existing <see cref="Grid{T}"/> by a default specified value.
        /// </summary>
        public void Clear(T value)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Values[y, x] = value;
        }

        /*
        public void ForEachValue(Func<T> action)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Values[y, x] = action.Invoke();
        }*/

        public void ForEachValue(Func<int, int, T> action)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Values[y, x] = action.Invoke(x, y);
        }

        public T this[int x, int y]
        {
           get => Values[y, x];
           set => SetValue(value, x, y);
        }
    }
}

using System;
using System.Linq;

namespace Orikivo.Drawing
{

    public static class GridExtensions
    {
        public static Grid<T?> GetRegionOrDefault<T>(this Grid<T> grid, int x, int y, int width, int height) where T : struct
        {
            Grid<T?> region = new Grid<T?>(width, height);

            for (int py = 0; py < height; py++)
                for (int px = 0; px < width; px++)
                    if (grid.Contains(px + x, py + y))
                        region.SetValue(grid.GetValue(x + px, y + py), px, py);

            return region;
        }
    }

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

        public Grid(T[,] values)
        {
            Values = values;
        }

        public Grid(T[][] values)
        {
            int height = values.GetUpperBound(0) + 1;
            int width = values.OrderByDescending(v => v.GetUpperBound(0)).First().GetUpperBound(0) + 1;

            Values = new T[height, width];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    if (y >= 0 && y <= values.GetUpperBound(0))
                        if (x >= 0 && x <= values[y].GetUpperBound(0))
                            Values[y, x] = values[y][x];
        }

        public T[,] Values { get; } // TODO: Maybe add support for T[][] (Jagged Arrays?)

        public int Width => Values.GetLength(1);
        public int Height => Values.GetLength(0);
        public System.Drawing.Size Size => new System.Drawing.Size(Values.GetLength(1), Values.GetLength(0));

        /// <summary>
        /// Sets the value of a grid coordinate by a specified <see cref="System.Drawing.Point"/>.
        /// </summary>
        public void SetValue(T value, System.Drawing.Point p)
            => SetValue(value, p.X, p.Y);

        public Grid<TResult> Cast<TResult>()
        {
            // TODO: Change how this method is handled, as it currently doesn't work as intended.
            Grid<TResult> result = new Grid<TResult>(Width, Height);

            result.SetEachValue((int x, int y) => GetValue(x, y).CastObject<TResult>());

            return result;
        }

        public Grid<TValue> Select<TValue>(Func<T, TValue> selector)
        {
            Grid<TValue> result = new Grid<TValue>(Size);

            ForEachValue((int x, int y) => result.SetValue(selector.Invoke(GetValue(x, y)), x, y));

            return result;
        }

        public bool All(Func<T, bool> predicate)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (!predicate.Invoke(GetValue(x, y)))
                        return false;

            return true;
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

            if (!Contains(x, y))
                return false;

            value = Values[y, x];
            return true;
        }

        // a region from which it doesn't matter if the portion of the area is available or not
        // boundaries dont matter in this one
        public Grid<T> GetPartialRegion(System.Drawing.Point point, System.Drawing.Size size)
            => GetPartialRegion(point.X, point.Y, size.Width, size.Height);

        public Grid<T> GetPartialRegion(int x, int y, int width, int height)
        {
            Grid<T> region = new Grid<T>(width, height);

            for (int py = 0; py < height; py++)
                for (int px = 0; px < width; px++)
                    if (Contains(px + x, py + y))
                        region.SetValue(GetValue(x + px, y + py), px, py);

            return region;
        }

        public bool Contains(int x, int y)
            => (x >= 0 && x < Width && y >= 0 && y < Height);

        // boundaries matter in this one

        public Grid<T> GetRegion(System.Drawing.Point point, System.Drawing.Size size)
            => GetRegion(point.X, point.Y, size.Width, size.Height);

        public Grid<T> GetRegion(int x, int y, int width, int height)
        {
            if (!Contains(x + width, y + height))
                throw new ArgumentException("The region specified is out of bounds.");

            Grid<T> region = new Grid<T>(width, height);

            for (int py = 0; py < height; py++)
                for (int px = 0; px < width; px++)
                    region.SetValue(GetValue(x + px, y + py), px, py);

            return region;
        }

        public void SetRegion(T value, int x, int y, int width, int height)
        {
            if (!Contains(x + width, y + height))
                throw new ArgumentException("The region specified is out of bounds.");

            for (int py = 0; py < height; py++)
                for (int px = 0; px < width; px++)
                    SetValue(value, px + x, py + y);
        }

        public void SetRegion(T[,] region, int x, int y, int width, int height)
        {
            if (!Contains(x + width, y + height))
                throw new ArgumentException("The region specified is out of bounds.");

            for (int py = 0; py < height; py++)
                for (int px = 0; px < width; px++)
                    SetValue(region[py, px], px + x, py + y);
        }

        public void SetRegion(Grid<T> region, int x, int y, int width, int height)
            => SetRegion(region.Values, x, y, width, height);

        /// <summary>
        /// Clears the existing <see cref="Grid{T}"/> by a default specified value.
        /// </summary>
        public void Clear(T value)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Values[y, x] = value;
        }

        public void SetEachValue(Func<int, int, T> action)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Values[y, x] = action.Invoke(x, y);
        }

        public void ForEachValue(Action<int, int> action)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    action.Invoke(x, y);
        }

        public T this[int x, int y]
        {
           get => Values[y, x];
           set => SetValue(value, x, y);
        }
    }
}

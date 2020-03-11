﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo.Drawing
{
    // TODO: Separate obscure methods into a GridExtensions file.
    /// <summary>
    /// Represents a grid that allows for complex generic matrix manipulation.
    /// </summary>
    public class Grid<T> : IEnumerable<T>
    {
        /// <summary>
        /// Initializes a new <see cref="Grid{T}"/> with a specified <see cref="System.Drawing.Size"/> and an optional default value.
        /// </summary>
        public Grid(System.Drawing.Size size, T defaultValue = default)
        {
            Values = new T[size.Height, size.Width];

            // TODO: Figure out why this no longer works.
            //if (defaultValue != default)
                Clear(defaultValue);
        }

        /// <summary>
        /// Initializes a new <see cref="Grid{T}"/> with a specified width, height, and an optional default value.
        /// </summary>
        public Grid(int width, int height, T defaultValue = default)
        {
            Values = new T[height, width];

            //if (defaultValue != default)
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        Values[y, x] = defaultValue;
        }

        /// <summary>
        /// Initializes a new <see cref="Grid{T}"/> from a rectangluar <see cref="Array"/>.
        /// </summary>
        public Grid(T[,] values)
        {
            Values = values;
        }

        // TODO: Implement a default value set-up for a jagged array, in case certain rows/columns are empty.
        /// <summary>
        /// Initializes a new <see cref="Grid{T}"/> from a jagged <see cref="Array"/>.
        /// </summary>
        public Grid(T[][] values, T defaultValue = default)
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

        public IEnumerator<T> GetEnumerator()
            => new Enumerator<T>(this);

        IEnumerator IEnumerable.GetEnumerator()
            => new Enumerator<T>(this);

        /// <summary>
        /// Represents the raw elements of the <see cref="Grid{T}"/>.
        /// </summary>
        public T[,] Values { get; } // TODO: Maybe add support for T[][] (Jagged Arrays?)

        /// <summary>
        /// Gets a 32-bit integer that represents the width of the <see cref="Grid{T}"/>.
        /// </summary>
        public int Width => Values.GetLength(1);

        /// <summary>
        /// Gets a 32-bit integer that represents the height of the <see cref="Grid{T}"/>.
        /// </summary>
        public int Height => Values.GetLength(0);

        /// <summary>
        /// Gets the total number of possible elements in the <see cref="Grid{T}"/>.
        /// </summary>
        public int Count => Values.Length;

        public System.Drawing.Size Size => new System.Drawing.Size(Values.GetLength(1), Values.GetLength(0));

        /// <summary>
        /// Initializes a new <see cref="Grid{T}"/> with this <see cref="Grid{T}"/>'s current values.
        /// </summary>
        public Grid<T> Clone()
            => new Grid<T>(Values);

        // TODO: Separate into an extension.
        /// <summary>
        /// Sets the value of a grid coordinate by a specified <see cref="System.Drawing.Point"/>.
        /// </summary>
        public void SetValue(T value, System.Drawing.Point p)
            => SetValue(value, p.X, p.Y);

        
        public Grid<TResult> ConvertAll<TResult>()
        {
            // TODO: Change how this method is handled, as it currently doesn't work as intended.
            Grid<TResult> result = new Grid<TResult>(Width, Height);
            result.SetEachValue((int x, int y) => GetValue(x, y).CastObject<TResult>());

            return result;
        }

        /// <summary>
        /// Projects each element of a <see cref="Grid{T}"/> into a new form.
        /// </summary>
        /// <param name="selector">A transform function to apply to each element.</param>
        public Grid<TValue> Select<TValue>(Func<T, TValue> selector)
        {
            Grid<TValue> result = new Grid<TValue>(Size);
            ForEachValue((int x, int y) => result.SetValue(selector.Invoke(GetValue(x, y)), x, y));

            return result;
        }

        /// <summary>
        /// Determines whether all elements of a <see cref="Grid{T}"/> satisfy a condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        public bool All(Func<T, bool> predicate)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (!predicate.Invoke(GetValue(x, y)))
                        return false;

            return true;
        }

        /// <summary>
        /// Sets the value of a grid coordinate to a specified x- and y-coordinate.
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
                row[x] = Values[y, x]; // yield return Values[y, x];
            
            // TODO: Compare if T[] or IEnumerable<T> is better.

            return row;
        }

        /// <summary>
        /// Sets all of the elements of a row to a specified value.
        /// </summary>
        /// <param name="y">The index of the row to set.</param>
        /// <param name="value">The value that will be set for each element in the row.</param>
        public void SetRow(int y, T value)
        {
            for (int x = 0; x < Width; x++)
                Values[y, x] = value;
        }

        // NOTE: This method can set a row, but if the row is incomplete, it will retain its previous value
        public void SetRow(int y, IEnumerable<T> values)
        {
            if (values.Count() > Width)
                throw new ArgumentException("The row specified must be less than or equal to the width of the grid.");
        }

        public void SetColumn(int x, IEnumerable<T> values)
        {
            if (values.Count() > Height)
                throw new ArgumentException("The column specified must be less than or equal to the height of the grid.");
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

        public Grid<T> GetRegion(System.Drawing.Rectangle rectangle)
            => GetRegion(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

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

        public void SetRegion(T[,] region, int x, int y)
        {
            int width = region.GetLength(1);
            int height = region.GetLength(0);
            if (!Contains(x + width, y + height))
                throw new ArgumentException("The region specified is out of bounds.");

            for (int py = 0; py < height; py++)
                for (int px = 0; px < width; px++)
                    SetValue(region[py, px], px + x, py + y);
        }

        public void SetRegion(Grid<T> region, int x, int y)
            => SetRegion(region.Values, x, y);

        /// <summary>
        /// Clears the existing <see cref="Grid{T}"/> using a specified value.
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

        public void ForEachColumn(Action<T[]> action)
        {
            for (int x = 0; x < Width; x++)
                action.Invoke(GetColumn(x));
        }

        public void ForEachRow(Action<T[]> action)
        {
            for (int y = 0; y < Height; y++)
                action.Invoke(GetRow(y));
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


        private T ValueAt(int i)
        {
            (int x, int y) = GetPosition(i);
            return Values[y, x];
        }

        private (int x, int y) GetPosition(int i)
        {
            int x = i;
            int y = 0;

            while (x >= Width)
            {
                x -= Width;
                y++;
            }

            return (x, y);
        }

        public T this[int i]
        {
            get => ValueAt(i);
            set
            {
                (int x, int y) = GetPosition(i);
                Values[y, x] = value;
            }
        }

        // TODO: Since a Grid can be any size, it might be wise to handle extremely large grids.
        /// <summary>
        /// Returns a <see cref="string"/> that represents all elements in the <see cref="Grid{T}"/>.
        /// </summary>
        public override string ToString()
        {
            StringBuilder grid = new StringBuilder();

            ForEachRow(delegate (T[] row)
            {
                grid.Append("[ ");
                grid.AppendJoin(" ", row.Select(x => x.ToString()));
                grid.AppendLine(" ]");
            });

            return grid.ToString();
        }

        public class Enumerator<T> : IEnumerator<T>
        {
            private T[,] _values;
            private int _position = -1;
            private T _current;

            public Enumerator(Grid<T> grid)
            {
                _values = grid.Values;
                _current = default(T);
            }

            private T GetCurrent()
            {
                int x = _position;
                int width = _values.GetLength(1);
                int height = _values.GetLength(0);
                int y = 0;

                while (x >= width)
                {
                    x -= width;
                    y++;
                }

                return _values[y, x];

            }

            public bool MoveNext()
            {
                if (++_position >= _values.Length)
                {
                    return false;
                }
                else
                {
                    _current = GetCurrent();
                }

                return true;
            }

            public void Reset()
            {
                _position = -1;
            }

            void IDisposable.Dispose() { }

            public T Current
            {
                get => _current;
            }

            object IEnumerator.Current
            {
                get => Current;
            }
        }
    }
}
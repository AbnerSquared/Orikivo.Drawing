using System;
using System.Collections.Generic;
using Point = System.Drawing.Point;

namespace Orikivo.Drawing.Graphics3D
{
    public class Camera
    {
        public Camera(int width, int height, float fov, float near, float far, GammaColorMap palette)
        {
            Width = width;
            Height = height;
            Fov = fov;
            Near = near;
            Far = far;
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Palette = palette;
            BackgroundColor = palette[Gamma.Min];
        }

        public GammaColorMap Palette { get; set; }

        public GammaColor BackgroundColor { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public float Fov { get; set; }

        // float ViewDistance
        // ClipPlane NearClipPlane
        // ClipPlane FarClipPlane

        public float Far { get; set; }
        public float Near { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Grid<GammaColor> GetScreen()
            => new Grid<GammaColor>(Width, Height, BackgroundColor);

        public MatrixF GetProjector()
            => MatrixF.CreateProjector(Near, Far, Fov, Width / Height);

        public bool Contains(System.Drawing.Point p)
            => Contains(p.X, p.Y);

        public bool Contains(int x, int y)
        {
            return RangeF.Contains(0, Width, x, true, false) &&
                   RangeF.Contains(0, Height, y, true, false);
        }

        public List<System.Drawing.Point> GetVisible(System.Drawing.Point a, System.Drawing.Point b)
            => GetVisible(a.X, a.Y, b.X, b.Y);

        public List<System.Drawing.Point> GetVisible(int x1, int y1, int x2, int y2)
        {
            List<System.Drawing.Point> points = new List<System.Drawing.Point>();
            int x = 0;
            int y = 0;

            int endX = 0;
            int endY = 0;

            int dx = x2 - x1;
            int dy = y2 - y1;
            int run = Math.Abs(dx);
            int rise = Math.Abs(dy);

            int px = 2 * rise - run;
            int py = 2 * run - rise;

            bool both = (dx < 0 && dy < 0) || (dx > 0 && dy > 0);

            if (rise <= run)
            {
                x = dx >= 0 ? x1 : x2;
                y = dx >= 0 ? y1 : y2;
                endX = dx >= 0 ? x2 : x1;

                //Console.WriteLine($"P({x}, {y})");
                if (Contains(x, y))
                    points.Add(new System.Drawing.Point(x, y));

                for (int i = 0; x < endX; i++)
                {
                    x += 1;

                    if (px < 0)
                    {
                        px += 2 * rise;
                    }
                    else
                    {
                        y += both ? 1 : -1;
                        px += 2 * (rise - run);
                    }

                    //Console.WriteLine($"P({x}, {y})");
                    if (Contains(x, y))
                        points.Add(new System.Drawing.Point(x, y));
                }
            }
            else
            {
                x = dy >= 0 ? x1 : x2;
                y = dy >= 0 ? y1 : y2;
                endY = dy >= 0 ? y2 : y1;

                if (Contains(x, y))
                    points.Add(new System.Drawing.Point(x, y));

                for (int i = 0; y < endY; i++)
                {
                    y += 1;

                    if (py <= 0)
                    {
                        py += 2 * run;
                    }
                    else
                    {
                        x += both ? 1 : -1;
                        py += 2 * (run - rise);
                    }

                    //Console.WriteLine($"P({x}, {y})");
                    if (Contains(x, y))
                        points.Add(new System.Drawing.Point(x, y));
                }
            }

            return points;
        }

        public List<System.Drawing.Point> GetVisible(Triangle t)
        {
            System.Drawing.Point a = new System.Drawing.Point((int)MathF.Round(t.Points[0].X), (int)MathF.Round(t.Points[0].Y));
            System.Drawing.Point b = new System.Drawing.Point((int)MathF.Round(t.Points[1].X), (int)MathF.Round(t.Points[1].Y));
            System.Drawing.Point c = new System.Drawing.Point((int)MathF.Round(t.Points[2].X), (int)MathF.Round(t.Points[2].Y));

            List<System.Drawing.Point> points = new List<System.Drawing.Point>();

            points.AddRange(GetVisible(a.X, a.Y, b.X, b.Y));
            points.AddRange(GetVisible(b.X, b.Y, c.X, c.Y));
            points.AddRange(GetVisible(c.X, c.Y, a.X, a.Y));

            return points;
        }
    }
}

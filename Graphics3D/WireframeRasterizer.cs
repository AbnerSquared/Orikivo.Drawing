using System.Collections.Generic;
using Point = System.Drawing.Point;

namespace Orikivo.Drawing.Graphics3D
{
    public class WireframeRasterizer : Rasterizer
    {
        public override Grid<GammaColor> Render(in Model model, Camera camera, GammaPen pen)
        {
            Grid<GammaColor> frame = camera.GetScreen();

            for (int i = 0; i < model.Mesh.Triangles.Count; i++)
            {
                Triangle t = Transform(model.Mesh.Triangles[i], model.Transform);
                Triangle p = Project(t, camera.GetProjector(), camera.Width, camera.Height);

                List<System.Drawing.Point> visible = camera.GetVisible(p);

                foreach (System.Drawing.Point v in visible)
                    frame.SetValue(pen.Color, v.X, v.Y);
            }

            return frame;
        }
    }
}

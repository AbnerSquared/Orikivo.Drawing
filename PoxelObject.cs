using System;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Poxel
{
    // internal version of Bitmap
    // a generic poxel object that can be drawn, etc.
    public class PoxelObject : IDisposable
    {
        // since this enforces the other rulesets to work, as opposed to its framework variant.
        public PoxelObject()
        {

        }

        private Bitmap _source; // where all of the layers are rendered.
        // this is where the backgrounds would be set.
        public List<Bitmap> Layers { get; } = new List<Bitmap>();
        // this is where all of the other objects are rendered.

        public void DrawString(string content, FontFace font, ColorBrightness brightness, OutlineProperties properties)
        {

        }

        public void Dispose()
        {
            _source.Dispose();
        }
    }

    // a layer containing
    public class ObjectLayer
    {

    }
}

/*
 
    new PoxelObject(Size, ColorMap);
    PoxelObject.AddLayer();
    PoxelObject.Layers();


     
     */


using System.Collections.Generic;

namespace Orikivo.Poxel
{
    // defines the information needed to create a sheet class.
    public class SheetInfo
    {
        public Unit Unit;
        public Offset CropOffset; // if defined, provides information on how each box is cut. This always overrides manual sprite info.
        public string Url;
        public List<SpriteInfo> Sprites;

    }
}

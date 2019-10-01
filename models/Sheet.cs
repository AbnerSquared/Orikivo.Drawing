using System;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Poxel
{
    public class Sheet : ISprite
    {
        public Sheet(SheetInfo info)
        {
            Url = info.Url;
            Source = new Bitmap(Url);
            Sprites = new List<CroppedSprite>();
            if (info.Unit != null)
            {
                if (Source.Width % info.Unit.Width != 0 || Source.Height % info.Unit.Height != 0)
                    throw new Exception("The crop info in correlation to the Bitmap must be evenly cut.");
                // make offsets to force it to be even.
                int xLen = Source.Width / info.Unit.Width;
                int yLen = Source.Height / info.Unit.Height;
                for (int n = 0; n < yLen; n++)
                {
                    for (int m = 0; m < xLen; m++)
                    {
                        Sprites.Add(new CroppedSprite(Url, m * info.Unit.Width, n * info.Unit.Height, info.Unit.Width, info.Unit.Height));
                    }
                }
            }
        }
        public string Url { get; }
        public Bitmap Source { get; }
        public List<CroppedSprite> Sprites { get; }

        public CroppedSprite this[int index]
        {
            get
            {
                return Sprites[index];
            }
        }

        public Bitmap GetSprite(int index)
        {
            // create a crop pointing to a part of the main sheet.
            return null;
        }
    }
}

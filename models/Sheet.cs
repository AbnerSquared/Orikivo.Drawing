using System;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Poxel
{
    public class Sheet
    {
        public Sheet(SheetInfo info)
        {
            Url = info.Url;
            Source = new Bitmap(Url);
            Sprites = new List<CroppedSprite>();
            if (info.CropInfo != null)
            {
                if (Source.Width % info.CropInfo.Width != 0 || Source.Height % info.CropInfo.Height != 0)
                    throw new Exception("The crop info in correlation to the Bitmap must be evenly cut.");
                // make offsets to force it to be even.
                int xLen = Source.Width / info.CropInfo.Width;
                int yLen = Source.Height / info.CropInfo.Height;
                for (int n = 0; n < yLen; n++)
                {
                    for (int m = 0; m < xLen; m++)
                    {
                        Sprites.Add(new CroppedSprite(Url, m * info.CropInfo.Width, n * info.CropInfo.Height, info.CropInfo.Width, info.CropInfo.Height));
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

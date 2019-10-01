using System;
using System.Drawing;
using System.Linq;

namespace Orikivo.Poxel
{

    public static class PoxelUtils
    {
        private static Size Bounds16_9 = new Size(400, 225);
        private static Size Bounds4_3 = new Size(400, 300);
        private static Size Bounds1_1 = new Size(300, 300);
        private static Size Bounds1_2 = new Size(400, 200);
        private static Size Bounds2_1 = new Size(150, 300);

        private static Size Thumbs16_9 = new Size(80, 45);
        private static Size Thumbs4_3 = new Size(80, 60);
        private static Size Thumbs1_1 = new Size(80, 80);
        private static Size Thumbs1_2 = new Size(80, 40);
        private static Size Thumbs2_1 = new Size(40, 80);

        /// <summary>
        /// Returns the size of the specified ratio and type for a Discord.Embed.
        /// </summary>
        /// <param name="ratio"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Size GetRatioDims(EmbedMediaRatio ratio, EmbedMediaType type)
        {
            switch (ratio)
            {
                case EmbedMediaRatio.Widescreen:
                    return type == EmbedMediaType.Thumbnail ? Thumbs16_9 : Bounds16_9;
                case EmbedMediaRatio.Wide:
                    return type == EmbedMediaType.Thumbnail ? Thumbs2_1 : Bounds2_1;
                case EmbedMediaRatio.Rectangle:
                    return type == EmbedMediaType.Thumbnail ? Thumbs4_3 : Bounds4_3;
                case EmbedMediaRatio.Square:
                    return type == EmbedMediaType.Thumbnail ? Thumbs1_1 : Bounds1_1;
                case EmbedMediaRatio.Tall:
                    return type == EmbedMediaType.Thumbnail ? Thumbs1_2 : Bounds1_2;
                default:
                    throw new Exception("The ratio type specified is not a valid ratio.");
            }
        }

        internal static (int i, int x, int y) GetCharIndex(char c, char[][][][] charMap)
        {
            (int i, int x, int y) pos = new ValueTuple<int, int, int>();
            
            foreach(char[][][] map in charMap)
            {
                if (map.Any(x => x.Any(y => y.Contains(c))))
                {
                    pos.i = charMap.ToList().IndexOf(map);
                    foreach (char[][] row in map)
                    {
                        if (row.Any(x => x.Contains(c)))
                        {
                            pos.y = map.ToList().IndexOf(row);
                            foreach(char[] item in row)
                            {
                                if (item.Contains(c))
                                {
                                    pos.x = row.ToList().IndexOf(item);
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
                break;
            }
            return pos;
        }
    }
}

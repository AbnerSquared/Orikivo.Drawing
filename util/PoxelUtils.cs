using System;
using System.Drawing;
using System.Linq;

namespace Orikivo.Poxel
{
    public enum BitmapRatio
    {
        SixteenToNine = 1, // 16:9
        FourToThree = 2, // 4:3
        OneToOne = 3, // 1:1
        TwoToOne = 4, // 2:1
    }

    public enum EmbedMediaType
    {
        Thumbnail = 1,
        Image = 2
    }

    public static class PoxelUtils
    {
        public static Size Bounds16_9 = new Size(400, 225);
        public static Size Bounds4_3 = new Size(400, 300);
        public static Size Bounds1_1 = new Size(300, 300);
        public static Size Bounds1_2 = new Size(400, 200);
        public static Size Bounds2_1 = new Size(150, 300);

        public static Size Thumbs16_9 = new Size(80, 45);
        public static Size Thumbs4_3 = new Size(80, 60);
        public static Size Thumbs1_1 = new Size(80, 80);
        public static Size Thumbs1_2 = new Size(80, 40);
        public static Size Thumbs2_1 = new Size(40, 80);

        public static (int i, int x, int y) GetCharIndex(char c, char[][][][] charMap)
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

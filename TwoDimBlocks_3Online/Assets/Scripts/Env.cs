using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace YasGameLib
{
    public static class Env
    {
        public const int BlockN = 8;
        public const int ChunkN = 8;
        public const int TotalPixels = BlockN * ChunkN;

        public static int[,] ConvertListToArray(List<int> list, int TotalBlockN)
        {
            int[,] ret = new int[TotalBlockN, TotalBlockN];

            int index = 0;
            for (int x = 0; x < TotalBlockN; x++)
            {
                for (int y = 0; y < TotalBlockN; y++)
                {
                    ret[x, y] = list[index++];
                }
            }

            return ret;
        }

        public static Color ConvertIntToColor(int color)
        {
            byte[] bytes = BitConverter.GetBytes(color);
            return new Color32(bytes[0], bytes[1], bytes[2], bytes[3]);
        }

        public static int ConvertColorToInt(Color color)
        {
            Color32 c32 = color;
            byte[] bytes = new byte[4] { c32.r, c32.g, c32.b, c32.a };
            return BitConverter.ToInt32(bytes, 0);
        }

        public static bool InsideCanvas(int x, int y)
        {
            if(x < 0 || x >= TotalPixels || y < 0 || y >= TotalPixels)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsTransparentColor(int color)
        {
            byte[] bytes = BitConverter.GetBytes(color);

            return bytes[3] != 255;
        }
    }
}

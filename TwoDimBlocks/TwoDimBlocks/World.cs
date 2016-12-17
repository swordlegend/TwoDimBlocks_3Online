using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YasGameLib;

namespace TwoDimBlocks
{
    public static class World
    {
        static int[,] bot = new int[Env.TotalPixels, Env.TotalPixels];
        static int[,] front = new int[Env.TotalPixels, Env.TotalPixels];

        static List<PixelData> buffer = new List<PixelData>();

        public static BMPInitData GetBMP()
        {
            BMPInitData bmp = new BMPInitData();
            bmp.botbmp.AddRange(bot.Cast<int>());
            bmp.frontbmp.AddRange(front.Cast<int>());

            return bmp;
        }

        public static void Buffer(DrawData draw)
        {
            foreach(PixelData pixel in draw.pixels)
            {
                PixelData old = buffer.FirstOrDefault(f => f.isbot == pixel.isbot && f.x == pixel.x && f.y == pixel.y);
                if(old != null)
                {
                    old.color = pixel.color;
                }
                else
                {
                    buffer.Add(pixel);
                }
            }
        }

        public static List<PixelData> GetBuffer()
        {
            foreach(PixelData pixel in buffer)
            {
                if(pixel.isbot)
                {
                    bot[pixel.x, pixel.y] = pixel.color;
                }
                else
                {
                    front[pixel.x, pixel.y] = pixel.color;
                }
            }

            List<PixelData> ret = buffer;
            buffer = new List<PixelData>();
            return ret;
        }
    }
}

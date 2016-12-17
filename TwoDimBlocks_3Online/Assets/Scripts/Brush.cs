using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using YasGameLib;

namespace TwoDimBlocks
{
    public static class Brush
    {
        static int color_int;
        static Color color;

        public static int Color_Int
        {
            get { return Brush.color_int; }
            set
            {
                Brush.color_int = value;
                Brush.color = Env.ConvertIntToColor(value);
            }
        }

        public static Color Color
        {
            get { return Brush.color; }
            set
            {
                Brush.color = value;
                Brush.color_int = Env.ConvertColorToInt(value);
            }
        }

        public static int Radius;
    }
}

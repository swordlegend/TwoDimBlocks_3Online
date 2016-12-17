using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwoDimBlocks
{
    public class IntVec3Color
    {
        public IntVec3 vec;
        public int color;
        public IntVec3Color(IntVec3 vec, int color)
        {
            this.vec = vec;
            this.color = color;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoDimBlocks
{
    public static class ShaderManager
    {
        static Dictionary<int, Material> cache = new Dictionary<int, Material>();

        public static Material GetShader(int color)
        {
            if (cache.ContainsKey(color))
            {
                return cache[color];
            }
            else
            {
                Material mat = new Material(Shader.Find("Standard"));
                byte[] bytes = BitConverter.GetBytes(color);
                mat.color = new Color32(bytes[0], bytes[1], bytes[2], bytes[3]);

                cache.Add(color, mat);

                return mat;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using YasGameLib;

namespace TwoDimBlocks
{
    public class WorldScript : MonoBehaviour
    {
        int[,,] blocks;
        GameObject[,,] chunks;

        void Start()
        {
            blocks = new int[Env.TotalPixels, Env.TotalPixels, Env.TotalPixels];
            //SetDummy();

            chunks = new GameObject[Env.ChunkN, Env.ChunkN, Env.ChunkN];
            for (int z = 0; z < Env.ChunkN; z++)
            {
                for (int y = 0; y < Env.ChunkN; y++)
                {
                    for (int x = 0; x < Env.ChunkN; x++)
                    {
                        chunks[x, y, z] = (GameObject)Instantiate(Resources.Load("Chunk"), transform);
                        chunks[x, y, z].transform.position = new Vector3(x * Env.BlockN, y * Env.BlockN, z * Env.BlockN);
                        chunks[x, y, z].GetComponent<ChunkScript>().Init(blocks, x, y, z);
                    }
                }
            }
        }

        void SetDummy()
        {
            blocks[12, 2, 12] = BitConverter.ToInt32(new byte[] { 255, 255, 255, 255 }, 0);
            blocks[13, 2, 13] = BitConverter.ToInt32(new byte[] { 255, 255, 255, 255 }, 0);
            blocks[13, 4, 13] = BitConverter.ToInt32(new byte[] { 255, 255, 255, 255 }, 0);
        }

        /*public void SetBlock(List<IntVec3> items)
        {
            List<IntVec3> f_chunks = new List<IntVec3>();

            foreach (IntVec3 block in items)
            {
                blocks[block.x, block.y, block.z] = BitConverter.ToInt32(new byte[] { 255, 255, 255, 255 }, 0);
                IntVec3 flag = new IntVec3(block.x / 8, block.y / 8, block.z / 8);

                if (f_chunks.FirstOrDefault(f => f.x == flag.x && f.y == flag.y && f.z == flag.z) == null)
                {
                    f_chunks.Add(flag);
                }
            }

            foreach (IntVec3 vec in f_chunks)
            {
                Destroy(chunks[vec.x, vec.y, vec.z]);
                chunks[vec.x, vec.y, vec.z] = (GameObject)Instantiate(Resources.Load("Chunk"), transform);
                chunks[vec.x, vec.y, vec.z].transform.position = new Vector3(vec.x * 8, vec.y * 8, vec.z * 8);
                chunks[vec.x, vec.y, vec.z].GetComponent<ChunkScript>().Init(blocks, vec.x, vec.y, vec.z);
            }
        }*/

        public void SetBlocks(List<IntVec3Color> items)
        {
            List<IntVec3> flags = new List<IntVec3>();

            foreach (IntVec3Color item in items)
            {
                blocks[item.vec.x, item.vec.y, item.vec.z] = item.color;
                IntVec3 flag = new IntVec3(item.vec.x / Env.BlockN, item.vec.y / Env.BlockN, item.vec.z / Env.BlockN);

                if (flags.FirstOrDefault(f => f.x == flag.x && f.y == flag.y && f.z == flag.z) == null)
                {
                    flags.Add(flag);
                }
            }

            foreach (IntVec3 vec in flags)
            {
                Destroy(chunks[vec.x, vec.y, vec.z]);
                chunks[vec.x, vec.y, vec.z] = (GameObject)Instantiate(Resources.Load("Chunk"), transform);
                chunks[vec.x, vec.y, vec.z].transform.position = new Vector3(vec.x * Env.BlockN, vec.y * Env.BlockN, vec.z * Env.BlockN);
                chunks[vec.x, vec.y, vec.z].GetComponent<ChunkScript>().Init(blocks, vec.x, vec.y, vec.z);
            }
        }

        public int GetColor(int x, int y, int z)
        {
            return blocks[x, y, z];
        }
    }
}
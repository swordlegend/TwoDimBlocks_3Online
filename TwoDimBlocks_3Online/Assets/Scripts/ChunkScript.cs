using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using YasGameLib;

namespace TwoDimBlocks
{
    public class ChunkScript : MonoBehaviour
    {
        int[,,] blocks;
        int x_chunk_num, y_chunk_num, z_chunk_num;

        Dictionary<int, GameObject> childs = new Dictionary<int, GameObject>();

        public void Init(int[,,] blocks, int x_chunk_num, int y_chunk_num, int z_chunk_num)
        {
            this.blocks = blocks;
            this.x_chunk_num = x_chunk_num;
            this.y_chunk_num = y_chunk_num;
            this.z_chunk_num = z_chunk_num;

            Construct();
        }

        void Construct()
        {
            for (int z = 0; z < Env.BlockN; z++)
            {
                for (int y = 0; y < Env.BlockN; y++)
                {
                    for (int x = 0; x < Env.BlockN; x++)
                    {
                        if ((blocks[x_chunk_num * Env.BlockN + x, y_chunk_num * Env.BlockN + y, z_chunk_num * Env.BlockN + z] & 0xff000000) != 0)
                        {
                            if (!BlockIsHide(x_chunk_num * Env.BlockN + x, y_chunk_num * Env.BlockN + y, z_chunk_num * Env.BlockN + z))
                            {
                                //GameObject obj = (GameObject)Instantiate(Resources.Load("Cube"), transform);
                                //obj.transform.position = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                                int color = blocks[x_chunk_num * Env.BlockN + x, y_chunk_num * Env.BlockN + y, z_chunk_num * Env.BlockN + z];
                                if (!childs.ContainsKey(color))
                                {
                                    childs.Add(color, (GameObject)Instantiate(Resources.Load("Combine"), transform));
                                }

                                GameObject obj = (GameObject)Instantiate(Resources.Load("Cube"), childs[color].transform);
                                obj.transform.position = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                            }
                        }
                    }
                }
            }

            Combine();

            Destroy(this);
        }

        void Combine()
        {
            foreach(KeyValuePair<int, GameObject> item in childs)
            {
                item.Value.GetComponent<CombineScript>().Combine();
                item.Value.GetComponent<Renderer>().material = ShaderManager.GetShader(item.Key);
            }
            /*if (GetComponent<MeshFilter>() != null)
            {
                Destroy(GetComponent<MeshFilter>());
            }

            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                //meshFilters[i].gameObject.SetActive(false);
                i++;
            }
            gameObject.AddComponent<MeshFilter>();

            transform.GetComponent<MeshFilter>().mesh = new Mesh();
            transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().mesh;
            transform.gameObject.SetActive(true);

            i = 0;
            while (i < meshFilters.Length)
            {
                if (meshFilters[i].gameObject != gameObject)
                    Destroy(meshFilters[i].gameObject);
                i++;
            }

            //GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
            GetComponent<Renderer>().material = ShaderManager.GetShader(BitConverter.ToInt32(new byte[] { 128, 128, 128, 255 }, 0));**/
        }

        bool BlockIsHide(int x, int y, int z)
        {
            if (x % Env.BlockN == 0 || x % Env.BlockN == Env.BlockN - 1 ||
                y % Env.BlockN == 0 || y % Env.BlockN == Env.BlockN - 1 ||
                z % Env.BlockN == 0 || z % Env.BlockN == Env.BlockN - 1)
            {
                return false;
            }

            /*if (x == 0 || x == Env.TotalPixels - 1 ||
                y == 0 || y == Env.TotalPixels - 1 ||
                z == 0 || z == Env.TotalPixels - 1)
            {
                return false;
            }*/

            if (Env.IsTransparentColor(blocks[x - 1, y, z]) || Env.IsTransparentColor(blocks[x + 1, y, z]) ||
                Env.IsTransparentColor(blocks[x, y - 1, z]) || Env.IsTransparentColor(blocks[x, y + 1, z]) ||
                Env.IsTransparentColor(blocks[x, y, z - 1]) || Env.IsTransparentColor(blocks[x, y, z + 1]))
            {
                return false;
            }

            return true;
        }
    }
}

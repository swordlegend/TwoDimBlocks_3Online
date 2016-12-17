using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoDimBlocks
{
    public class CombineScript : MonoBehaviour
    {
        public void Combine()
        {
            if (GetComponent<MeshFilter>() != null)
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
            //GetComponent<Renderer>().material = ShaderManager.GetShader(BitConverter.ToInt32(new byte[] { 128, 128, 128, 255 }, 0));

            Destroy(this);
        }
    }
}

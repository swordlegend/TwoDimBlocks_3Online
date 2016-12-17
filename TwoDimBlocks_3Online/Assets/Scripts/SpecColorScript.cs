using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoDimBlocks
{
    public class SpecColorScript : MonoBehaviour
    {
        [SerializeField]
        Color color;
        [SerializeField]
        Image image;
        // Use this for initialization
        void Start()
        {
            image.color = color;
        }

        public void Click()
        {
            Brush.Color = color;
            GameObject.Find("Canvas").GetComponent<UIScript>().SetSpecColor(color);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoDimBlocks
{
    public class SpecRadScript : MonoBehaviour
    {
        [SerializeField]
        int radius;
        [SerializeField]
        RectTransform rect;
        // Use this for initialization
        void Start()
        {
            int img_size = 1 + radius * 2;
            rect.sizeDelta = new Vector2(img_size * 2, img_size * 2);
        }

        public void Click()
        {
            Brush.Radius = radius;
            GameObject.Find("Canvas").GetComponent<UIScript>().SetRadius(radius);
        }
    }

}

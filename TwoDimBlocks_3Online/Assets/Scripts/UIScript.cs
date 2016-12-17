using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TwoDimBlocks
{
    public class UIScript : MonoBehaviour
    {
        Camera main;
        GameObject background, botCanvas, frntCanvas, clrPalette, radPalette;
        [SerializeField]
        Image color_image;
        [SerializeField]
        Slider r_slider, g_slider, b_slider, rad_slider;
        [SerializeField]
        RectTransform rad_image;
        void Start()
        {
            main = Camera.main;

            background = GameObject.Find("CanvasBG");
            background.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, Screen.height);
            background.GetComponent<RectTransform>().anchoredPosition = new Vector2(-Screen.width / 4, 0);

            botCanvas = GameObject.Find("BottomCanvas");
            frntCanvas = GameObject.Find("FrontCanvas");
            /*botCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 4, Screen.width / 4);
            botCanvas.GetComponent<RectTransform>().anchoredPosition = new Vector2(-Screen.width / 4, 0);
            frntCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 4, Screen.width / 4);
            frntCanvas.GetComponent<RectTransform>().anchoredPosition = new Vector2(-Screen.width / 4, 0);*/

            clrPalette = GameObject.Find("ColorPalette");
            clrPalette.GetComponent<RectTransform>().anchoredPosition = new Vector2(-Screen.width / 16, 105);
            radPalette = GameObject.Find("RadiusPalette");
            radPalette.GetComponent<RectTransform>().anchoredPosition = new Vector2(-Screen.width / 16, -105);

            Brush.Color = Color.black;
            SetColor();
            SetRadius();

            DisableAllUI();
        }

        void DisableAllUI()
        {
            background.SetActive(false);
            botCanvas.SetActive(false);
            frntCanvas.SetActive(false);
            clrPalette.SetActive(false);
            radPalette.SetActive(false);
        }

        public void SetInitData(BMPInitData bmps)
        {
            botCanvas.GetComponent<CanvasScript>().SetPixels(bmps.botbmp);
            frntCanvas.GetComponent<CanvasScript>().SetPixels(bmps.frontbmp);

            frntCanvas.GetComponent<CanvasScript>().SetAllBlocks();

            BottomButtonDown();
        }

        public void BottomButtonDown()
        {
            SetOrthographic();

            botCanvas.SetActive(true);
            botCanvas.GetComponent<CanvasScript>().SetAwake();
            frntCanvas.SetActive(false);

            clrPalette.SetActive(true);
            radPalette.SetActive(true);

            Camera.main.GetComponent<CameraScript>().SetFollowState(false);
        }

        public void FrontButtonDown()
        {
            SetOrthographic();

            botCanvas.SetActive(false);
            frntCanvas.SetActive(true);
            frntCanvas.GetComponent<CanvasScript>().SetAwake();

            clrPalette.SetActive(true);
            radPalette.SetActive(true);

            Camera.main.GetComponent<CameraScript>().SetFollowState(false);
        }

        public void TPSButtonDown()
        {
            SetPerspective();

            botCanvas.SetActive(false);
            frntCanvas.SetActive(false);

            clrPalette.SetActive(false);
            radPalette.SetActive(false);

            Camera.main.GetComponent<CameraScript>().SetFollowState(true);
        }

        void SetOrthographic()
        {
            main.orthographic = true;
            main.orthographicSize = 64;
            main.transform.position = new Vector3(-50, 100, -50);
            main.transform.rotation = Quaternion.Euler(45, 45, 0);

            main.rect = new Rect(0.5f, 0.0f, 1.0f, 1.0f);

            background.SetActive(true);
            background.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, Screen.height);
            background.GetComponent<RectTransform>().anchoredPosition = new Vector2(-Screen.width / 4, 0);
        }

        void SetPerspective()
        {
            main.orthographic = false;
            main.transform.position = new Vector3(0, 0, 0);
            main.transform.rotation = Quaternion.Euler(0, 0, 0);

            main.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

            background.SetActive(false);
        }

        public void SetColor()
        {
            byte[] bytes = new byte[4] { (byte)(r_slider.value), (byte)(g_slider.value), (byte)(b_slider.value), ((Color32)Brush.Color).a };
            Brush.Color_Int = BitConverter.ToInt32(bytes, 0);

            color_image.color = Brush.Color;
        }

        public void SetSpecColor(Color32 color)
        {
            r_slider.value = color.r;
            g_slider.value = color.g;
            b_slider.value = color.b;

            color_image.color = color;
        }

        public void SetRadius()
        {
            int rad = (int)rad_slider.value;
            Brush.Radius = rad;

            int img_size = 1 + rad * 2;
            rad_image.sizeDelta = new Vector2(img_size * 2, img_size * 2);
        }

        public void SetRadius(int radius)
        {
            rad_slider.value = radius;

            int img_size = 1 + radius * 2;
            rad_image.sizeDelta = new Vector2(img_size * 2, img_size * 2);
        }

        public void AddBlocks(List<PixelData> pixels)
        {
            botCanvas.GetComponent<CanvasScript>().AddPixels(pixels.Where(f => f.isbot).ToList<PixelData>());
            frntCanvas.GetComponent<CanvasScript>().AddPixels(pixels.Where(f => !f.isbot).ToList<PixelData>());
        }
    }
}

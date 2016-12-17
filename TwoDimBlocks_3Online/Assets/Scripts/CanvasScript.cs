using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using YasGameLib;

namespace TwoDimBlocks
{
    public class CanvasScript : MonoBehaviour
    {
        int[,] pixels;
        bool awake = false;
        float width;
        //Brush brush;

        [SerializeField]
        int surface;
        [SerializeField]
        CanvasScript other;
        /*[SerializeField]
        UIScript ui;*/
        [SerializeField]
        WorldScript world;
        void Start()
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 4, Screen.width / 4);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(-Screen.width / 4, 0);

            width = GetComponent<RectTransform>().rect.width;

            EventTrigger trigger = GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
            trigger.triggers.Add(entry);

            EventTrigger.Entry beginEntry = new EventTrigger.Entry();
            beginEntry.eventID = EventTriggerType.PointerDown;
            beginEntry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
            trigger.triggers.Add(beginEntry);

            //brush = GameObject.Find("Canvas").GetComponent<UIScript>().GetBrush();
        }

        // Update is called once per frame
        void Update()
        {
            if (awake)
            {
                awake = false;
                Refresh();
            }
        }

        public void SetPixels(List<int> list)
        {
            pixels = Env.ConvertListToArray(list, Env.TotalPixels);
        }

        
        public void SetAwake()
        {
            awake = true;
        }

        public void OnDrag(PointerEventData data)
        {
            //Debug.Log(transform.InverseTransformPoint(data.position));
            Vector2 localPos = transform.InverseTransformPoint(data.position) + new Vector3(width / 2, width / 2, 0);
            localPos *= ((Env.TotalPixels - 1) / width);
            //Debug.Log(localPos);
            /*float real_x = localPos.x;
            float rate = real_x / width;
            float fix_x = 255.0f * rate;
            Debug.Log(fix_x);*/
            //Debug.Log(localPos);

            /*if(Env.PointerIsOutOfBound(localPos))
            {
                return;
            }*/

            if (data.button == PointerEventData.InputButton.Left)
            {
                //DrawPixel(localPos, brush.color, brush.radius / 2);
                Dictionary<int, List<int>> rewrites = DrawPixels(localPos, Brush.Color_Int, Brush.Radius);

                if(rewrites.Count != 0)
                {
                    Refresh();

                    world.SetBlocks(GetBlocks(rewrites));

                    SendDrawData(rewrites);
                }
            }
            /*else if (data.button == PointerEventData.InputButton.Right)
            {
                pixels[(int)localPos.x, (int)localPos.y] = 0;
            }*/

            //Refresh();
        }

        public void SetAllBlocks()
        {
            Dictionary<int, List<int>> rewrites = new Dictionary<int, List<int>>();
            for(int x=0; x<Env.TotalPixels; x++)
            {
                List<int> y_array = new List<int>();
                for(int y=0; y<Env.TotalPixels; y++)
                {
                    y_array.Add(y);
                }
                rewrites.Add(x, y_array);
            }

            world.SetBlocks(GetBlocks(rewrites));
        }

        public void AddPixels(List<PixelData> data)
        {
            Dictionary<int, List<int>> rewrites = new Dictionary<int, List<int>>();

            foreach(PixelData pic in data)
            {
                pixels[pic.x, pic.y] = pic.color;
                if(rewrites.ContainsKey(pic.x))
                {
                    rewrites[pic.x].Add(pic.y);
                }
                else
                {
                    List<int> list = new List<int>();
                    list.Add(pic.y);
                    rewrites.Add(pic.x, list);
                }
            }

            Refresh();

            world.SetBlocks(GetBlocks(rewrites));
        }

        void SendDrawData(Dictionary<int, List<int>> rewrites)
        {
            DrawData draw = new DrawData();

            foreach(KeyValuePair<int, List<int>> item in rewrites)
            {
                foreach(int y in item.Value)
                {
                    if(surface == 0)
                    {
                        draw.pixels.Add(new PixelData() { isbot = true, x = item.Key, y = y, color = pixels[item.Key, y] });
                    }
                    else
                    {
                        draw.pixels.Add(new PixelData() { isbot = false, x = item.Key, y = y, color = pixels[item.Key, y] });
                    }
                }
            }

            GCli.Send(MessageType.Draw, GCli.Serialize<DrawData>(draw), Lidgren.Network.NetDeliveryMethod.Unreliable);
        }

        void Refresh()
        {
            Texture2D texture = new Texture2D(Env.TotalPixels, Env.TotalPixels);
            GetComponent<CanvasRenderer>().SetMaterial(GetComponent<CanvasRenderer>().GetMaterial(), texture);

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    //Color color = pixels[x, y] == 0 ? Color.white : Color.black;
                    if ((pixels[x, y] & 0xff000000) == 0)
                    {
                        texture.SetPixel(x, y, Color.white);
                    }
                    else
                    {
                        byte[] bytes = BitConverter.GetBytes(pixels[x, y]);
                        Color color = new Color32(bytes[0], bytes[1], bytes[2], bytes[3]);
                        texture.SetPixel(x, y, color);
                    }
                }
            }
            texture.Apply();
        }

        Dictionary<int, List<int>> DrawPixels(Vector2 center, int color, int radius)
        {
            Dictionary<int, List<int>> rewrites = new Dictionary<int, List<int>>();

            int center_x = (int)center.x;
            int center_y = (int)center.y;

            for (int y = -radius + 1; y < radius; y++)
            {
                for (int x = -radius + 1; x < radius; x++)
                {
                    if(InsideCircle(x, y, radius))
                    {
                        int target_x = center_x + x;
                        int target_y = center_y + y;

                        if(Env.InsideCanvas(target_x, target_y) && pixels[target_x, target_y] != color)
                        {
                            pixels[target_x, target_y] = color;
                            if(rewrites.ContainsKey(target_x))
                            {
                                rewrites[target_x].Add(target_y);
                            }
                            else
                            {
                                List<int> list = new List<int>();
                                list.Add(target_y);
                                rewrites.Add(target_x, list);
                            }
                        }
                    }
                }
            }

            return rewrites;
        }

        bool InsideCircle(int x, int y, int radius)
        {
            return x * x + y * y <= radius * radius;
        }

        List<IntVec3Color> GetBlocks(Dictionary<int, List<int>> rewrites)
        {
            List<IntVec3Color> blocks = new List<IntVec3Color>();

            foreach(KeyValuePair<int, List<int>> item in rewrites)
            {
                int x = item.Key;

                List<int> over_y = item.Value;

                foreach(int my_y in over_y)
                {
                    for(int y = 0; y < Env.TotalPixels; y++)
                    {
                        int old_color = surface == 0 ? world.GetColor(x, y, my_y) : world.GetColor(x, my_y, y);

                        int new_color = CalcColor(pixels[x, my_y], other.GetColor(x, y));

                        if(!Env.IsTransparentColor(old_color) || !Env.IsTransparentColor(new_color))
                        {
                            blocks.Add(new IntVec3Color(surface == 0 ? new IntVec3(x, y, my_y) : new IntVec3(x, my_y, y), new_color));
                        }
                    }
                }

                /*for(int y=0; y<other_y.Length; y++)
                {
                    if((other_y[y] & 0xff000000) != 0)
                    {
                        foreach(int my_y in item.Value)
                        {
                            if(surface == 0)
                            {
                                blocks.Add(new IntVec3Color(new IntVec3(x, y, my_y), CalcColor(pixels[x, my_y], other.GetColor(x, y))));
                            }
                            else
                            {
                                blocks.Add(new IntVec3Color(new IntVec3(x, my_y, y), CalcColor(pixels[x, my_y], other.GetColor(x, y))));
                            }
                        }
                    }
                }*/
            }

            return blocks;
        }

        public int[] GetYPixels(int x)
        {
            int[] ret = new int[Env.TotalPixels];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = pixels[x, i];
            }

            return ret;
        }

        public int GetColor(int x, int y)
        {
            return pixels[x, y];
        }

        int CalcColor(int left, int right)
        {
            byte[] lefts = BitConverter.GetBytes(left);
            byte[] rights = BitConverter.GetBytes(right);

            byte[] ret = new byte[4];
            ret[0] = (byte)((lefts[0] + rights[0]) / 2);
            ret[1] = (byte)((lefts[1] + rights[1]) / 2);
            ret[2] = (byte)((lefts[2] + rights[2]) / 2);
            ret[3] = (byte)((lefts[3] + rights[3]) / 2);
            if(ret[3] != 255)
            {
                ret[3] = 0;
            }

            return BitConverter.ToInt32(ret, 0);
        }

        /*void DrawPixel(Vector2 pos, int color, int radius)
        {
            int world_x = (int)pos.x;
            int world_y = (int)pos.y;

            List<IntVector3> addBlocks = new List<IntVector3>();

            for (int y = -radius; y < radius; y++)
            {
                for (int x = -radius; x < radius; x++)
                {
                    if (Math.Pow(radius, 2) >= Math.Pow(x, 2) + Math.Pow(y, 2))
                    {
                        int tar_x = world_x + x;
                        int tar_y = world_y + y;
                        if (!PointerIsOutOfBound(new Vector2(tar_x, tar_y)))
                        {
                            //pixels[tar_x, tar_y] = BitConverter.ToInt32(BitConverter.GetBytes(0xffffffff), 0);
                            pixels[tar_x, tar_y] = color;

                            int[] other_y = surface == 0 ? ui.GetCanvasScript(1).GetYPixels(tar_x) : ui.GetCanvasScript(0).GetYPixels(tar_x);
                            //int sum = 0;
                            for (int i = 0; i < other_y.Length; i++)
                            {
                                if ((color & 0xff000000) != 0 && (other_y[i] & 0xff000000) != 0)
                                {
                                    //sum++;
                                    if (surface == 0)
                                    {
                                        AddBlocks(addBlocks, new IntVector3(tar_x, i, tar_y));
                                    }
                                    else
                                    {
                                        AddBlocks(addBlocks, new IntVector3(tar_x, tar_y, i));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (addBlocks.Count != 0)
            {
                //Debug.Log(addBlocks.Count);
                world.SetBlock(addBlocks);
            }
        }

        public void AddBlocks(List<IntVector3> addBlocks, IntVector3 add)
        {
            if (addBlocks.FirstOrDefault(x => x.x == add.x && x.y == add.y && x.z == add.z) == null)
            {
                addBlocks.Add(add);
            }
        }

        bool PointerIsOutOfBound(Vector2 pos)
        {
            int x = (int)pos.x;
            int y = (int)pos.y;

            if (x < 0 || x >= 256 || y < 0 || y >= 256)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int[] GetYPixels(int x)
        {
            int[] ret = new int[256];
            for (int i = 0; i < 256; i++)
            {
                ret[i] = pixels[x, i];
            }

            return ret;
        }

        public int AddColor(int c0, int c1)
        {
            byte[] left = BitConverter.GetBytes(c0);
            byte[] right = BitConverter.GetBytes(c1);

            byte[] ret = new byte[4];

            ret[0] = (byte)(left[0] + right[0]) >= 255 ? (byte)255 : (byte)(left[0] + right[0]);
            ret[1] = (byte)(left[1] + right[1]) >= 255 ? (byte)255 : (byte)(left[1] + right[1]);
            ret[2] = (byte)(left[2] + right[2]) >= 255 ? (byte)255 : (byte)(left[2] + right[2]);
            ret[3] = (byte)(left[3] + right[3]) >= 255 ? (byte)255 : (byte)(left[3] + right[3]);

            return BitConverter.ToInt32(ret, 0);
        }*/
    }
}

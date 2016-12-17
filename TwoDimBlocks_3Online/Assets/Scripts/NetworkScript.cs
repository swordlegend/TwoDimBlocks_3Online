using Lidgren.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using YasGameLib;

namespace TwoDimBlocks
{
    public class NetworkScript : MonoBehaviour
    {
        string host;
        int port;

        float frameSpan = 0;

        [SerializeField]
        UIScript ui;

        void Start()
        {
            if (!ReadFile(Application.dataPath + "/../Setting/setting.txt"))
            {
                return;
            }

            GCli.Init();
            GCli.SetConnectPacketHandler(ConnectHandler);
            //GCli.SetDebugPacketHandler(DebugHandler);

            GCli.Connect("TwoDimBlocks0.1", host, port);
        }

        InitData GetDummyInitData()
        {
            PlayerSyncData sync = new PlayerSyncData { userid = 1, xpos = 10.0f, ypos = 0, zpos = 10.0f, yrot = 0, anime = 0 };
            BMPInitData bmps = new BMPInitData();

            int[,] bot_blocks = new int[Env.TotalPixels, Env.TotalPixels];
            bmps.botbmp.AddRange(bot_blocks.Cast<int>());
            bmps.frontbmp.AddRange(bot_blocks.Cast<int>());

            return new InitData { sync = sync, bmps = bmps };
        }


        bool ReadFile(string path)
        {
            FileInfo fi = new FileInfo(path);
            try
            {
                using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
                {
                    while (true)
                    {
                        string line = sr.ReadLine();

                        if (line == null)
                        {
                            break;
                        }

                        if (line.StartsWith("Host="))
                        {
                            string sub = line.Substring("Host=".Length);
                            host = sub;
                        }
                        else if (line.StartsWith("Port="))
                        {
                            string sub = line.Substring("Port=".Length);
                            if (!int.TryParse(sub, out port))
                            {
                                throw new Exception();
                            }
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        bool firstFrame = true;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            GCli.Receive();

            frameSpan += Time.deltaTime;
            if (frameSpan >= 0.1f)
            {
                frameSpan = 0;

                PushData push = Players.GetPushData();
                if(push != null)
                {
                    GCli.Send(MessageType.Push, GCli.Serialize<PushData>(push), NetDeliveryMethod.UnreliableSequenced);
                }
            }
            /*if(firstFrame)
            {
                firstFrame = false;
                InitData init = GetDummyInitData();
                GameObject.Find("Canvas").GetComponent<UIScript>().SetInitData(init.bmps);
            }*/
        }

        void FixedUpdate()
        {
            Players.FixedUpdate(Time.deltaTime);
        }

        void OnDestroy()
        {
            GCli.Shutdown();
        }

        public void ConnectHandler(NetConnection connection, object data)
        {
            GCli.ClearPacketHandler();

            GCli.SetPacketHandler(MessageType.ConnectSuccess, DataType.Bytes, ConnectSuccessHandler);            
        }

        public void ConnectSuccessHandler(NetConnection connection, object data)
        {
            GCli.ClearPacketHandler();

            InitData init = GCli.Deserialize<InitData>((byte[])data);
            GameObject.Find("Canvas").GetComponent<UIScript>().SetInitData(init.bmps);
            Players.SetPlayer(init.sync);

            GCli.SetPacketHandler(MessageType.Snapshot, DataType.Bytes, SnapshotHandler);
        }

        public void SnapshotHandler(NetConnection connection, object data)
        {
            SnapShotData snap = GCli.Deserialize<SnapShotData>((byte[])data);

            Players.UpdatePlayerSyncData(snap.syncs);
            ui.AddBlocks(snap.pixels);
        }

        /*public void DebugHandler(NetConnection connection, object data)
        {
            string message = (string)data;
            debugText.text = message;
        }*/
    }
}

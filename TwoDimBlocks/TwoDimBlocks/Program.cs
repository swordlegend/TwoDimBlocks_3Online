using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YasGameLib;

namespace TwoDimBlocks
{
    class Program
    {
        static Task task;
        static bool exit = false;
        const float frameSpan = 1.0f / 30.0f;
        static int sendCount = 0;

        static int port;
        static string sqlpath;

        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory() + "/Setting/setting.txt";
            if (!ReadFile(path))
            {
                return;
            }

            task = Task.Run(() =>
            {
                Server();
            });

            while (true)
            {
                Console.WriteLine(@"type exit for close server");
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }
            }
            exit = true;

            task.Wait();
        }

        static void Server()
        {

            //GSQLite.Open(sqlpath);            

            GSrv.Init();
            GSrv.SetConnectPacketHandler(ConnectHandler);
            GSrv.SetDisconnectPacketHandler(DisconnectHandler);
            GSrv.SetDebugPacketHandler(DebugHandler);
            GSrv.SetPacketHandler(MessageType.Push, DataType.Bytes, PushHandler);
            GSrv.SetPacketHandler(MessageType.Draw, DataType.Bytes, DrawHandler);

            GSrv.Listen("TwoDimBlocks0.1", port);

            while (!exit)
            {
                DateTime startTime = DateTime.Now;

                GSrv.Receive();

                sendCount++;

                if (sendCount == 3)
                {
                    sendCount = 0;

                    SendSnapshot();
                }

                Update(frameSpan);

                TimeSpan span = DateTime.Now - startTime;
                if (span.TotalMilliseconds < frameSpan * 1000)
                {
                    Thread.Sleep((int)(frameSpan * 1000) - (int)span.TotalMilliseconds);
                }
            }

            GSrv.Shutdown();
            /*Players.SaveAllPlayer();
            GSQLite.Close();*/
        }

        static void SendSnapshot()
        {
            SnapShotData data = new SnapShotData();
            Players.AddSyncs(data.syncs);
            data.pixels.AddRange(World.GetBuffer());

            GSrv.SendToAll(MessageType.Snapshot, GSrv.Serialize<SnapShotData>(data), NetDeliveryMethod.ReliableUnordered);
        }

        static void Update(float delta)
        {
            //Lobby.Update(delta);
        }

        static bool ReadFile(string path)
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

                        if (line.StartsWith("Port="))
                        {
                            string sub = line.Substring("Port=".Length);
                            if (!int.TryParse(sub, out port))
                            {
                                throw new Exception();
                            }
                        }
                        else if (line.StartsWith("SQLite="))
                        {
                            string sub = line.Substring("SQLite=".Length);
                            sqlpath = Directory.GetCurrentDirectory() + "/SQLite/" + sub;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        static public void ConnectHandler(NetConnection connection, object data)
        {
            PlayerSyncData sync = Players.GeneratePlayer(connection);
            BMPInitData bmps = World.GetBMP();

            InitData init = new InitData() { sync = sync, bmps = bmps };
            GSrv.Send(MessageType.ConnectSuccess, GSrv.Serialize<InitData>(init), connection, NetDeliveryMethod.ReliableOrdered);
        }

        static public void DisconnectHandler(NetConnection connection, object data)
        {
            Players.DeletePlayer(connection);
        }

        static public void DebugHandler(NetConnection connection, object data)
        {
            Console.WriteLine((string)data);
        }

        static public void PushHandler(NetConnection connection, object data)
        {
            PushData push = GSrv.Deserialize<PushData>((byte[])data);

            Players.Push(connection, push);
        }

        static public void DrawHandler(NetConnection connection, object data)
        {
            DrawData draw = GSrv.Deserialize<DrawData>((byte[])data);

            World.Buffer(draw);
        }
    }
}

using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoDimBlocks
{
    public static class Players
    {
        static int userCount = 0;
        static Dictionary<NetConnection, PlayerSyncData> players = new Dictionary<NetConnection, PlayerSyncData>();

        public static PlayerSyncData GeneratePlayer(NetConnection connection)
        {
            int userid = userCount++;
            PlayerSyncData sync = new PlayerSyncData { userid = userid, xpos = 10.0f, ypos = 0.0f, zpos = 10.0f, yrot = 0, anime = 0 };
            players.Add(connection, sync);

            return sync;
        }

        public static void DeletePlayer(NetConnection connection)
        {
            players.Remove(connection);
        }

        public static void AddSyncs(List<PlayerSyncData> data)
        {
            data.AddRange(players.Values);
        }

        public static void Push(NetConnection connection, PushData push)
        {
            if(players.ContainsKey(connection))
            {
                PlayerSyncData sync = players[connection];
                sync.xpos = push.xpos;
                sync.ypos = push.ypos;
                sync.zpos = push.zpos;
                sync.yrot = push.yrot;
                sync.anime = push.anime;
            }
        }
    }
}

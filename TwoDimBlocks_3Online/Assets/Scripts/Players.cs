using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoDimBlocks
{
    public static class Players
    {
        static int userid;
        static Player player;
        static Dictionary<int, OtherPlayer> others = new Dictionary<int, OtherPlayer>();

        public static void SetPlayer(PlayerSyncData sync)
        {
            userid = sync.userid;
            player = new Player(sync);
        }

        public static void UpdatePlayerSyncData(List<PlayerSyncData> syncs)
        {
            foreach (OtherPlayer other in others.Values)
            {
                other.ResetUpdateFlag();
            }

            foreach (PlayerSyncData sync in syncs)
            {
                if (sync.userid == userid)
                {
                    player.ReceiveLatestData(sync);
                }
                else if (others.ContainsKey(sync.userid))
                {
                    others[sync.userid].ReceiveLatestData(sync);
                }
                else
                {
                    others.Add(sync.userid, new OtherPlayer(sync));
                }
            }

            var removes = others.Where(f => f.Value.updated == false).ToArray();
            foreach (var remove in removes)
            {
                remove.Value.Destroy();
                others.Remove(remove.Key);
            }
        }

        public static PushData GetPushData()
        {
            if(player == null)
            {
                return null;
            }
            else
            {
                return player.GetPushData();
            }
        }

        public static void FixedUpdate(float delta)
        {
            foreach(OtherPlayer other in others.Values)
            {
                other.Update(delta);
            }
        }
    }
}

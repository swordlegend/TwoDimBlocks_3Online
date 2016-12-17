using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoDimBlocks
{
    public class Player : PlayerBase
    {
        GameObject prefab;

        public Player(PlayerSyncData sync) : base(sync)
        {
            prefab = (GameObject)GameObject.Instantiate(Resources.Load("Player"));
            prefab.GetComponent<PlayerScript>().Init(sync);

            Camera.main.GetComponent<CameraScript>().SetTarget(prefab.transform);
        }

        public PushData GetPushData()
        {
            return prefab.GetComponent<PlayerScript>().GetPushData();
        }
    }
}

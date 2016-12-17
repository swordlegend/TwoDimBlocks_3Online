using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoDimBlocks
{
    public class OtherPlayer : PlayerBase
    {
        GameObject prefab;

        public OtherPlayer(PlayerSyncData sync) : base(sync)
        {
            prefab = (GameObject)GameObject.Instantiate(Resources.Load("OtherPlayer"));
            prefab.GetComponent<OtherPlayerScript>().Init(sync);
        }

        public void Update(float delta)
        {
            prefab.GetComponent<OtherPlayerScript>().Interpolation(latest, delta);
        }

        public void Destroy()
        {
            GameObject.Destroy(prefab);
        }
    }
}

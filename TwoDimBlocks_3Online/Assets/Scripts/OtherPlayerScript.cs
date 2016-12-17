using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoDimBlocks
{
    public class OtherPlayerScript : MonoBehaviour
    {
        Animator anime;

        int state;

        public void Init(PlayerSyncData sync)
        {
            anime = GetComponent<Animator>();

            transform.position = new Vector3(sync.xpos, sync.ypos, sync.zpos);
            transform.localEulerAngles = new Vector3(0, sync.yrot, 0);

            state = sync.anime;

            SetAnime();
        }

        void SetAnime()
        {
            if(state == 0)
            {
                anime.SetBool("Walk", false);
            }
            else
            {
                anime.SetBool("Walk", true);
            }
        }

        public void Interpolation(PlayerSyncData latest, float delta)
        {
            Vector3 latestPos = new Vector3(latest.xpos, latest.ypos, latest.zpos);
            transform.position = Vector3.Lerp(transform.position, latestPos, delta * 10.0f);

            transform.localEulerAngles = new Vector3(0, Mathf.LerpAngle(transform.localEulerAngles.y, latest.yrot, delta * 10.0f), 0);

            if (this.state != latest.anime)
            {
                this.state = latest.anime;
                SetAnime();
            }
        }
    }
}

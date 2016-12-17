using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoDimBlocks
{
    public class PlayerScript : MonoBehaviour
    {
        [SerializeField]
        int playerNum;

        Rigidbody rgd;
        Animator anime;

        bool walk = false;

        public void Init(PlayerSyncData sync)
        {
            rgd = GetComponent<Rigidbody>();
            anime = GetComponent<Animator>();

            transform.position = new Vector3(sync.xpos, sync.ypos, sync.zpos);
            transform.localEulerAngles = new Vector3(0, sync.yrot, 0);
        }

        public PushData GetPushData()
        {
            PushData data = new PushData();
            data.xpos = transform.position.x;
            data.ypos = transform.position.y;
            data.zpos = transform.position.z;
            data.yrot = transform.localEulerAngles.y;
            data.anime = walk ? 1 : 0;

            return data;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            float v = 0, h = 0;
            bool jump = false;

            if (playerNum == 0)
            {
                v += Input.GetKey(KeyCode.S) ? -1.0f : 0;
                v += Input.GetKey(KeyCode.W) ? 1.0f : 0;

                h += Input.GetKey(KeyCode.A) ? -1.0f : 0;
                h += Input.GetKey(KeyCode.D) ? 1.0f : 0;

                if (IsGrounded() && Input.GetKeyDown(KeyCode.LeftShift))
                {
                    jump = true;
                }
            }
            else
            {
                v += Input.GetKey(KeyCode.DownArrow) ? -1.0f : 0;
                v += Input.GetKey(KeyCode.UpArrow) ? 1.0f : 0;

                h += Input.GetKey(KeyCode.LeftArrow) ? -1.0f : 0;
                h += Input.GetKey(KeyCode.RightArrow) ? 1.0f : 0;

                if (IsGrounded() && Input.GetKeyDown(KeyCode.RightShift))
                {
                    jump = true;
                }
            }

            Move(v);
            Turn(h);

            if (jump)
            {
                //rgd.AddForce(new Vector3(0, 100.0f, 0));
                //rgd.velocity = new Vector3(rgd.velocity.x, 5.5f, rgd.velocity.z);
                rgd.velocity = new Vector3(rgd.velocity.x, 10.0f, rgd.velocity.z);
                //GetComponent<AudioSource>().Play();
            }

            if (v == 0 && h == 0)
            {
                walk = false;
                anime.SetBool("Walk", false);
            }
            else
            {
                walk = true;
                anime.SetBool("Walk", true);
            }
        }

        void Move(float v)
        {
            Vector3 movement = transform.forward * v * 3.0f * Time.deltaTime;
            rgd.MovePosition(transform.position + movement);
        }


        void Turn(float h)
        {
            float turn = h * 120.0f * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            transform.localRotation *= turnRotation;
        }

        bool IsGrounded()
        {
            return Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), -Vector3.up, 0.15f);
        }
    }
}

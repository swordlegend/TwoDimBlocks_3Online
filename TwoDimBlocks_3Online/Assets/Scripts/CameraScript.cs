using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoDimBlocks
{
    public class CameraScript : MonoBehaviour
    {
        [SerializeField]
        Transform target;
        bool active;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(active)
            {
                transform.position = target.position + target.forward * -2.0f + Vector3.up * 0.5f;
                transform.LookAt(target.position + new Vector3(0, 0.5f, 0));
            }
        }

        public void SetFollowState(bool active)
        {
            this.active = active;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}

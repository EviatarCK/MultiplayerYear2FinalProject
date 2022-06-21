using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MultiPlayerProject
{
    public class Destroy : MonoBehaviour
    {
        public GameObject glass;
        private void Update()
        {
            if (Input.GetKey(KeyCode.X))
            {
                Destroy(glass);
            }
        }
    }
}

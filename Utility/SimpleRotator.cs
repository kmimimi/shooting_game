using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KShooting
{
    public class SimpleRotator : MonoBehaviour
    {
        public float xSpeed;
        public float ySpeed;
        public float zSpeed;


        private Vector3 rot;


        private void Update()
        {
            this.rot += new Vector3(xSpeed, ySpeed, zSpeed) * TimeManager.deltaTime;

            this.transform.localRotation = Quaternion.Euler(this.rot);
        }
    }
}
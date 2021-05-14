using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    public class ParticleDestroyer : MonoBehaviour
    {
        private ParticleSystem ps;



        private void Awake()
        {
            this.ps = GetComponentInChildren<ParticleSystem>();
        }

        public void Update()
        {
            if (!this.ps.IsAlive(true))
            {
                ObjectPoolManager.inst.Return(this.gameObject);
            }
        }
    }
}
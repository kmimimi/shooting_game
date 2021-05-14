using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    public class SoundObject : MonoBehaviour
    {
        public AudioSource audioSource { get; private set; }



        private void Awake()
        {
            this.audioSource = this.gameObject.AddComponent<AudioSource>();
        }

        private void OnDisable()
        {
            this.audioSource.clip = null;
            this.audioSource.Stop();
        }

        private void Update()
        {
            if(!this.audioSource.isPlaying)
                SoundManager.inst.ReturnObject(this);
        }
    }
}
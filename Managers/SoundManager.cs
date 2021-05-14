using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KShooting
{
    /// <summary>
    /// 사운드 매니저. 동작만 되도록 최소한으로 구현해둠.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        /// <summary>
        /// 사운드 오브젝트 생성 수
        /// </summary>
        private const int MAX_SOUND_OBJECT_COUNT = 50;
        public static SoundManager inst { get; private set; }

        private AudioSource bgmSource;


        private AudioClip[] clips;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Construct()
        {
            inst = new GameObject("Sound Manager").AddComponent<SoundManager>();
            DontDestroyOnLoad(inst);
        }

        private void Awake()
        {
            this.bgmSource = this.gameObject.AddComponent<AudioSource>();
            this.bgmSource.loop = true;
            this.bgmSource.volume = 0.3f;
            this.clips = Resources.LoadAll<AudioClip>("Sounds/");

            // 사운드 오브젝트 생성

            for(int i=0; i<MAX_SOUND_OBJECT_COUNT - 1; i++)
            {
                SoundObject obj = new GameObject("Sound Object").AddComponent<SoundObject>();
                obj.transform.SetParent(this.transform);
                obj.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// BGM을 재생한다.
        /// </summary>
        /// <param name="key"></param>
        public void PlayBGM(string key)
        {
            this.bgmSource.clip = GetClip(key);
            this.bgmSource.Play();
        }

        /// <summary>
        /// BGM을 중단한다.
        /// </summary>
        public void StopBGM()
        {
            this.bgmSource.Stop();
        }

        public void PlaySound(string key, float volume = 1)
        {
            SoundObject so = PullSoundObject();

            if(so != null)
            {
                so.gameObject.SetActive(true);
                so.audioSource.clip = GetClip(key);
                so.audioSource.volume = volume;
                so.audioSource.spatialBlend = 1;
                so.audioSource.minDistance = 10;
                so.audioSource.Play();
            }
        }

        public void PlaySound(string key, Vector3 worldPos, float volume = 1)
        {
            SoundObject so = PullSoundObject();

            if (so != null)
            {
                so.gameObject.SetActive(true);
                so.audioSource.clip = GetClip(key);
                so.transform.position = worldPos;
                so.audioSource.volume = volume;
                so.audioSource.spatialBlend = 1;
                so.audioSource.minDistance = 10;
                so.audioSource.Play();
            }
        }

        public void PlaySound(string key, Transform parent, Vector3 offset, float volume = 1)
        {
            SoundObject so = PullSoundObject();

            if (so != null)
            {
                so.gameObject.SetActive(true);
                so.audioSource.clip = GetClip(key);
                so.transform.SetParent(parent);
                so.transform.localPosition = offset;
                so.audioSource.volume = volume;
                so.audioSource.spatialBlend = 1;
                so.audioSource.minDistance = 10;
                so.audioSource.Play();
            }
        }

        public SoundObject PullSoundObject()
        {
            if(this.transform.childCount > 0)
            {
                SoundObject so = this.transform.GetChild(0).GetComponent<SoundObject>();

                so.transform.SetParent(null);
                return so;
            }
            else
            {
                return null;
            }
        }

        public AudioClip GetClip(string key)
        {
            for(int i=0; i<this.clips.Length; i++)
            {
                if (this.clips[i].name == key)
                    return this.clips[i];
            }

            throw new KeyNotFoundException("키가 없음. " + key);
        }

        public void ReturnObject(SoundObject so)
        {
            so.transform.SetParent(this.transform);
            so.gameObject.SetActive(false);
        }
    }
}
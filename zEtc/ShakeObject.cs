using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KShooting
{
    /// <summary>
    /// 간단하게 오브젝트를 흔들어주는 컴포넌트
    /// </summary>
    public class ShakeObject : MonoBehaviour
    {
        private float remainTime = 0f;
        private float amount = 0.7f;


        public void Play(float time=1f, float amount = 0.1f)
        {
            this.remainTime = time;
            this.amount = amount;
        }

        private void Update()
        {
            if (this.remainTime > 0)
            {
                this.transform.localPosition = Random.insideUnitSphere * this.amount;

                this.remainTime -= TimeManager.deltaTime;
            }
            else
            {
                this.remainTime = 0f;
                this.transform.localPosition = Vector3.zero;
            }
        }
    }
}
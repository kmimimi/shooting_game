using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KShooting
{
    /// <summary>
    /// 어그로 아이콘.
    /// </summary>
    [RequireComponent(typeof(Billboard))]
    public class AggroIcon : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer = null;
        private float lifeTime;


        public void Init()
        {
            this.lifeTime = 1;
            this.spriteRenderer.color = Color.white;
        }

        private void Update()
        {
            this.lifeTime -= Time.deltaTime;
            this.spriteRenderer.color = new Color(1, 1, 1, Mathf.Clamp01(this.lifeTime * 5));

            // 1초 지나면 자동 삭제
            if (this.lifeTime <= 0)
                ObjectPoolManager.inst.Return(this.gameObject);
        }
    }
}

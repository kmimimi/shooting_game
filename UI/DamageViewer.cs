using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace KShooting.UI
{
    /// <summary>
    /// 입힌 대미지를 표시해준다.
    /// </summary>
    public class DamageViewer : MonoBehaviour
    {
        /// <summary>
        /// 지속시간. 0이되면 자동으로 소멸한다.
        /// </summary>
        private const float LIFE_TIME = 0.5f;

        [SerializeField] private TextMeshPro text;
        /// <summary>
        /// 값이 +일때의 컬러
        /// </summary>
        [SerializeField] private Color plusColor = Color.green;

        /// <summary>
        /// 값이 -일대의 컬러
        /// </summary>
        [SerializeField] private Color minusColor = Color.red;

        private float lifeTime;



        private void Reset()
        {
            this.text = GetComponentInChildren<TextMeshPro>();
        }

        private void Awake()
        {
            if (this.text == null)
                KLog.LogError("Text is null");
        }

        private void Update()
        {
            this.lifeTime -= TimeManager.deltaTime;

            if(this.lifeTime <= 0)
            {
                ObjectPoolManager.inst.Return(this.gameObject);
                return;
            }

            this.transform.position += Vector3.up * 2 * TimeManager.deltaTime;
        }

        public void Init(float value)
        {
            this.text.color = value >= 0 ? this.plusColor : this.minusColor;
            this.text.text  = Mathf.RoundToInt(value).ToString();

            this.lifeTime = LIFE_TIME;
        }
    }
}
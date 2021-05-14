using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace KShooting.UI
{
    /// <summary>
    /// 스킬 상태(사용여부, 쿨타임)를 표시해주는 UI Element
    /// </summary>
    public class SkillViewer_IconElement : MonoBehaviour
    {
        [SerializeField] private Image _iconImage     = null;
        [SerializeField] private Image _progressImage = null;
        [SerializeField] private TMP_Text _keyText    = null;
             

        [Tooltip("사용가능할때 나올 이펙트 이미지")]
        [SerializeField] private Image _effect = null;
        [SerializeField] private Image _xImage = null;


        private float progressValue;
        private Coroutine effectCo;



        public void Init(Sprite icon, string key)
        {
            this._iconImage.overrideSprite     = icon;
            this._progressImage.overrideSprite = icon;
            
            this._keyText.text   = key;
            this.progressValue   = 1;
            this.effectCo        = null;
            this._effect.enabled = false;
            this._xImage.enabled = false;
        }

        private void Awake()
        {
            this._effect.enabled = false;
        }

        /// <summary>
        /// value값이 1이면 남은 쿨타임이 없다.
        /// </summary>
        public void SetCooltime(float value)
        {
            // 현재 표시되고 있는 값과 같은 값이 들어오면 무시
            if (value == this.progressValue)
                return;


            // UI에 반영
            this._progressImage.fillAmount = value;
            

            // 쿨타임이 끝났을 경우, 간단한 이펙트 표시
            if(value == 1)
            {
                // 혹시 이펙트가 돌아가는 중이면 이펙트 지움
                if (this.effectCo != null)
                    StopCoroutine(this.effectCo);

                this.effectCo = StartCoroutine(EffectUpdate());
            }

            // 변경된값 저장
            this.progressValue = value;
        }

        /// <summary>
        /// 스킬을 사용할 수 있는지 표시해준다.(use가 false면 사용 불가능이라고 표시)
        /// </summary>
        public void SetUseable(bool use)
        {
            this._xImage.enabled = !use;
        }

        /// <summary>
        /// 쿨타임이 끝났을때 나올 이펙트
        /// </summary>
        private IEnumerator EffectUpdate()
        {
            this._effect.enabled = true;
            yield return new WaitForSeconds(0.1f);
            this._effect.enabled = false;

            this.effectCo = null;
        }
    }
}
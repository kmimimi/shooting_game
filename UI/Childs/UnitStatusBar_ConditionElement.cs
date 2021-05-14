using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KShooting.UI
{
    /// <summary>
    /// 컨디션 아이콘
    /// </summary>
    public class UnitStatusBar_ConditionElement : MonoBehaviour
    {
        [SerializeField] private Image iconImage = null;
        [SerializeField] private Image bgImage  = null;

        public Condition condition { get; private set; }



        public void Init(Condition condition)
        {
            // 보여줄 컨디션 등록
            this.condition = condition;

            Sprite sprite = SpriteManager.Get(this.condition.iconPath);

            this.iconImage.sprite = sprite;
            this.bgImage.sprite   = sprite;
        }

        public void UpdateUI()
        {
            // 남은시간 표시
            if (this.condition.duration <= 0)
                return;

            this.iconImage.fillAmount = Mathf.Clamp01(this.condition.remainTime / this.condition.duration);
        }

        private void OnDisable()
        {
            this.condition = null;
        }
    }
}
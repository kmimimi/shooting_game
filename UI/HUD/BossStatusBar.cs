using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace KShooting.UI
{
    /// <summary>
    /// 보스전 상태표시 UI
    /// </summary>
    public class BossStatusBar : MonoBehaviour
    {
        public static BossStatusBar current { get; private set; }

        [SerializeField] private TMP_Text      bossNameText = null;
        [SerializeField] private Slider        hpSlider     = null;
        [SerializeField] private TMP_Text      hpText       = null;
        [SerializeField] private TMP_Text      hpPercent    = null;
        [SerializeField] private RectTransform conditionLayout = null;

        private Unit observingUnit;
        private List<UnitStatusBar_ConditionElement> conditionElements = new List<UnitStatusBar_ConditionElement>();


        private float lastCurrHP;
        private float lastMaxHP;



        public void Init(Unit bossUnit)
        {
            // 관찰 유닛 등록
            this.observingUnit     = bossUnit;
            // 보스 이름 표시
            this.bossNameText.text = bossUnit.unitName;
            // 보스 컨디션 표시
            this.observingUnit.conditionModule.onAddCondition    += OnAddCondition;
            this.observingUnit.conditionModule.onRemoveCondition += OnRemoveCondition;

            UpdateHP();
            SoundManager.inst.PlayBGM(SoundKeys.BGM_BATTLE_BOSS); // 보스전은 다른 사운드 재생
        }

        private void Awake()
        {
            current = this;
            this.gameObject.SetActive(false);
        }

        private void Update()
        {
            // 유닛 탐색을 종료한다.
            if (observingUnit.unitState != UnitState.Live)
            {
                this.gameObject.SetActive(false);
                return;
            }

            this.hpSlider.value = Mathf.Clamp01(this.observingUnit.currHP / this.observingUnit.maxHP);

            // 체력을 표시할 UI가 있고 && 체력 변경점이 있을 때
            if ((this.lastCurrHP != this.observingUnit.currHP) || (this.lastMaxHP != this.observingUnit.maxHP))
            {
                UpdateHP();
            }
        }

        private void OnDisable()
        {
            // 필요없는 이벤트 제거
            if(this.observingUnit != null)
            { 
                this.observingUnit.conditionModule.onAddCondition    -= OnAddCondition;
                this.observingUnit.conditionModule.onRemoveCondition -= OnRemoveCondition;

                this.observingUnit = null;
            }
        }
        
        /// <summary>
        /// 체력 데이터를 UI에 업데이트 한다.
        /// </summary>
        private void UpdateHP()
        {
            this.lastCurrHP = this.observingUnit.currHP;
            this.lastMaxHP = this.observingUnit.maxHP;
            this.hpText.text  = string.Format(UnitStatusBar.HP_AND_MP_FORMAT, this.observingUnit.currHP, this.observingUnit.maxHP);
            this.hpPercent.text = Mathf.CeilToInt((this.lastCurrHP / this.lastMaxHP) * 100).ToString() + "%";
        }

        /// <summary>
        /// Called By Unit.ConditionModule
        /// 유닛에 컨디션이 추가될 때 호출된다.
        /// </summary>
        private void OnAddCondition(Condition condition)
        {
            // 특정 상태이상은 UI에 표시 안함
            if (condition.conditionType == UnitCondition.Knockback)
                return;

            // 아이콘 생성
            var element = ObjectPoolManager.inst.Get<UnitStatusBar_ConditionElement>(PrefabPath.UI.UnitStatusBar_ConditionElement);

            element.transform.SetParent(this.conditionLayout);

            element.transform.localRotation = Quaternion.identity;
            element.transform.localScale    = Vector3.one;
            element.Init(condition);

            this.conditionElements.Add(element);
        }

        /// <summary>
        /// Called By Unit.ConditionModule
        /// 유닛에 컨디션이 제거될 때 호출된다.
        /// </summary>
        private void OnRemoveCondition(Condition condition)
        {
            // 특정 상태이상은 UI에 표시 안함
            if (condition.conditionType == UnitCondition.Knockback)
                return;

            for (int i=0; i<this.conditionElements.Count; i++)
            {
                if(this.conditionElements[i].condition.conditionType == condition.conditionType)
                {
                    ObjectPoolManager.inst.Return(this.conditionElements[i].gameObject);
                    this.conditionElements.RemoveAt(i);

                    return;
                }
            }
        }
    }
}

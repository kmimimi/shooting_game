using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace KShooting.UI
{
    /// <summary>
    /// 유닛의 상태를 표시해준다.
    /// Disable시 관찰중인 유닛을 잃게 된다.
    /// </summary>
    public class UnitStatusBar : MonoBehaviour
    {
        public const string HP_AND_MP_FORMAT = "{0} / {1}";

        [Header("사용할 필드값만 캐싱")]
        [SerializeField] private Slider        hpSlider        = null;
        [SerializeField] private TMP_Text      hpText          = null;
        [SerializeField] private Slider        mpSlider        = null;
        [SerializeField] private TMP_Text      mpText          = null;
        [SerializeField] private RectTransform conditionLayout = null;


        private Unit observingUnit;
        private List<UnitStatusBar_ConditionElement> conditionElements = new List<UnitStatusBar_ConditionElement>();

        /// <summary>
        /// 마지막으로 UI에 반영한 체력
        /// </summary>
        private float lastCurrHP;
        private float lastMaxHP;
        /// <summary>
        /// 마지막으로 UI에 반영한 마력
        /// </summary>
        private float lastCurrMP;
        private float lastMaxMP;
        


        public void Init(Unit observingUnit)
        {
            this.observingUnit = observingUnit;

            // 컨디션을 표시할 UI가 있을땐, 컨디션 변경상태를 체크한다
            if(this.conditionLayout != null)
            {
                this.observingUnit.conditionModule.onAddCondition    += OnAddCondition;
                this.observingUnit.conditionModule.onRemoveCondition += OnRemoveCondition;
            }

            // UI에 체력 표시
            if(this.hpText != null)
                UpdateHP();

            // UI에 마력표시
            if (this.mpText != null)
                UpdateMP();
        }

        private void OnDisable()
        {
            // 필요없는 이벤트 제거
            if (this.observingUnit != null && this.conditionLayout != null)
            {
                this.observingUnit.conditionModule.onAddCondition    -= OnAddCondition;
                this.observingUnit.conditionModule.onRemoveCondition -= OnRemoveCondition;

                this.observingUnit = null;
            }
        }

        private void Update()
        {
            // 관찰중인 유닛이 없으면 작동X
            if (this.observingUnit == null)
                return;

            // 체력표시 관련 처리
            if (this.hpSlider != null)
                this.hpSlider.value = Mathf.Clamp01(this.observingUnit.currHP / this.observingUnit.maxHP);

            // 체력을 표시할 UI가 있고 && 체력 변경점이 있을 때
            if (this.hpText != null &&
                ((this.lastCurrHP != this.observingUnit.currHP) || (this.lastMaxHP != this.observingUnit.maxHP)))
            {
                UpdateHP();
            }

            // 마력표시 관련 처리
            if (this.mpSlider != null)
                this.mpSlider.value = Mathf.Clamp01(this.observingUnit.currMP / this.observingUnit.maxMP);

            // 마력을 표시할 UI가 있고 && 마력 변경점이 있을 때
            if (this.mpText != null &&
                ((this.lastCurrMP != this.observingUnit.currMP) || (this.lastMaxMP != this.observingUnit.maxMP)))
            {
                UpdateMP();
            }

            // 컨디션 관련 처리
            for (int i = 0 ; i < this.conditionElements.Count ; i++)
                this.conditionElements[i].UpdateUI();
        }

        private void UpdateHP()
        {
            this.lastCurrHP = this.observingUnit.currHP;
            this.lastMaxHP = this.observingUnit.maxHP;

            this.hpText.text = string.Format(HP_AND_MP_FORMAT, this.observingUnit.currHP, this.observingUnit.maxHP);
        }

        private void UpdateMP()
        {
            this.lastCurrMP = this.observingUnit.currMP;
            this.lastMaxMP = this.observingUnit.maxMP;

            this.mpText.text = string.Format(HP_AND_MP_FORMAT, this.observingUnit.currMP, this.observingUnit.maxMP);
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

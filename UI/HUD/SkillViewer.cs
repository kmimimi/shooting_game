using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting.UI
{
    /// <summary>
    /// 플레이어의 스킬 정보 표시
    /// </summary>
    public sealed class SkillViewer : MonoBehaviour
    {
        [SerializeField] private RectTransform _layout = null;


        private Unit unit;
        private SkillViewer_IconElement[] elements = new SkillViewer_IconElement[0];



        public void Init(Unit unit)
        {
            this.unit = unit;

            // 기존에 사용하고 있던 element제거
            for (int i = 0 ; i < this.elements.Length ; i++)
                ObjectPoolManager.inst.Return(this.elements[i].gameObject);


            // 새로운 element생성
            this.elements = new SkillViewer_IconElement[this.unit.skillModule.skills.Count];
            for(int i=1; i<this.unit.skillModule.skills.Count; i++)
            {
                this.elements[i] = ObjectPoolManager.inst.New<SkillViewer_IconElement>(PrefabPath.UI.SkillViewer_IconElement);

                this.elements[i].transform.SetParent(this._layout);
                (this.elements[i].transform as RectTransform).anchoredPosition3D = Vector3.zero;
                this.elements[i].transform.localRotation                         = Quaternion.identity;
                this.elements[i].transform.localScale                            = Vector3.one;

                this.elements[i].Init(SpriteManager.Get(this.unit.skillModule.skills[i].iconPath), i.ToString());
            }
        }

        private void Update()
        {
            for(int i = 1 ; i< this.unit.skillModule.skills.Count ; i++)
            {
                // 스킬 쿨타임이 존재하고 && 스킬사용 쿨타임이 남아있을때
                if (this.unit.skillModule.skills[i].cooltime > 0 && this.unit.skillModule.skills[i].remainCooltime > 0)
                {
                    this.elements[i].SetCooltime(Mathf.Clamp01(
                        (this.unit.skillModule.skills[i].cooltime - this.unit.skillModule.skills[i].remainCooltime)
                        / this.unit.skillModule.skills[i].cooltime));
                }
                else
                {
                    // 아니면 항상 사용가능한것으로 표시
                    this.elements[i].SetCooltime(1);
                }

                this.elements[i].SetUseable(this.unit.skillModule.skills[i].Usable(false));
            }
        }

        /// <summary>
        /// 스킬이 변경됬을 때 호출된다.(미구현)
        /// </summary>
        private void OnSkillChanged()
        {
            Init(this.unit);
        }
    }
}

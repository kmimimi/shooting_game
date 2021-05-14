using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 거미줄. 닿으면 이동속도가 느려진다.
    /// </summary>
    public class SpiderWeb : MonoBehaviour
    {
        private Unit owner;




        public void Init(Unit owner, Vector2 size)
        {
            this.owner = owner;
            this.transform.localScale = new Vector3(size.x, 1, size.y);
        }

        public void PlayAnimation(Vector3 targetPos)
        {
            StartCoroutine(SkillUtility.Parabola(this.transform, targetPos));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (this.owner == null)
                return;

            Unit hitUnit = other.GetComponentInParent<Unit>();
            if (hitUnit != null)
            {
                // 충돌대상이 상대편 진영인경우
                if (hitUnit.unitSide != this.owner.unitSide)
                {
                    hitUnit.conditionModule.AddCondition(new SlowCondition(this.owner));
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (this.owner == null)
                return;

            Unit hitUnit = other.GetComponentInParent<Unit>();
            if (hitUnit != null)
            {
                // 충돌대상이 상대편 진영인경우
                if (hitUnit.unitSide != this.owner.unitSide)
                {
                    hitUnit.conditionModule.RemoveCondition(UnitCondition.Slow);
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 아이템 기본 클래스
    /// </summary>
    public abstract class Item : MonoBehaviour
    {
        [SerializeField] private Collider triggeCollider = null;

        
        private void OnTriggerEnter(Collider other)
        {
            var unit = other.GetComponentInParent<Unit>();
            if (unit != null)
            {
                if (unit.unitSide == UnitSide.Friendly && IsEat())
                {
                    Eat();
                    ObjectPoolManager.inst.Return(this.gameObject);
                }
            }
        }

        /// <summary>
        /// 먹을 수 있는지
        /// </summary>
        public virtual bool IsEat()
        {
            return true;
        }

        /// <summary>
        /// 아이템을 먹는다
        /// </summary>
        public abstract void Eat();


        public void PlayAnimation(Vector3 targetPos)
        {
            StartCoroutine(AnimationUpdate(targetPos));
        }

        private IEnumerator AnimationUpdate(Vector3 targetPos)
        {
            this.triggeCollider.enabled = false;
            yield return StartCoroutine(SkillUtility.Parabola(this.transform, targetPos, 70));
            this.triggeCollider.enabled = true;
        }
    }
}
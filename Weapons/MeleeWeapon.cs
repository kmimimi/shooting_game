using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 근거리 무기
    /// </summary>
    public abstract class MeleeWeapon : Weapon
    {
        /// <summary>
        /// 무기 대미지
        /// </summary>
        public float damage { get; set; }
        /// <summary>
        /// 무언가에 맞았을때 보여줄 히트 이펙트
        /// </summary>
        protected abstract string hitEffectPath { get; }

        private Collider[] colliders;



        private void Awake()
        {
            this.colliders = GetComponentsInChildren<Collider>();
        }

        public override void Init(Unit owner)
        {
            base.Init(owner);
            InactiveWeapon();
        }

        private void OnTriggerEnter(Collider other)
        {
            Unit hitUnit = other.GetComponentInParent<Unit>();
            if (hitUnit != null)
            {
                // 충돌대상이 상대편 진영인경우
                if (hitUnit.unitSide != this.owner.unitSide)
                    Hit(hitUnit, other);
            }
            else
            {
                // 충돌 대상이 오브젝트인경우
                Hit(hitUnit, other);
            }
        }

        /// <summary>
        /// hitUnit이 null이면 유닛이 안맞음
        /// </summary>
        protected virtual void Hit(Unit hitUnit, Collider other)
        {
            if (!string.IsNullOrEmpty(this.hitEffectPath))
            {
                var ps = ObjectPoolManager.inst.Get<ParticleSystem>(this.hitEffectPath);

                ps.transform.position = this.transform.position;
                ps.Play();
            }
        }

        /// <summary>
        /// 웨폰 기능 활성화
        /// </summary>
        public virtual void ActiveWeapon()
        {
            foreach (var c in this.colliders)
                c.enabled = true;
        }

        /// <summary>
        /// 웨폰기능 비활성화
        /// </summary>
        public virtual void InactiveWeapon()
        {
            foreach (var c in this.colliders)
                c.enabled = false;
        }
    }
}
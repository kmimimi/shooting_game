using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 폭탄 불릿. 히트반경으로 일정 범위 폭발을 일으킨다.
    /// </summary>
    public class BombBullet : Projectile
    {
        private float damage => 30;

        /// <summary>
        /// 터졌을 때, 공격 범위
        /// </summary>
        public float range { get; set; }
        protected override string hitEffectPath => PrefabPath.Particle.FireExplosion;
        protected override string hitSoundKey => SoundKeys.EFFECT_EXPLOSION;
        protected override float hitEffectScale => this.range;
        protected override string fireSoundKey => SoundKeys.EFFECT_SKILL_FIRE1;
        protected override float fireSoundVolume => 0.3f;



        public override void Init(Unit owner, float speed, float lifeTime)
        {
            base.Init(owner, speed, lifeTime);
            this.range = 2;
        }

        protected override void Hit(Unit hitUnit, Collider other)
        {
            base.Hit(hitUnit, other);

            // 히트지점으로부터 range내의 모든 유닛들을 가져온다.
            var units = KUtils.GetOverlapUnitsNonAlloc(this.transform.position, this.range);
            KUtils.DebugDrawSphere(this.transform.position, this.range); // 디버깅할때 범위 확인용!

            for(int i=0; i<units.Count; i++)
            {
                //유닛끼리 서로 다른 진영일경우, 대미지를 준다
                if (units[i].unitSide != this.owner.unitSide)
                {
                    units[i].DealDamage(this.owner, Mathf.RoundToInt(Calculator.RandomDamage(damage)));
                }
            }

            Finish();
        }
    }
}
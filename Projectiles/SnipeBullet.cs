using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 저격용 불릿
    /// </summary>
    public class SnipeBullet : Projectile
    {
        /// <summary>
        /// 기본대미지
        /// </summary>
        private float damage => 150;

        protected override string fireEffectPath => PrefabPath.Particle.SnipeShotEffect;
        protected override string fireSoundKey   => SoundKeys.EFFECT_SKILL_FIRE1;
        protected override string hitEffectPath  => PrefabPath.Particle.FireHit;
        protected override string hitSoundKey => SoundKeys.EFFECT_STRONG_HIT_SOUND;
        protected override float fireSoundVolume => 0.6f;


        protected override void Hit(Unit hitUnit, Collider other)
        {
            base.Hit(hitUnit, other);

            if (hitUnit != null)
            {
                //유닛끼리 서로 다른 진영일경우, 대미지를 준다
                if (hitUnit.unitSide != this.owner.unitSide)
                {
                    hitUnit.DealDamage(this.owner, Mathf.RoundToInt(Calculator.RandomDamage(damage)));
                }
            }

            Finish();
        }
    }
}
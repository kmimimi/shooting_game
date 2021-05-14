using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KShooting
{
    /// <summary>
    /// 독 불릿. 맞으면 일정시간동안 독 대미지를 입는다.
    /// </summary>
    public class PoisonBullet : Projectile
    {
        protected override string hitEffectPath => PrefabPath.Particle.PoisonHitEffect;
        protected override string hitSoundKey   => SoundKeys.EFFECT_POISON;
        protected override string fireSoundKey => SoundKeys.EFFECT_SKILL_FIRE1;
        protected override float fireSoundVolume => 0.3f;
        private float damage => 1;

        protected override void Hit(Unit hitUnit, Collider other)
        {
            base.Hit(hitUnit, other);

            if (hitUnit != null)
            {
                //유닛끼리 서로 다른 진영일경우, 대미지를 준다
                if (hitUnit.unitSide != this.owner.unitSide)
                { 
                    // 대미지를 준다
                    hitUnit.DealDamage(this.owner, Mathf.RoundToInt(Calculator.RandomDamage(damage)));

                    // 독 컨디션 추가
                    hitUnit.conditionModule.AddCondition(new PoisonCondition(this.owner, 10));
                }
            }

            Finish();
        }
    }
}
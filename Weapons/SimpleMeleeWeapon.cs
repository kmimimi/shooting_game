using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    public class SimpleMeleeWeapon : MeleeWeapon
    {
        protected override string hitEffectPath => this.overrideHitEffectPath;

        public string overrideHitEffectPath { get; set; }

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
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 넉백 상태이상
    /// 유닛을 밀어버림. 밀리는 동안 조작불가능
    /// </summary>
    public class KnockbackCondition : Condition
    {
        public override UnitCondition conditionType => UnitCondition.Knockback;
        public override string iconPath => null;
        public override string effectPath => null;

        private Vector3 vel;


        public KnockbackCondition(Unit attackedUnit, float duration, Vector3 vel) 
            : base(attackedUnit, duration)
        {
            this.vel = vel;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            Vector3 finalVel = Vector3.Lerp(Vector3.zero, this.vel, dt * 2);
            this.unit.chCtrl.Move(finalVel);
            this.vel -= finalVel;
        }
    }
}
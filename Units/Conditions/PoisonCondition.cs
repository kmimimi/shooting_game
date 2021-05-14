using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 독 대미지 컨디션
    /// TICK마다 일정 대미지를 줌
    /// </summary>
    public class PoisonCondition : Condition
    {
        /// <summary>
        /// 한번의 틱당 줄 대미지
        /// </summary>
        private const float DAMAGE = 10;

        /// <summary>
        /// 한번의 틱(초)
        /// </summary>
        private const float TICK   = 1;



        public override UnitCondition conditionType => UnitCondition.Poison;
        public override string iconPath => SpritePath.ConditionIcon_Path + UnitCondition.Poison.ToString();
        public override string effectPath => PrefabPath.Particle.Condition.PoisonConditionEffect;
        public override Vector3 effectPosition => this.unit.pivots.center.position;

        private float lastTickDamage;


        public PoisonCondition(Unit attackedUnit, float duration) : base(attackedUnit, duration)
        {
            this.lastTickDamage = duration;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if(this.lastTickDamage - TICK >= this.remainTime)
            {
                this.lastTickDamage -= TICK;
                this.unit.DealDamage(unit, DAMAGE);
            }
        }
    }
}
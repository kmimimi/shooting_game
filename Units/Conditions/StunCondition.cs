using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 스턴 상태이상.
    /// 유닛이 일정시간동안 행동불가
    /// </summary>
    public class StunCondition : Condition
    {
        public override UnitCondition conditionType => UnitCondition.Stun;
        public override string iconPath   => SpritePath.ConditionIcon_Path + UnitCondition.Stun.ToString();
        public override string effectPath => PrefabPath.Particle.Condition.StunConditionEffect;
        public override Vector3 effectPosition => this.unit.pivots.overhead.position;

        public StunCondition(Unit attackedUnit, float duration) : base(attackedUnit, duration)
        {
        }

        public override void Start()
        {
            base.Start();
            unit.animCtrl.TriggerStun();
        }

        public override void Finish()
        {
            base.Finish();
            unit.animCtrl.FinishStun();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    public class SlowCondition : Condition
    {
        public override UnitCondition conditionType => UnitCondition.Slow;

        public override string iconPath   => SpritePath.ConditionIcon_Path + UnitCondition.Slow.ToString();
        public override string effectPath => PrefabPath.Particle.Condition.SlowConditionEffect;

        public SlowCondition(Unit attackedUnit) : base(attackedUnit, -1) { }
    }
}
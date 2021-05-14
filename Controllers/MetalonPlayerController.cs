using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KShooting
{
    public class MetalonPlayerController : PlayerController
    {
        private Metalon metalon;

        public override void OnInit()
        {
            base.OnInit();
            this.metalon = this.unit as Metalon;
        }

        protected override void UseSkill(int index)
        {
            if (index == Metalon.SKILL_IDX_JUMP_ATTACK)
                this.metalon.JumpAttack();
            else if (index == Metalon.SKILL_IDX_SUMMON)
                this.metalon.SpellAttack(Metalon.SpellAttackKind.Summon);
            else
                base.UseSkill(index);
        }
    }
}
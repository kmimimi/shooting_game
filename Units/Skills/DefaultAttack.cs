using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 기본 공격.
    /// </summary>
    public class DefaultAttack : SkillBase
    {
        public override float skillUseTime => 0;


        public interface IDefaultAttackEvent
        {
            void OnDefaultAttack_ActionEvent();
        }

        public override string skillKey => SkillKeys.DefaultAttack;
        public override string iconPath => string.Empty;

        IDefaultAttackEvent attackEvent;


        public DefaultAttack(float cooltime) : base(cooltime)
        {
        }

        public override void Action()
        {
            base.Action();

            if(this.attackEvent == null)
                this.attackEvent = this.unit as IDefaultAttackEvent;


            this.attackEvent?.OnDefaultAttack_ActionEvent();
        }
    }
}
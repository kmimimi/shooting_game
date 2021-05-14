using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 유니티짱 전용 애니메이터 컨트롤러
    /// </summary>
    public class UnityChanAnimatorController : UnitAnimatorController
    {
        private UnityChan unityChan;


        protected override void Awake()
        {
            base.Awake();
            this.unityChan = this.unit as UnityChan;
        }

        protected virtual void Update()
        {
            // 해결방법으로 적절하진 않지만, 가끔 Die가 재생이 안된다...
            if (this.death)
            {
                if (!this.anim.GetBool(PARAM_IS_DEATH))
                    this.anim.SetTrigger(PARAM_DIE);
            }
        }

        public override void UpdateParameters()
        {
            base.UpdateParameters();

            // 공격 애니메이션 최~~대한 자연스럽게 하기 위해서...(IDLE이랑 Run이랑 허리 회전이 달라서 매우 어색함)
            if (this.unityChan.attacking && !this.unit.conditionModule.HasCondition(UnitCondition.Stun))
            {
                if(this.unityChan.isMoving)
                {
                    this.anim.SetLayerWeight(1, 0);
                    this.anim.SetLayerWeight(2, 1);
                }
                else
                {
                    this.anim.SetLayerWeight(1, 1);
                    this.anim.SetLayerWeight(2, 0);
                }
            }
            else
            {
                this.anim.SetLayerWeight(1, 0);
                this.anim.SetLayerWeight(2, 0);
            }
        }

        public override void TriggerDie()
        {
            base.TriggerDie();

            this.anim.SetLayerWeight(1, 0);
            this.anim.SetLayerWeight(2, 0);
        }
    }
}

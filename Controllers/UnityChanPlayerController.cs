using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 유니티짱용 플레이어 컨트롤러
    /// </summary>
    public class UnityChanPlayerController : PlayerController
    {
        /// <summary>
        /// 캐싱용
        /// </summary>
        private UnityChan unitychan;



        public override void OnInit()
        {
            base.OnInit();

            this.unitychan = this.unit as UnityChan;
        }

        protected override bool AttackUpdate()
        {
            bool ret = base.AttackUpdate();


            if (ret)
                this.unitychan.attacking = true;
            else
                this.unitychan.attacking = false;


            return ret;
        }

        protected override void UseSkill(int index)
        {
            // 스킬이 날라갈 방향 계산
            if (PlayerCamera.current.aimOn)
                this.unitychan.gun.fireDirection = this.unitychan.CalcAimFireDirection();
            else
                this.unitychan.gun.fireDirection = this.unitychan.transform.forward;

            base.UseSkill(index);
        }
    }
}

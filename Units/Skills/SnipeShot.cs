using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 저격 샷
    /// </summary>
    public class SnipeShot : SkillBase
    {
        public override float skillUseTime => 0.5f;
        public override string iconPath => SpritePath.SkillIcon_Path + SkillKeys.SnipeShot;
        public override string skillKey => SkillKeys.SnipeShot;
        

        private Gun gun;


        public SnipeShot(Gun gun, float cooltime) : base(cooltime)
        {
            this.gun = gun;
        }

        public override void Action()
        {
            base.Action();

            // 불릿 생성후 발사
            var bullet = ObjectPoolManager.inst.Get<SnipeBullet>(PrefabPath.Projectile.SnipeBullet);

            bullet.transform.position = this.gun.firePivot;
            bullet.transform.rotation = Quaternion.LookRotation(this.gun.fireDirection);


            // 불릿 초기화 및 발사
            bullet.Init(this.gun.owner, this.gun.power * 1.5f, 1);
            bullet.FireDirection();

            this.unit.conditionModule.AddCondition(new KnockbackCondition(null, 0.2f, -this.unit.transform.forward * 10));
        }

        public override bool Usable(bool checkCooltime)
        {
            if(this.isPlayerSkill)
            {
                if (!this.unitychan.playerCtrl.aimOn) // 저격 스킬은 조준상태일때만 사용가능
                    return false;
            }

            return base.Usable(checkCooltime);
        }
    }
}

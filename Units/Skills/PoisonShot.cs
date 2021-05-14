using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 독 샷
    /// </summary>
    public class PoisonShot : SkillBase
    {
        public override float skillUseTime => 0.2f;
        public override string iconPath => SpritePath.SkillIcon_Path + SkillKeys.PoisonShot;
        public override string skillKey => SkillKeys.PoisonShot;


        private Gun gun;


        public PoisonShot(Gun gun, float cooltime) : base(cooltime)
        {
            this.gun = gun;
        }

        public override void Action()
        {
            base.Action();

            // 불릿 생성후 발사
            var bullet = ObjectPoolManager.inst.Get<PoisonBullet>(PrefabPath.Projectile.PoisonBullet);

            bullet.transform.position = this.gun.firePivot;
            bullet.transform.rotation = Quaternion.LookRotation(this.gun.fireDirection);


            // 불릿 초기화 및 발사
            bullet.Init(this.gun.owner, this.gun.power, 1);
            bullet.FireDirection();
        }
    }
}

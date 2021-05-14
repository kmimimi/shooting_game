using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 폭탄 불릿
    /// </summary>
    public class BombShot : SkillBase
    {
        public override float skillUseTime => 0.2f;
        public override string iconPath => SpritePath.SkillIcon_Path + SkillKeys.BombShot;
        public override string skillKey => SkillKeys.BombShot;

        private Gun     gun;

        public BombShot(Gun gun, float cooltime) : base(cooltime)
        {
            this.gun = gun;
        }

        public override void Action()
        {
            base.Action();

            // 불릿 생성후 발사
            var bullet = ObjectPoolManager.inst.Get<BombBullet>(PrefabPath.Projectile.BombBullet);

            bullet.transform.position = this.gun.firePivot;
            bullet.transform.rotation = Quaternion.LookRotation(this.gun.fireDirection);


            // 불릿 초기화 및 발사
            bullet.Init(this.gun.owner, this.gun.power, 10);
            bullet.range = 3;
            bullet.FireDirection();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 전방위 샷
    /// </summary>
    public class Bullet360Shot : SkillBase
    {
        [System.Serializable]
        public struct Bullet360Info
        {
            /// <summary>
            /// 파워(이동속도)
            /// </summary>
            public float power;
            /// <summary>
            /// 불릿의 수명
            /// </summary>
            public float lifeTime;
            /// <summary>
            /// 발사횟수
            /// </summary>
            public int fireCount;
            /// <summary>
            /// 발사 한번당 나가는 불릿 수
            /// </summary>
            public int bulletCount;
            /// <summary>
            /// 발사횟수 주기
            /// </summary>
            public float cycle;


            /// <summary>
            /// 값들의 의미는 선언부분 참조
            /// </summary>
            public Bullet360Info(float power, float lifeTime, int fireCount, int bulletCount, float cycle)
            {
                this.power       = power;
                this.lifeTime    = lifeTime;
                this.fireCount   = fireCount;
                this.bulletCount = bulletCount;
                this.cycle       = cycle;
            }
        }

        public override float skillUseTime => 0;
        public override string skillKey => SkillKeys.Bullet360Shot;
        public override string iconPath => SpritePath.SkillIcon_Path + SkillKeys.Bullet360Shot;


        public Bullet360Info info;
        public Transform     firePivot;



        public Bullet360Shot(float cooltime, Transform firePivot, Bullet360Info info) : base(cooltime)
        {
            this.firePivot = firePivot;
            this.info = info;
        }


        public override void Action()
        {
            base.Action();
            
            this.unit.StartCoroutine(SkillUtility.FireProjectile_CircularSector(
                this.unit,
                this.firePivot.position,
                this.info.power,
                this.info.lifeTime,
                this.info.bulletCount,
                360,
                this.info.fireCount,
                this.info.cycle,
                true,
                SoundKeys.EFFECT_DEFAULT_ATTACK));
        }
    }
}
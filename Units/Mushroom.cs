using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KShooting
{
    public class Mushroom : Unit
    {
        private enum DefaultAttackType
        {
            /// <summary>
            /// 근접공격
            /// </summary>
            Melee,
            /// <summary>
            /// 곡사공격
            /// </summary>
            ParabolaBomb,
            /// <summary>
            /// 부채꼴 공격
            /// </summary>
            CircularSector
        }

        [Tooltip("발사체 피벗")]
        [SerializeField] private Transform _firePivot = null;

        private DefaultAttackType attackType;
        public override string dieSoundKey => SoundKeys.EFFECT_DIE_MONSETR;





        protected override void Awake()
        {
            base.Awake();

            // 유닛 컬러에 따라 공격 방식이 다르다.
            if (this.unitKey == UnitKeys.Mushroom_Red)
                this.attackType = DefaultAttackType.Melee;
            else if (this.unitKey == UnitKeys.Mushroom_Green)
                this.attackType = DefaultAttackType.ParabolaBomb;
            else if (this.unitKey == UnitKeys.Mushroom_Blue)
                this.attackType = DefaultAttackType.CircularSector;

            // 에러체크
            if (this._firePivot == null)
                KLog.LogError(string.Format("FirePivot is Null. Unit: {0}", this.unitKey), this);
        }

        public override void InitSkill(SkillModule module) { }
        public override void InitWeapon() { }

        public override void OnDefaultAttack_ActionEvent()
        {
            this.animCtrl.TriggerAttack();
        }

        public override void OnStartAttackEvent(int index)
        {
            base.OnStartAttackEvent(index);
            switch (this.attackType)
            {
                case DefaultAttackType.Melee:
                    SkillUtility.SimpleRangeDamage(this, this._firePivot.position, 1.5f, this.unitSide, this.defaultDamage, PrefabPath.Particle.FireHit, SoundKeys.EFFECT_HIT_SOUND);
                    SoundManager.inst.PlaySound(SoundKeys.EFFECT_PUNCH, this._firePivot.position);
                    break;
                case DefaultAttackType.ParabolaBomb:
                    {
                        //타겟이 없으면 안쏨
                        if ((this.controller as SimpleAIController).target == null)
                            return;


                        // 불릿 생성후 발사
                        var bullet = ObjectPoolManager.inst.Get<BombBullet>(PrefabPath.Projectile.BombBullet);

                        bullet.transform.position = this._firePivot.transform.position;

                        // 불릿 초기화 및 발사
                        bullet.Init(this, 1, 10);

                        bullet.range = 1;
                        bullet.FireParabola((this.controller as SimpleAIController).target.transform.position);
                    }
                    break;
                case DefaultAttackType.CircularSector:
                    SoundManager.inst.PlaySound(SoundKeys.EFFECT_DEFAULT_ATTACK, this._firePivot.position);
                    StartCoroutine(SkillUtility.FireProjectile_CircularSector(this, this._firePivot.position, 3, 3, 3, 90, 1, 0.5f));
                    break;
                default:
                    break;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 점프어택
    /// </summary>
    public class JumpAttack : SkillBase
    {
        public override float skillUseTime => 1;
        public override string skillKey => SkillKeys.JumpAttack;
        public override string iconPath => SpritePath.SkillIcon_Path + SkillKeys.JumpAttack;


        private float range;
        private float damage;



        public JumpAttack(float cooltime, float damage, float range) : base(cooltime)
        {
            this.range  = range;
            this.damage = damage;
        }

        public override void Action()
        {
            base.Action();

            // 범위 안에 있는 주변 유닛들을 모두 가져온다
            var units = KUtils.GetOverlapUnitsNonAlloc(this.unit.transform.position, this.range);

            for (int i = 0 ; i < units.Count ; i++)
            {
                //자기 자신이면 무시한다.
                if (units[i] == this.unit)
                    continue;

                //유닛끼리 서로 다른 진영일경우,
                if (units[i].unitSide != this.unit.unitSide)
                {
                    // 그리고 땅을 딛고 있을경우에만 대미지를 준다.
                    if (units[i].isGrounded)
                    {
                        units[i].DealDamage(this.unit, Mathf.RoundToInt(Calculator.RandomDamage(damage)));
                        units[i].conditionModule.AddCondition(new StunCondition(this.unit, 3));
                    }
                }
            }

            // 사운드 재생
            SoundManager.inst.PlaySound(SoundKeys.EFFECT_JUMP_ATTACK, this.unit.transform.position);

            // 카메라 흔들기
            PlayerCamera.current.ShakeCamera(0.5f, 0.2f);

            // 파티클 재생
            ParticleSystem ps = ObjectPoolManager.inst.Get<ParticleSystem>(PrefabPath.Particle.JumpAttackEffect);
            ps.transform.position = this.unit.transform.position;
            ps.transform.rotation = this.unit.transform.rotation;
            ps.transform.localScale = Vector3.one;
            ps.Play();
        }
    }
}
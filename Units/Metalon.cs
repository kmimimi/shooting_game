using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KShooting
{
    /// <summary>
    /// 보스. 패턴정보는 MetalonAIController가 가지고 있음
    /// </summary>
    public class Metalon : Unit, Summon.ISummon
    {
        public enum SpellAttackKind
        {
            Direct,
            Summon,
            SpiderWeb,
        }

        private static readonly int SPELL_ATTACK = Animator.StringToHash("SpellAttack");
        private static readonly int JUMP_ATTACK  = Animator.StringToHash("JumpAttack");

        public const int SKILL_IDX_ATTACK       = 0;
        public const int SKILL_IDX_BULLET_ATTACK = 1;
        public const int SKILL_IDX_JUMP_ATTACK  = 2;
        public const int SKILL_IDX_SUMMON       = 3;


        [Header("Metalon")]
        [SerializeField] private Transform         hone      = null;
        [SerializeField] private Transform         tail      = null;
        [SerializeField] private Transform         firePivot = null;
        public bool isMini = false;


        /// <summary>
        /// 애니메이션이 SpellAttack일때, 구분하기 위한 용도
        /// SpellAttack을 재생할경우, OnActiveSkill()의 index값이 1로만 들어오기 떄문에(애니메이션 이벤트에 그렇게 지정되어 있음).
        /// 별도로 구분짓기 위함.
        /// </summary>
        private SpellAttackKind spell;
        private Coroutine skillCo = null;

        public override bool movable => base.movable && this.skillCo == null;
        public override string dieSoundKey => SoundKeys.EFFECT_DIE_MONSETR;


        protected override void Awake()
        {
            base.Awake();

            // 에러체크
            if (this.hone == null)
                KLog.LogError("Hone is null. " + this.unitKey, this);
            if (this.tail == null)
                KLog.LogError("Tail is null. " + this.unitKey, this);
            if (this.firePivot == null)
                KLog.LogError("FirePivot is null. " + this.unitKey, this);
        }

        public override void Init()
        {
            base.Init();
            this.skillCo = null;

            // 작은 거미들은 회피 우선순위를 낮춘다
            if (!this.isMini)
                this.agent.avoidancePriority = 0;
        }

        /// <summary>
        /// 스킬 초기화
        /// </summary>
        public override void InitSkill(SkillModule module)
        {
            // 순서는 하드코딩이라 고치면 안됨. 수정시 SKILL_INDEX부분도 같이 고칠것.

            Bullet360Shot.Bullet360Info info = new Bullet360Shot.Bullet360Info(3, 3, this.isMini ? 3 : 6, 20, this.isMini ? 0.5f : 0.25f);
            module.AddSkill(new Bullet360Shot(1, this.firePivot, info));
            module.AddSkill(new JumpAttack(2f, 30, 20));
            module.AddSkill(new Summon(this, 3));
        }

        public override void InitWeapon()
        {
        }

        public override void Die()
        {
            base.Die();

            if (this.skillCo != null)
                StopCoroutine(this.skillCo);

            this.skillCo = null;
        }

        public override void OnDefaultAttack_ActionEvent()
        {
            this.animCtrl.TriggerAttack();
        }

        /// <summary>
        /// index == 0 : 기본공격
        /// index == 1 : SpellAttack(SpellAttack은 종류가 여러가지임)
        /// index == 2 : JumpAttack
        /// </summary>
        public override void OnStartAttackEvent(int index)
        {
            base.OnStartAttackEvent(index);

            switch (index)
            {
                case SKILL_IDX_ATTACK: // 기본 공격
                    {
                        SkillUtility.SimpleRangeDamage(this, this.hone.position, 1.5f, this.unitSide, this.defaultDamage, PrefabPath.Particle.FireHit, SoundKeys.EFFECT_HIT_SOUND);
                        SoundManager.inst.PlaySound(SoundKeys.EFFECT_PUNCH, this.hone.position);
                    }
                    break;
                case SKILL_IDX_BULLET_ATTACK:
                    {
                        if (this.spell == SpellAttackKind.Direct) // 360 샷
                        {
                            this.skillModule.skills[SKILL_IDX_BULLET_ATTACK].Action();
                        }
                        else if (this.spell == SpellAttackKind.Summon) // 소환
                        {
                            this.skillModule.skills[SKILL_IDX_SUMMON].Action();
                        }
                        else if (this.spell == SpellAttackKind.SpiderWeb) // 거미줄(일부로 하드코딩 했음)
                        {
                            for (int i = 0 ; i < 2 ; i++)
                            {
                                SpiderWeb web = ObjectPoolManager.inst.Get<SpiderWeb>(PrefabPath.SpiderWeb);
                                web.transform.position = this.transform.position;

                                web.Init(this, Vector2.one * (Random.Range(-1.0f, 1) + 3));
                                web.PlayAnimation(KUtils.SamplePosition_NavMesh(this.transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5))));
                                SoundManager.inst.PlaySound(SoundKeys.EFFECT_CREATE_SPIDER_WEB, this.transform.position);
                            }
                        }
                    }
                    break;
                case SKILL_IDX_JUMP_ATTACK: // 점프공격
                    {
                        this.skillModule.skills[SKILL_IDX_JUMP_ATTACK].Action();
                    }
                    break;
            }
        }

        /// <summary>
        /// 스펠 어택
        /// </summary>
        public void SpellAttack(SpellAttackKind kind)
        {
            if(kind == SpellAttackKind.Direct) // 360샷
            {
                this.spell = SpellAttackKind.Direct;
                this.animCtrl.SetTrigger(SPELL_ATTACK);
                this.skillModule.skills[SKILL_IDX_BULLET_ATTACK].SetCooltime();
            }
            else if(kind == SpellAttackKind.Summon) // 소환
            {
                this.spell = SpellAttackKind.Summon;
                this.animCtrl.SetTrigger(SPELL_ATTACK);
                this.skillModule.skills[SKILL_IDX_SUMMON].SetCooltime();
            }
            else if(kind == SpellAttackKind.SpiderWeb) // 거미줄
            {
                this.spell = SpellAttackKind.SpiderWeb;
                this.animCtrl.SetTrigger(SPELL_ATTACK);
                //this.skillModule.skills[SKILL_IDX_SPIDER].SetCooltime();
            }
        }

        /// <summary>
        /// 점프 공격
        /// </summary>
        public void JumpAttack()
        {
            this.animCtrl.SetTrigger(JUMP_ATTACK);
            this.skillModule.skills[SKILL_IDX_JUMP_ATTACK].SetCooltime();
        }

        /// <summary>
        /// 작은 거미를 소환한다.(Called By Skill)
        /// 직접 호출금지.
        /// </summary>
        public void Summon()
        {
            Metalon mini = ObjectPoolManager.inst.Get<Metalon>(PrefabPath.Unit.MetalonMini);
            mini.transform.position = this.tail.transform.position;
            mini.unitSide = this.unitSide;
            (mini.controller as MetalonAIController).targetPosition = KUtils.SamplePosition_NavMesh(mini.transform.position + new Vector3(Random.Range(-1, 1.0f) * 10, 0, Random.Range(-1, 1.0f) * 10));

            mini.Init();

            SoundManager.inst.PlaySound(SoundKeys.EFFECT_CREATE_METALON_MINI, this.transform.position);
        }
    }
}
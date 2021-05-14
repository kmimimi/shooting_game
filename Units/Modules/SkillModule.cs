using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace KShooting
{
    /// <summary>
    /// 스킬들을 관리하는 Module
    /// 0번째 인덱스는 일반공격으로 셋팅됨. 특별한 경우 아니면 바꾸지 말것.
    /// </summary>
    public class SkillModule
    {
        public List<SkillBase> skills { get; private set; } = new List<SkillBase>();
        public event UnityAction onSkillChanged;

        protected Unit unit { get; private set; }
        public DefaultAttack defaultAttack { get; private set; }

        /// <summary>
        /// 스킬사용이 진행중이다.
        /// </summary>
        public bool skillRunning
        {
            get
            {
                for(int i=0; i<this.skills.Count; i++)
                {
                    if (this.skills[i].skillUseRemainTime > 0)
                        return true;
                }

                return false;
            }
        }



        public SkillModule(Unit unit, DefaultAttack defaultAttack)
        {
            this.unit = unit;

            this.defaultAttack = defaultAttack;

            if(this.defaultAttack != null)  // 유닛의 생성자에서 초기화할땐 null로 들어옴.
                this.defaultAttack.SetUnit(this.unit);

            this.skills.Add(defaultAttack);
        }

        public void Update(float dt)
        {
            // 스킬 쿨타임 감소
            for (int i = 0 ; i < this.skills.Count ; i++)
            {
                // 스킬 쿨타임을 계산하지 말아야 하면 무시
                if (!this.skills[i].coolDowning)
                    continue;

                if (this.skills[i].skillUseRemainTime > 0)
                {
                    // 스킬이 지속되는동안엔 쿨타임 감소가 되지 않는다.
                    this.skills[i].skillUseRemainTime -= dt;
                }
                else
                {
                    // 스킬 사용시간이 끝나면 쿨타임을 감소시킨다
                    this.skills[i].remainCooltime -= dt;
                    if (this.skills[i].remainCooltime <= 0)
                        this.skills[i].Ready();
                }
            }
        }

        public void AddSkill(SkillBase skill)
        {
            // 사용할 유닛을 등록시켜줘야 함.
            skill.SetUnit(unit);

            this.skills.Add(skill);
            this.onSkillChanged?.Invoke();
        }
    }
}
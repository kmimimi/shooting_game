using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 스킬의 기본클래스. 
    /// 직접적으로 무언가를 하기보단 Unit클래스에 정의된 액션들을 처리할 수 있게 도와줌
    /// 
    /// 하위 클래스 구현시 여기 SkillBase내에서 처리를 할수도 있지만
    /// DefaultAttack처럼 인터페이스를 통해 구현하는 방법도 있음.
    /// </summary>
    public abstract class SkillBase
    {
        /// <summary>
        /// 스킬을 사용하는 유닛. 스킬모듈에 등록될 때 셋팅된다.
        /// </summary>
        protected Unit unit { get; private set; }

        #region 스킬고유정보
        /// <summary>
        /// 스킬 고유키
        /// </summary>
        public abstract string skillKey { get; }

        /// <summary>
        /// 스킬 아이콘
        /// </summary>
        public abstract string iconPath { get; }

        /// <summary>
        /// 스킬사용시간. Action()을 호출하면 this.skillUseRemainTime에 이 값이 들어간다.
        /// </summary>
        public abstract float skillUseTime { get; }
        #endregion 스킬고유정보

        #region Unit에 따라 변하는 정보
        /// <summary>
        /// 기본 쿨타임
        /// </summary>
        public float cooltime { get; private set; }
        #endregion Unit에 따라 변하는 정보

        #region Skill State
        /// <summary>
        /// 쿨타임 계산이 진행중이다.
        /// </summary>
        public bool coolDowning;
        /// <summary>
        /// 쿨타임 남은시간
        /// </summary>
        public float remainCooltime;

        /// <summary>
        /// 이 스킬을 사용하는데 필요한 시간.
        /// 이 값이 0보다 크면 스킬 사용중이다.
        /// </summary>
        public float skillUseRemainTime { get; set; }
        #endregion Skill State

        #region Player Only
        /// <summary>
        /// 플레이어가 사용하는 스킬일경우, 플레이어 유닛이 들어가 있다.
        /// </summary>
        protected UnityChan unitychan { get; private set; }

        /// <summary>
        /// 플레이어의 스킬인지
        /// </summary>
        public bool isPlayerSkill => this.unitychan != null;
        #endregion Player Only


        public SkillBase(float cooltime)
        {
            this.cooltime = cooltime;
        }

        /// <summary>
        /// Called by SkillModule
        /// </summary>
        /// <param name="unit"></param>
        public void SetUnit(Unit unit)
        {
            this.unit      = unit;
            this.unitychan = (unit is UnityChan) ? unit as UnityChan : null;

            Ready();
        }

        /// <summary>
        /// 스킬 사용 준비가 되었다.
        /// </summary>
        public virtual void Ready()
        {
            this.coolDowning = false;
        }

        /// <summary>
        /// 스킬을 사용한다.
        /// </summary>
        public virtual void Action()
        {
            SetCooltime();
            this.coolDowning = true;

            //// 기본공격은 로그 표시안함
            //if (!(this is DefaultAttack))
            //    KLog.Log("Use Skill. " + this.skillKey);
        }

        /// <summary>
        /// 스킬을 사용할 수 있는지
        /// </summary>
        /// <returns></returns>
        public virtual bool Usable(bool checkCooltime = true)
        {
            // 쿨타임 && 스턴 상태이상 여부
            return (checkCooltime ? this.remainCooltime <= 0 : true) && !this.unit.conditionModule.HasCondition(UnitCondition.Stun);
        }

        /// <summary>
        /// 쿨타임을 적용한다
        /// </summary>
        public void SetCooltime()
        {
            this.remainCooltime     = this.cooltime;
            this.skillUseRemainTime = this.skillUseTime;
        }
    }
}

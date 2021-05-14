using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 유닛에게서 데이터를 받아서 적절한 메카님 애니메이션을 재생시켜준다
    /// </summary>
    public class UnitAnimatorController : MonoBehaviour
    {
        #region Paramerter
        /// <summary>
        /// Locomotion에 사용(float)
        /// </summary>
        protected static readonly int PARAM_HORIZONTAL  = Animator.StringToHash("horizontal");
        /// <summary>
        /// Locomotion에 사용(float)
        /// </summary>
        protected static readonly int PARAM_VERTICAL    = Animator.StringToHash("vertical");
        /// <summary>
        /// 이동(bool)
        /// </summary>
        protected static readonly int PARAM_IS_MOVING   = Animator.StringToHash("isMoving");
        /// <summary>
        /// 죽음(bool)
        /// </summary>
        protected static readonly int PARAM_IS_DEATH    = Animator.StringToHash("isDeath");
        /// <summary>
        /// 스턴(Trigger)
        /// </summary>
        protected static readonly int PARAM_STUN        = Animator.StringToHash("Stun");
        /// <summary>
        /// 스턴중인지(bool)
        /// </summary>
        protected static readonly int PARAM_IS_STUN     = Animator.StringToHash("isStun");
        /// <summary>
        /// 히트(Trigger)
        /// </summary>
        protected static readonly int PARAM_HIT         = Animator.StringToHash("Hit");
        /// <summary>
        /// 넉다운(Trigger)
        /// </summary>
        protected static readonly int PARAM_KNOCKDOWN   = Animator.StringToHash("KnockDown");
        /// <summary>
        /// 공격(Trigger)
        /// </summary>
        protected static readonly int PARAM_ATTACK      = Animator.StringToHash("Attack");
        /// <summary>
        /// 죽음(Trigger)
        /// </summary>
        protected static readonly int PARAM_DIE         = Animator.StringToHash("Die");
        #endregion Parameter

        protected Animator anim { get; private set; }
        protected Unit     unit { get; private set; }

        /// <summary>
        /// 유닛이 죽었다.
        /// </summary>
        protected bool death { get; private set; }



        #region Unity
        protected virtual void Awake()
        {
            // 캐싱
            this.anim = GetComponent<Animator>();
            this.unit = GetComponentInParent<Unit>();

            // 에러체크
            if (this.anim == null)
                KLog.LogError("Animator Controller는 Animator가 붙어 있어야 함", this);
            if (this.unit == null)
                KLog.LogError("Unit 컴포넌트 찾지못함", this);
        }
        #endregion Unity

        #region Event
        /// <summary>
        /// Called By Animation Event
        /// Index에 해당되는 공격(무기 등)을 활성화 한다.
        /// </summary>
        public void OnStartAttackEvent(int index)
        {
            //Debug.Log("Active. " + this.unit.unitKey);
            this.unit.OnStartAttackEvent(index);
        }

        /// <summary>
        /// Called By Animation Event
        /// Index에 해당되는 공격(무기 등)을 비활성화 한다.
        /// </summary>
        public void OnFinishAttackEvent(int index)
        {
            //Debug.Log("Inactive. " + this.unit.unitKey);
            this.unit.OnFinishAttackEvent(index);
        }

        /// <summary>
        /// index가 0이면 왼쪽, 1이면 오른쪽 발
        /// </summary>
        public virtual void OnFootStep(int index)
        {
        }
        #endregion Event

        public virtual void UpdateParameters()
        {
            // 이동관련 애니메이션 파라메터 처리
            if (this.unit.isPlayer)
            {
                this.anim.SetFloat(PARAM_HORIZONTAL, InputManager.CharacterMoveAxis().x);
                this.anim.SetFloat(PARAM_VERTICAL, InputManager.CharacterMoveAxis().y);
            }
            else
            {
                this.anim.SetFloat(PARAM_HORIZONTAL, this.unit.moveDirection.x);
                this.anim.SetFloat(PARAM_VERTICAL, this.unit.moveDirection.y);
            }
            this.anim.SetBool(PARAM_IS_MOVING, this.unit.isMoving);
        }

        #region Define Trigger
        /// <summary>
        /// 일반 공격에 주로 사용됨
        /// </summary>
        public virtual void TriggerAttack()
        {
            if (this.death)
                return;

            this.anim.SetTrigger(PARAM_ATTACK);
        }

        /// <summary>
        /// 죽음 애니메이션 재생
        /// </summary>
        public virtual void TriggerDie()
        {
            this.death = true;

            this.anim.SetBool(PARAM_IS_DEATH, true);
            this.anim.SetTrigger(PARAM_DIE);
        }

        /// <summary>
        /// 스턴 애니메이션 재생
        /// </summary>
        public virtual void TriggerStun()
        {
            this.anim.SetBool(PARAM_IS_STUN, true);
            this.anim.SetTrigger(PARAM_STUN);
        }

        /// <summary>
        /// 스턴이 종료되면 자동으로 호출됨
        /// </summary>
        public virtual void FinishStun()
        {
            this.anim.SetBool(PARAM_IS_STUN, false);
        }
        #endregion Define Trigger

        public virtual void SetTrigger(int trigger)
        {
            if (this.death)
                return;

            this.anim.SetTrigger(trigger);
        }

        public virtual void SetBool(int key, bool value)
        {
            if (this.death)
                return;

            this.anim.SetBool(key, value);
        }
    }
}
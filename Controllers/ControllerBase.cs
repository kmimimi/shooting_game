using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 모든 컨트롤러의 베이스 클래스. Unit으로부터 이벤트를 받아와서 호출됨
    /// 
    /// 유닛 초기화 직후 -> 유닛 활동 전   -> 유닛이 죽은 후
    /// OnInit()         -> OnLiveUpdate() -> OnDie() 
    /// </summary>
    public abstract class ControllerBase : MonoBehaviour
    {
        public Unit unit { get; private set; }




        #region Unity
        protected virtual void Awake()
        {
            // 캐싱
            this.unit = GetComponent<Unit>();

            // 에러체크
            if (this.unit == null)
                Debug.LogError("Null", this);
        }
        #endregion Unity


        #region Unit Events
        /// <summary>
        /// Called By Unit
        /// 유닛의 초기화가 이루어지면 호출된다.
        /// </summary>
        public virtual void OnInit() { }
        /// <summary>
        /// Called By Unit
        /// 유닛이 죽으면 호출된다.
        /// </summary>
        public virtual void OnDie() { }
        /// <summary>
        /// Called By Unit
        /// 유닛이 대미지를 입으면 호출된다.
        /// </summary>
        public virtual void OnHit(Unit attackedUnit, float finalDamage) { }
        /// <summary>
        /// Called By Unit
        /// </summary>
        public virtual void OnLiveUpdate() { }
        /// <summary>
        /// Called By Unit
        /// </summary>
        public virtual void OnDeathUpdate() { }
        /// <summary>
        /// Called By Unit
        /// </summary>
        public virtual void OnLateLiveUpdate() { }
        #endregion Unit Event
    }
}

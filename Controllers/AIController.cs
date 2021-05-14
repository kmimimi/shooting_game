using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace KShooting
{
    /// <summary>
    /// AI들이 사용하는 컨트롤러 기본 클래스. 
    /// </summary>
    public abstract class AIController : ControllerBase
    {
        private static Dictionary<string, List<AIController>> _aggroGroups = new Dictionary<string, List<AIController>>();
        
        [Tooltip("어그로 그룹. 이 집단에 속해있는 유닛은 동시에 타겟을 인식한다. 없으면 단독인식")]
        public string aggroGroupKey = string.Empty;
        [Tooltip("순찰하고 싶을때")]
        public Vector3[] patrolPositions = new Vector3[0];


        public Unit         target { get; private set; }

        protected UI.UnitStatusBar unitStatusBar;
        protected UI.BossStatusBar bossStatusBar;

        private int   patrolIndex;
        private float patrolDeltaTime;




        #region Unit
        public override void OnInit()
        {
            base.OnInit();
            InitStatusBar(); // 유닛 HP바를 생성한다.

            // 그룹이 있을경우 그룹에 등록한다
            if(!string.IsNullOrEmpty(this.aggroGroupKey))
            {
                // 딕셔너리에 키 없으면 등록
                if (!_aggroGroups.ContainsKey(this.aggroGroupKey))
                    _aggroGroups.Add(this.aggroGroupKey, new List<AIController>());

                _aggroGroups[this.aggroGroupKey].Add(this);
            }

            this.patrolIndex = 0;
        }

        public override void OnDie()
        {
            base.OnDie();

            // 체력표시용 UI들 비활성화!
            if (this.unitStatusBar != null)
                ObjectPoolManager.inst.Return(this.unitStatusBar.gameObject);

            if (this.bossStatusBar != null)
                this.bossStatusBar.gameObject.SetActive(false);

            this.unitStatusBar = null;
            this.bossStatusBar = null;

            // 그룹에 등록되어 있을 경우, 그룹에서 제외한다
            if(!string.IsNullOrEmpty(this.aggroGroupKey))
                _aggroGroups[this.aggroGroupKey].Remove(this);
        }

        public override void OnLiveUpdate()
        {
            base.OnLiveUpdate();

            // 순찰(구현은 일단 심플하게 해둠)
            if(this.target == null && this.patrolPositions.Length > 0)
            {
                if(this.patrolDeltaTime <= 0)
                {
                    // 인덱스 초과하면 0부터 시작
                    this.patrolIndex++;
                    if(this.patrolIndex >= this.patrolPositions.Length)
                        this.patrolIndex = 0;

                    // 유닛 이동
                    this.patrolDeltaTime = 5;
                }
                else
                {
                    this.patrolDeltaTime -= TimeManager.deltaTime;
                }

                MoveTargetUpdate(this.patrolPositions[this.patrolIndex], 0.1f);
            }

            // 보스 상태바 활성화 조건
            // 보스 상태바가 없고 &&  타겟이 존재하며 && 타겟이 플레이어일경우
            if (this.unit.isBoss && this.bossStatusBar == null && this.target != null && this.target.controller is PlayerController)
                this.bossStatusBar = StageMaster.current.playerController.hud.LoadBossStatusBar(this.unit);

            // 타겟이 죽었으면 비움
            if (this.target != null && this.target.unitState == UnitState.Death)
                SetTarget(null);
        }

        public override void OnHit(Unit attackedUnit, float finalDamage)
        {
            base.OnHit(attackedUnit, finalDamage);

            // 누군가 날 때렸으면 어그로
            if(attackedUnit != null && attackedUnit.unitSide != this.unit.unitSide)
                SetTarget(attackedUnit);
        }
        #endregion Unit

        /// <summary>
        /// 유닛 상태 바를 생성한다
        /// </summary>
        protected virtual void InitStatusBar()
        {
            if (!this.unit.isBoss)
            {
                ////////////////////////////////////////
                // 일반 유닛용 스테이터스 바 생성
                if (this.unitStatusBar == null)
                    this.unitStatusBar = ObjectPoolManager.inst.Get<UI.UnitStatusBar>(PrefabPath.UI.UnitStatusBar);

                // Transform 지정
                this.unitStatusBar.transform.SetParent(this.unit.pivots.overhead);

                RectTransform rt = this.unitStatusBar.transform as RectTransform;

                rt.anchoredPosition3D = Vector3.zero;
                rt.localRotation = Quaternion.identity;
                rt.sizeDelta = new Vector2(300 * this.unit.radius * 2, rt.sizeDelta.y);
                rt.localScale = Vector3.one * 0.01f;

                this.unitStatusBar.Init(this.unit);
            }
        }

        #region AI
        /// <summary>
        /// target을 비울 때, null로 들어오니 주의할것.
        /// </summary>
        public void SetTarget(Unit target)
        {
            // 타겟이 같은경우 무시
            if(this.target == target)
                return;

            // 살아있는 유닛이 아니면 타겟세팅 X
            if (target != null && target.unitState != UnitState.Live)
                return;

            // 타겟을 셋팅한다(
            this.target = target;

            // 어그로 그룹에 있을경우, 주변 유닛들에게 타겟인식을 알린다
            if (!string.IsNullOrEmpty(this.aggroGroupKey))
            {
                foreach(var ctr in _aggroGroups[this.aggroGroupKey])
                {
                    // 타겟이 이미 있는 경우는 무시한다.
                    if(ctr.target != null)
                        continue;

                    ctr.SetTarget(target);
                }
            }

            // UI
            // 적이고 && 보스가 아닐경우
            if(this.target != null && this.unit.unitSide == UnitSide.Enemy && !this.unit.isBoss)
            {
                AggroIcon icon = ObjectPoolManager.inst.Get<AggroIcon>(PrefabPath.UI.AggroIcon);

                icon.transform.SetParent(this.unit.transform, false);
                icon.transform.position = this.unit.pivots.overhead.position + Vector3.up * 0.7f;
                icon.transform.localScale = Vector3.one;

                icon.Init();
            }
        }

        /// <summary>
        /// 타겟을 찾는것을 시도한다.
        /// </summary>
        public bool TrySearchTarget()
        {
            // 타겟을 찾는다
            List<Unit> findUnits = UnitManager.GetNearUnitsNonAlloc(this.unit.transform.position, this.unit.searchRange, UnitSide.Friendly);

            for (int i = 0 ; i < findUnits.Count ; i++)
            {
                // 나 자신은 거른다
                if (findUnits[i] == this.unit)
                    continue;

                // 가장 가까운 유닛
                SetTarget(findUnits[i]);
                return true;
            }

            return false;
        }

        /// <summary>
        /// TargetPosition으로 접근함
        /// true를 리턴하면 타겟에 접근했음
        /// </summary>
        /// <param name="targetPosition"> 이동할 타겟 포지션(Transfrom</param>
        /// <param name="range">접근 영역값</param>
        public bool MoveTargetUpdate(Vector3 targetPosition, float range)
        {
            // 회전
            Vector3 lookPos = targetPosition - this.unit.transform.position;
            lookPos.y = 0; // y축은 무시한다

            if (!lookPos.IsZero())
                this.unit.RotateUnit(Quaternion.LookRotation(lookPos));


            // 공격하기에 거리가 멀면 다가간다
            if ((targetPosition - this.unit.transform.position).sqrMagnitude > (range * range))
            {
                this.unit.SetMovable(true);
                this.unit.MovePosition(targetPosition);

                return false;
            }
            else
            {
                this.unit.SetMovable(false);

                return true;
            }
        }

        /// <summary>
        /// 타겟을 바라본다
        /// </summary>
        public void RotateTarget()
        {
            this.unit.RotateUnit(Quaternion.LookRotation(this.target.transform.position - this.unit.transform.position));
        }
        #endregion AI
    }
}

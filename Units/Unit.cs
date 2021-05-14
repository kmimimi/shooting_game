using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace KShooting
{
    /// <summary>
    /// 유닛의 기본클래스. 
    /// 
    /// 유닛 초기화           -> 유닛 활동                -> 죽음
    /// Init()                -> LiveUpdate()             -> Die() 
    ///  - 스킬(InitSkill)      - 공격(AttackEvent)          - 죽을떄 하는 행동(DeathUpdate)
    ///  - 무기(InitWeapon)     - 피해 입음(DealDamage)   
    ///  
    ///                         - 행동(Controller)
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public abstract class Unit : MonoBehaviour, DefaultAttack.IDefaultAttackEvent
    {
        #region Serialize Field
        [Header("에디터 테스트용")]
        [SerializeField] private bool viewSearchRange = false;

        [Header("초기화 정보")]
        [Tooltip("자동으로 유닛 초기화 실행")]
        [SerializeField] private bool autoInitialize = false;

        [Header("유닛 컴포넌트")]
        [Tooltip("캐릭터 컨트롤러")]
        [SerializeField] private CharacterController _chCtrl = null;
        [Tooltip("유닛과 컨트롤러는 무조건 같이 붙어있어야 한다")]
        [SerializeField] private ControllerBase _controller = null;
        [Tooltip("애니메이션")]
        [SerializeField] private UnitAnimatorController _animCtrl = null;
        [Tooltip("유닛의 피벗. 무조건 채워져 있어야 한다")]
        [SerializeField] private UnitPivots _pivots = new UnitPivots();

        [Header("유닛 데이터")]
        [SerializeField] private string _unitName = "Unknown";
        [Tooltip("아군 적군판별")]
        [SerializeField] public UnitSide unitSide = UnitSide.Enemy;
        [Tooltip("보스일경우 true")]
        [SerializeField] private bool _isBoss = false;
        [Tooltip("최대체력")]
        [SerializeField] private float _maxHP = 100;
        [Tooltip("최대마력")]
        [SerializeField] private float _maxMP = 50;
        [Tooltip("이동속도")]
        [SerializeField] private float _moveSpeed = 3;
        [Tooltip("타겟 찾는 범위")]
        [SerializeField] private float _searchRange = 5;
        [Tooltip("공격사거리")]
        [SerializeField] private float _attackRange = 0.5f;
        [Tooltip("기본 공격력")]
        [SerializeField] private float _defaultDamage = 10;
        [Tooltip("기본공격 사이클")]
        [SerializeField] private float _defaultAttackCycle = 3;
        #endregion Serialize Field

        /// <summary>
        /// 유닛의 키. FIXME: 제대로 된 값을 받아오기 전까지 임시로 처리
        /// </summary>
        public string unitKey => this.gameObject.name.Replace("(Clone)", "");
        public virtual string dieSoundKey => string.Empty;

        #region Get Component
        /// <summary>
        /// 캐릭터 컨트롤러
        /// </summary>
        public CharacterController chCtrl => this._chCtrl;
        /// <summary>
        /// 컨트롤러
        /// </summary>
        public ControllerBase controller => this._controller;
        /// <summary>
        /// 애니메이션 컨트롤러
        /// </summary>
        public UnitAnimatorController animCtrl => this._animCtrl;
        /// <summary>
        /// 유닛 피벗
        /// </summary>
        public UnitPivots pivots => this._pivots;
        /// <summary>
        /// 유닛 스킬 모듈
        /// </summary>
        public SkillModule skillModule { get; private set; } = new SkillModule(null, null);
        /// <summary>
        /// 유닛 컨디션 모듈
        /// </summary>
        public ConditionModule conditionModule { get; private set; } = new ConditionModule(null);
        /// <summary>
        /// NavmeshAgent(Init함수 이후 초기화 된다)
        /// </summary>
        public NavMeshAgent agent { get; private set; }
        #endregion

        #region Get State
        /// <summary>
        /// 유닛 이름
        /// </summary>
        public string unitName => this._unitName;
        /// <summary>
        /// 보스면 true
        /// </summary>
        public bool isBoss => this._isBoss;
        /// <summary>
        /// 유닛 두꼐
        /// </summary>
        public float radius => this.chCtrl.radius;
        /// <summary>
        /// 최대 체력
        /// </summary>
        public float maxHP => this._maxHP;
        /// <summary>
        /// 현재 체력
        /// </summary>
        public float currHP { get; private set; }
        /// <summary>
        /// 최대마력
        /// </summary>
        public float maxMP => this._maxMP;
        /// <summary>
        /// 현재마력
        /// </summary>
        public float currMP { get; private set; }
        /// <summary>
        /// 기본 대미지
        /// </summary>
        public float defaultDamage => this._defaultDamage;
        /// <summary>
        /// 이동속도
        /// </summary>
        public virtual float moveSpeed => this._moveSpeed * (this.conditionModule.HasCondition(UnitCondition.Slow) ? 0.2f : 1);
        /// <summary>
        /// 회전속도
        /// </summary>
        public float rotateSmooth => 10;
        /// <summary>
        /// 점프높이
        /// </summary>
        public float jumpHeight => 1.5f;
        /// <summary>
        /// 탐색범위
        /// </summary>
        public float searchRange => this._searchRange;
        /// <summary>
        /// 공격사거리
        /// </summary>
        public float attackRange => this._attackRange;
        /// <summary>
        /// 유닛의 상태
        /// </summary>
        public UnitState unitState { get; private set; }
        /// <summary>
        /// 목표로 하는 회전값.
        /// </summary>
        public Quaternion targetRotation { get; private set; }
        /// <summary>
        /// 값이 zero가 아닐때 움직일 방향
        /// </summary>
        public Vector3 moveDirection { get; private set; }
        /// <summary>
        /// 목표에 도달해야 하는 위치
        /// </summary>
        public Vector3 targetPosition { get; private set; }
        /// <summary>
        /// 점프값. 0보다 크면 점프중인것이다.
        /// </summary>
        public float jumpVelocity { get; private set; }
        /// <summary>
        /// 유닛이 이동중이다
        /// </summary>
        public bool isMoving => this.moveDirection != Vector3.zero;
        /// <summary>
        /// 이동가능 여부
        /// </summary>
        public virtual bool movable
        {
            get
            {
                if (this.conditionModule.HasCondition(UnitCondition.Stun))
                    return false;

                if (this.isPlayer)
                    return  true;
                else
                {
                    return this.agent.enabled && !this.agent.isStopped;
                }
            }
        }
        /// <summary>
        /// 유닛이 점프중이다
        /// </summary>
        public bool isJump =>this.jumpVelocity > 0;
        /// <summary>
        /// 유닛이 땅을 밟고 있다.
        /// </summary>
        public bool isGrounded { get; private set; }
        /// <summary>
        /// 이 유닛은 플레이어가 조종한다
        /// </summary>
        public bool isPlayer { get; private set; }
        #endregion Get State

        #region Inner Property
        private Collider[] cols;
        /// <summary>
        /// 죽은이후 deathElapsedTime만큼 지났다.
        /// </summary>
        private float deathElapsedTime;
        #endregion Inner Property



        public virtual void Init()
        {
            // 데이터 초기화
            this.targetRotation = this.transform.rotation;
            this.moveDirection = Vector3.zero;
            this.jumpVelocity = 0;
            this.currHP = this._maxHP;
            this.currMP = this._maxMP;
            this.unitState = UnitState.Live;

            // 스킬 모듈 초기화
            this.skillModule = new SkillModule(this, new DefaultAttack(this._defaultAttackCycle));
            InitSkill(this.skillModule);
            InitWeapon();

            this.isPlayer = this.controller is PlayerController;

            // 네브메쉬 초기화
            if (this.agent == null)
            {
                this.agent = this.gameObject.AddComponent<NavMeshAgent>();

                this.agent.radius = this.chCtrl.radius;
                this.agent.height = this.chCtrl.height;
                this.agent.speed = this.moveSpeed;

                // 플레이어는 네브메쉬 만들고 끔.
                this.agent.enabled = !this.isPlayer;
            }

            // 컨디션 모듈 초기화
            this.conditionModule = new ConditionModule(this);
            this.controller.OnInit();

            // 모든 콜라이더 캐싱 및 초기화
            this.cols = GetComponentsInChildren<Collider>();
            //for (int i = 0 ; i < this.cols.Length ; i++)
            //    this.cols[i].enabled = true;

            MovePosition(this.transform.position);
        }

        #region Abstract
        /// <summary>
        /// 스킬관련 초기화시 사용
        /// </summary>
        public abstract void InitSkill(SkillModule module);

        /// <summary>
        /// 무기 초기화
        /// </summary>
        public abstract void InitWeapon();
        #endregion Abstarct

        #region Unity
        protected virtual void Reset()
        {
            this._chCtrl     = GetComponent<CharacterController>();
            this._controller = GetComponent<ControllerBase>();
            this._animCtrl   = GetComponentInChildren<UnitAnimatorController>();
        }

        protected virtual void Awake()
        {
            // 에러체크
            if (this.chCtrl == null)
                KLog.LogError("Character Controller없음", this);
            if (this.controller == null)
                KLog.LogError("Controller Base 없음", this);
            if (this.animCtrl == null)
                KLog.LogError("애니메이션 컨트롤러가 없음", this);
            if (!this.pivots.IsVaild())
                KLog.LogError("피벗정보가 비어있음", this);
        }

        protected virtual void OnEnable()
        {
            UnitManager.AddUnit(this);
        }

        protected virtual void OnDisable()
        {
            UnitManager.RemoveUnit(this);
        }

        protected virtual void Start()
        {
            if (this.autoInitialize)
                Init();
        }

        protected virtual void Update()
        {
            this.skillModule.Update(TimeManager.deltaTime);
            this.conditionModule.Update(TimeManager.deltaTime);


            if (this.unitState == UnitState.Live)
            {
                this.controller.OnLiveUpdate();
                LiveUpdate();
                this.animCtrl.UpdateParameters();


                if (this.currHP <= 0)
                    Die();
            }
            else if(this.unitState == UnitState.Death)
            {
                this.controller.OnDeathUpdate();
                DeathUpdate();
            }
        }

        protected virtual void LateUpdate()
        {
            if (this.unitState == UnitState.Live)
            {
                this.controller.OnLateLiveUpdate();
                LateLiveUpdate();
            }
        }

        protected virtual void OnDrawGizmos()
        {
            // 탐색거리 표시
            if(this.viewSearchRange)
            {
                Color origin = Gizmos.color;

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(this.transform.position, this._searchRange);

                Gizmos.color = origin;
            }
        }
        #endregion Unity

        #region Update
        /// <summary>
        /// 유닛이 살아 있을때만 호출이 된다.
        /// </summary>
        protected virtual void LiveUpdate()
        {
            // 플레이어의 이동이랑 AI의 이동기능은 다른 메커니즘으로 구현된다.
            if (this.isPlayer)
                PlayerBehaviourUpdate();
            else
                AIBehaviourUpdate();
        }

        /// <summary>
        /// 유닛이 살아 있을때만 호출이 된다
        /// </summary>
        protected virtual void LateLiveUpdate()
        {

        }

        /// <summary>
        /// 유닛이 죽으면 호출된다.
        /// </summary>
        protected virtual void DeathUpdate()
        {
            this.deathElapsedTime += TimeManager.deltaTime;

            // 일정시간이 지나면
            if(this.deathElapsedTime > 3.0f)
            {
                if (this.isPlayer)
                {
                    // 플레이어일경우 게임 오버.
                    StageMaster.current.GameOver();
                }
                else
                {
                    // 플레이어가 아닐경우 삭제.
                    ObjectPoolManager.inst.Return(this.gameObject);
                }
            }
        }

        /// <summary>
        /// 플레이어 행동 업데이트
        /// </summary>
        protected virtual void PlayerBehaviourUpdate()
        {
            //스턴되면 Locomotion동작X
            if (this.conditionModule.HasCondition(UnitCondition.Stun))
                return;

            // 유닛 회전
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, this.targetRotation, TimeManager.deltaTime * this.rotateSmooth);


            if (this.movable)
            {
                // 유닛 최종 이동 계산
                Vector3 finalVel = Vector3.zero;

                // 유닛 이동
                if (this.moveDirection != Vector3.zero)
                {
                    finalVel += (this.moveDirection.normalized * this.moveSpeed);
                }
                finalVel.y += this.jumpVelocity;

                this.chCtrl.Move(finalVel * TimeManager.deltaTime);


                // 유닛이 바닥에 있는지 계산
                this.isGrounded = IsGrounded();
                if (this.isGrounded)
                    this.jumpVelocity = 0;
                else
                    this.jumpVelocity += Physics.gravity.y * TimeManager.deltaTime; // 땅에 있지 않으면 떨어지도록
            }
        }

        /// <summary>
        /// AI 행동 업데이트
        /// </summary>
        protected virtual void AIBehaviourUpdate()
        {
            bool isStun = this.conditionModule.HasCondition(UnitCondition.Stun);


            // 유닛 회전
            if (!isStun)
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, this.targetRotation, TimeManager.deltaTime * this.rotateSmooth);

            // 유닛 이동
            this.agent.speed = this.moveSpeed;
            if (this.movable && !isStun)
            {
                this.agent.isStopped = false;
                this.agent.SetDestination(this.targetPosition);
            }
            else
            {
                this.agent.isStopped = true;
            }
        }
        #endregion Update

        #region Event
        /// <summary>
        /// Called By SkillModule
        /// 
        /// 기본공격이 시작되면 호출되는 이벤트
        /// 코드상에서 기본공격을 시작할때 호출된다.
        /// </summary>
        public abstract void OnDefaultAttack_ActionEvent();

        /// <summary>
        /// Called By Animation Event
        /// 
        /// 애니메이션에서 공격할 타이밍에 이벤트로 넘어온다
        /// 0번째 인덱스는 기본공격으로 사용한다
        /// </summary>
        public virtual void OnStartAttackEvent(int index) { }

        /// <summary>
        /// Called By Animation Event
        /// 
        /// 애니메이션에서 비활성화가 필요할때 이벤트로 넘어온다
        /// 애니메이션에 따라 반드시 호출되는것은 아님.
        /// 0번째 인덱스는 기본공격으로 사용한다
        /// </summary>
        public virtual void OnFinishAttackEvent(int index) { }
        #endregion Event

        #region Action
        /// <summary>
        /// 유닛을 rotation값으로 회전시킨다.
        /// </summary>
        public virtual void RotateUnit(Quaternion targetRotation)
        {
            // 넉백중엔 회전 불가능
            if (this.conditionModule.HasCondition(UnitCondition.Knockback))
            {
                this.targetRotation = this.transform.rotation;
                return;
            }

            this.targetRotation = targetRotation;
        }

        /// <summary>
        /// 유닛을 dir방향으로 이동시킨다(Player Only)
        /// Controller에서 프레임단위로 호출됨.
        /// </summary>
        public virtual void MoveDirection(Vector3 moveDirection)
        {
            // 넉백중엔 이동 불가능
            if (this.conditionModule.HasCondition(UnitCondition.Knockback))
            {
                this.moveDirection = Vector3.zero;
                return;
            }

            this.moveDirection = moveDirection;
        }

        /// <summary>
        /// 유닛을 이동시킨다 (AI Only)
        /// Controller에서 프레임단위로 호출됨
        /// </summary>
        public virtual void MovePosition(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
            this.moveDirection  = this.agent.velocity;

        }

        public void SetMovable(bool isMovable)
        {
            this.agent.isStopped = !isMovable;
            this.moveDirection   = Vector3.zero;
        }

        public void UseItem(ItemInfo item)
        {
            // 포션 아이템만 사용가능
            if (item.itemKind != ItemKind.Potion)
                return;

            switch (item.itemKey)
            {
                case ItemKeys.HPPotion:
                    this.currHP = Mathf.Clamp(this.currHP + item.intValue, 0, this.maxHP);
                    break;
                case ItemKeys.MPPotion:
                    this.currMP = Mathf.Clamp(this.currMP + item.intValue, 0, this.maxMP);
                    break;
                default:
                    KLog.LogError("알수 없는 아이템 사용. " + item.itemKey);
                    break;
            }
        }

        /// <summary>
        /// 기본공격을 시도한다. 쿨타임, 딜레이 여부없이 무조건 공격
        /// </summary>
        public virtual void DefaultAttack()
        {
            this.skillModule.defaultAttack.Action();
        }

        /// <summary>
        /// 유닛을 점프시킨다
        /// </summary>
        public virtual bool Jump()
        {
            // 넉백중엔 점프 불가능
            if (this.conditionModule.HasCondition(UnitCondition.Knockback))
                return false;

            // 이미 점프중임
            if (this.isJump || !this.isGrounded)
                return false;
            
            // 점프
            float velocity = 2f * Mathf.Abs(Physics.gravity.y) * this.jumpHeight;
            velocity = Mathf.Sqrt(velocity);

            this.jumpVelocity = velocity;
            return true;
        }

        /// <summary>
        /// 유닛에게 대미지를 줌
        /// </summary>
        public virtual void DealDamage(Unit attackedUnit, float finalDamage)
        {
            // 대미지 이펙트 생성
            if (!(this.controller is PlayerController)) // 자기 자신이 입는 대미지는 표시 안함
            {
                var dmgViewer = ObjectPoolManager.inst.Get<UI.DamageViewer>(PrefabPath.UI.DamageViewer);
                dmgViewer.transform.position = this.transform.position + Random.insideUnitSphere;
                dmgViewer.Init(-finalDamage);
            }

            // 대미지를 줌
            this.currHP = Mathf.Max(this.currHP - finalDamage, 0);

            // 컨트롤러에 이벤트 보냄
            this.controller.OnHit(attackedUnit, finalDamage);
        }

        /// <summary>
        /// 유닛 죽음
        /// </summary>
        public virtual void Die()
        {
            if(!string.IsNullOrEmpty(this.dieSoundKey))
                SoundManager.inst.PlaySound(this.dieSoundKey, 0.2f);

            this.unitState = UnitState.Death;
            this.controller.OnDie();
            this.conditionModule.Collect();

            if (this.agent.enabled)
                this.agent.enabled = false;

            if(this.unitSide == UnitSide.Enemy)
            {
                // 퀘스트가 있다면 퀘스트 체크
                QuestManager.inst.CheckQuest_Accum(QuestCheckKey.QuestKey_Hunt_ + this.unitKey, 1);

                // 코인 뿌림. 값은 그냥 랜덤줌.
                int count = Random.Range(3, 5);
                for(int i=0; i<count; i++)
                {
                    CoinObject coin = ObjectPoolManager.inst.Get<CoinObject>(PrefabPath.Coin);

                    coin.transform.position = this.transform.position;
                    coin.Init(Random.Range(30, 50));
                    coin.PlayAnimation(KUtils.SamplePosition_NavMesh(this.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f))));
                }

                // 스코어 누적
                if (StageMaster.current != null)
                {
                    if (this.isBoss)
                        StageMaster.current.scoreInfo.killBoss++;
                    else
                        StageMaster.current.scoreInfo.killMonster++;
                }
            }

            // 죽으면 모든 콜라이더 끔
            for (int i = 0 ; i < this.cols.Length ; i++)
                this.cols[i].enabled = false;


            this.animCtrl.TriggerDie();
        }
        #endregion Action

        #region State
        /// <summary>
        /// CharacterController의 isGrounded로 체크하기에는 문제가 있음(땅에 있어도 값이 변화함)
        /// </summary>
        private bool IsGrounded()
        {
            if (this.chCtrl.isGrounded)
                return true;

            // 바닥에 레이를 쏴서 정말로 바닥에 있는지 확인
            var ray = new Ray(this.transform.position + Vector3.up * 0.1f, Vector3.down);
            var maxDistance = 0.2f;

            //Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * maxDistance, Color.red);
            return Physics.Raycast(ray, maxDistance, Layers.Group_Ground);
        }
        #endregion State
    }
}

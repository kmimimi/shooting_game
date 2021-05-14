using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// ---------- Metalon ------------
    /// 간단하게 특정 패턴을 반복하며 나오도록 해둠.
    /// 
    /// ---------- Metalon_Mini ------------
    /// 3초마다 Direct 360 Attack
    /// </summary>
    public class MetalonAIController : AIController
    {
        [SerializeField] private Transform centerTransform = null;

        private enum State
        {
            /// <summary>
            /// 기본공격
            /// </summary>
            DefaultAttack,

            /// <summary>
            /// 가운데로 이동
            /// </summary>
            MoveCenter,

            /// <summary>
            /// 타겟을 향해 이동
            /// </summary>
            MoveTarget,

            /// <summary>
            /// 특정 포지션으로 이동
            /// </summary>
            MovePosition,

            /// <summary>
            /// 360도 탄환 발사
            /// </summary>
            BulletAttack,

            /// <summary>
            /// 점프 공격
            /// </summary>
            JumpAttack,
            
            /// <summary>
            /// 자식소환
            /// </summary>
            CreateMini,

            /// <summary>
            /// 거미주 소환
            /// </summary>
            CreateSpiderWeb,
        }

        #region Mini Only
        private State[] miniState = new State[] { State.MovePosition, State.BulletAttack };

        /// <summary>
        /// State.MovePosition일떄. 이동할 위치
        /// </summary>
        public Vector3 targetPosition { get; set; }
        #endregion Mini Only

        #region Boss
        private State[] phase1State = new State[] {
            State.MoveCenter,
            State.BulletAttack,
            State.JumpAttack,
            State.MoveTarget,
            State.DefaultAttack,
            State.CreateMini,
            State.MoveTarget,
            State.DefaultAttack,
            State.CreateSpiderWeb,
        };
        #endregion Boss

        /// <summary>
        /// 현재 재생중인 State의 Index
        /// </summary>
        private int   stateIndex = 0;

        /// <summary>
        /// 0보다 클경우 대기한다
        /// </summary>
        private float delay;
        
        /// <summary>
        /// 캐싱용
        /// </summary>
        private Metalon metalon;



        public override void OnInit()
        {
            base.OnInit();
            this.metalon = this.unit as Metalon;


            if (!this.metalon.isMini && this.centerTransform == null)
                KLog.LogError("Metalon은 센터위치를 지정해 줘야함. ", this);
        }

        public override void OnLiveUpdate()
        {
            base.OnLiveUpdate();

            //스킬 테스트용
            if(Input.GetKeyDown(KeyCode.K))
            {
                this.metalon.SpellAttack(Metalon.SpellAttackKind.Summon);
                //this.metalon.JumpAttack();
                //this.unit.DefaultAttack();
                //this.metalon.SpellAttack(Metalon.SpellAttackKind.SpiderWeb);
                //this.metalon.SpellAttack(Metalon.SpellAttackKind.Direct);
            }

            if (this.delay > 0)
            {
                this.delay -= TimeManager.deltaTime;
                return;
            }

            // 타겟이 있을경우
            if (target != null)
            {
                State currState = this.metalon.isMini ? this.miniState[this.stateIndex] : this.phase1State[this.stateIndex];

                //페이즈 1
                switch (currState)
                {
                    case State.DefaultAttack:
                        {
                            this.delay = 3;

                            this.unit.skillModule.defaultAttack.Action();
                            NextState();
                        }
                        break;
                    case State.MoveCenter:
                        if(MoveTargetUpdate(this.centerTransform.position, 1))
                        {
                            NextState();
                        }
                        break;
                    case State.MoveTarget:
                        if (MoveTargetUpdate(this.target.transform.position, 4))
                        {
                            NextState();
                        }
                        break;
                    case State.MovePosition:
                        if (MoveTargetUpdate(this.targetPosition, 1))
                        {
                            NextState();
                        }
                        break;
                    case State.BulletAttack:
                        {
                            this.delay = 3;
                            this.metalon.SpellAttack(Metalon.SpellAttackKind.Direct);
                            NextState();
                        }
                        break;
                    case State.JumpAttack:
                        {
                            this.delay = 3;
                            this.metalon.JumpAttack();
                            NextState();
                        }
                        break;
                    case State.CreateMini:
                        {
                            this.delay = 3;
                            this.metalon.SpellAttack(Metalon.SpellAttackKind.Summon);
                            NextState();
                        }
                        break;
                    case State.CreateSpiderWeb:
                        {
                            this.delay = 3;
                            this.metalon.SpellAttack(Metalon.SpellAttackKind.SpiderWeb);
                            NextState();
                        }
                        break;
                    default:
                        break;
                }
            }
            else // 타겟이 없을경우
            {
                this.unit.SetMovable(false);
                TrySearchTarget();
            }
        }

        private void NextState()
        {
            this.stateIndex++;
            if (this.stateIndex >= (this.metalon.isMini ? this.miniState.Length : this.phase1State.Length))
                this.stateIndex = this.metalon.isMini ? 1 : 0;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KShooting.UI;


namespace KShooting
{
    /// <summary>
    /// 플레이어가 사용하는 Controller
    /// 플레이어의 입력을 받아 유닛을 조정하는 역할을 함
    /// </summary>
    public class PlayerController : ControllerBase
    {
        /// <summary>
        /// 생성된 HUD
        /// </summary>
        public GameHud hud { get; private set; }

        /// <summary>
        /// 조준 모드인지.
        /// </summary>
        public bool aimOn => PlayerCamera.current.aimOn;

        /// <summary>
        /// 조준 애니메이션이 재생중이다.
        /// </summary>
        private bool aimAnimating = false;

        /// <summary>
        /// 상호작용 아이콘. 상호작용가능한 오브젝트가 있을때 UI로 표시해준다.
        /// </summary>
        private InteractionIcon interactionIcon;

        /// <summary>
        /// 현재 상호작용가능한 제일 가까운 오브젝트
        /// </summary>
        private IInteractable interactableObject;


        #region Unit Event
        public override void OnInit()
        {
            base.OnInit();

            // UI생성
            this.hud = ObjectPoolManager.inst.New<GameHud>(PrefabPath.UI.GameHUD);
            
            this.hud.transform.position = new Vector3(0, -10000, 0);
            this.hud.Init(this.unit);

            // 카메라 초기화
            PlayerCamera.current.ResetRotation(this.unit.transform.eulerAngles);
            PlayerCamera.current.FollowUnit(this.unit.transform);
            this.aimAnimating = false;

            // 상호작용 아이콘 초기화
            this.interactionIcon = ObjectPoolManager.inst.New<InteractionIcon>(PrefabPath.UI.InteractionIcon);
            this.interactionIcon.gameObject.SetActive(false);
            
            // 커서 상태 잠금
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public override void OnLiveUpdate()
        {
            base.OnLiveUpdate();

            CursorUpdate();// 마우스 커서 관련 업데이트
            RotateUpdate();// 회전 업데이트
            AimingUpdate();// 에이밍
            MoveUpdate();// 이동 관련 업데이트
            JumpUpdate();// 점프
            AttackUpdate(); // 공격
            SkillUpdate(); // 스킬사용
            ItemUpdate(); // 아이템 사용
            InteractionUpdate(); // 상호작용
        }

        public override void OnDeathUpdate()
        {
            base.OnDeathUpdate();

            CursorUpdate();// 마우스 커서 관련 업데이트
            PlayerCamera.current.CameraRotateUpdate(); // 카메라 회전 업데이트(죽으면 카메라는 돌아가도록)
        }

        /// <summary>
        /// 아이템 사용을 시도한다. 이펙트는 일단 하드코딩
        /// </summary>
        public bool TryUseItem(string itemKey, string effectPath)
        {
            // 플레이어 유닛이 유효할 때,
            if (StageMaster.current.player != null)
            {
                //스턴 상태일땐 물약 못씀
                if (this.unit.conditionModule.HasCondition(UnitCondition.Stun))
                    return false;

                int itemCount = UserManager.GetItemCount(itemKey);

                // 개수가 1이상 인지 체크
                if (itemCount >= 1)
                {
                    // 아이템을 사용하고
                    StageMaster.current.player.UseItem(ItemDatabase.GetItemInfo(itemKey));

                    // 아이템 개수를 감소시킨다.
                    UserManager.AccumItem(itemKey, -1);

                    // 사운드 재생
                    SoundManager.inst.PlaySound(SoundKeys.EFFECT_DRINK, this.transform.position);

                    // 이펙트 재생
                    if (!string.IsNullOrEmpty(effectPath))
                    {
                        ParticleSystem ps = ObjectPoolManager.inst.Get<ParticleSystem>(effectPath);
                        ps.transform.SetParent(this.unit.transform);
                        ps.transform.position = this.unit.pivots.center.position;
                        ps.transform.localRotation = Quaternion.identity;
                        ps.transform.localScale = Vector3.one;
                        ps.Play();
                    }
                    return true;
                }
            }

            return false;
        }

        public override void OnLateLiveUpdate()
        {
            base.OnLateLiveUpdate();
            PlayerCamera.current.CameraPositionUpdate();
        }

        public override void OnDie()
        {
            base.OnDie();
            PlayerCamera.current.UnFollowUnit();
        }
        #endregion Unit Event

        /// <summary>
        /// 커서 관련기능 업데이트
        /// </summary>
        protected virtual void CursorUpdate()
        {
            // 토글기능
            if (UI.GameUI.inst.isActiveUI)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (InputManager.ToggleCursor())
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        /// <summary>
        /// 아이템 업데이트
        /// </summary>
        protected virtual void ItemUpdate()
        {
            if (StageMaster.current.isCombatPossible)
            {
                // 아이템
                if (InputManager.UseHPPotion())
                    TryUseItem(ItemKeys.HPPotion, PrefabPath.Particle.HPPotionEffect);
                if (InputManager.UseMPPotion())
                    TryUseItem(ItemKeys.MPPotion, PrefabPath.Particle.MPPotionEffect);
            }
        }

        /// <summary>
        /// 회전 업데이트
        /// </summary>
        protected virtual void RotateUpdate()
        {
            // 카메라 회전 
            if (!this.unit.conditionModule.HasCondition(UnitCondition.Stun))
                PlayerCamera.current.CameraRotateUpdate();

            // 카메라 정면
            Vector3 camForward = PlayerCamera.current.transform.forward;
            camForward.y = 0; // Y값은 의미가 없으므로 제거
            camForward = camForward.normalized;

            // 카메라에 맞게 캐릭터 방향 설정
            this.unit.RotateUnit(Quaternion.LookRotation(camForward));
        }

        /// <summary>
        /// 이동 업데이트
        /// </summary>
        protected virtual void MoveUpdate()
        {
            Vector2 moveAxis = Vector2.zero;

            // UI가 활성화 되어 있지 않고 && 스킬사용중이 아닐 때,
            if (!GameUI.inst.isActiveUI && !this.unit.skillModule.skillRunning)
                moveAxis = InputManager.CharacterMoveAxis();

            Vector3 targetDir = this.unit.transform.forward * moveAxis.y + this.unit.transform.right * moveAxis.x;

            this.unit.MoveDirection(targetDir);
        }

        /// <summary>
        /// 점프업데이트
        /// </summary>
        protected virtual void JumpUpdate()
        {
            // 점프 입력이 들어왔고 && UI가 비활성화 되어 있으며 && 스킬 사용중이 아닐 때
            if (InputManager.Jump() && !UI.GameUI.inst.isActiveUI && !this.unit.skillModule.skillRunning)
                this.unit.Jump();
        }

        /// <summary>
        /// 공격 업데이트
        /// 반환값이 True면 공격버튼 눌러짐
        /// </summary>
        protected virtual bool AttackUpdate()
        {
            // 공격
            if (StageMaster.current.isCombatPossible          // 전투 가능한 지역이고
                && InputManager.Fire()                        // 공격 입력이 들어왔고
                && Cursor.lockState == CursorLockMode.Locked  // 커서가 잠금 모드일때
                && !UI.GameUI.inst.isActiveUI                 // UI가 비활성화 되어 있고
                && !this.unit.skillModule.skillRunning)       // 스킬사용중이 아닐때
            {
                if (this.unit.skillModule.defaultAttack.Usable())
                    this.unit.DefaultAttack();

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 스킬 업데이트
        /// </summary>
        protected virtual void SkillUpdate()
        {
            // 전투 가능한 상태이고 && 스킬이 사용중이 아닐때
            if (StageMaster.current.isCombatPossible && !this.unit.skillModule.skillRunning)
            {
                // 0번은 기본스킬이므로 제외한다
                for (int i = 1 ; i < this.unit.skillModule.skills.Count ; i++)
                {
                    if (InputManager.Skill(i) && this.unit.skillModule.skills[i].Usable())
                    {
                        // 스킬 사용전 호출해야 하는 것들 먼저 호출
                        UseSkill(i);
                    }
                }
            }
        }

        /// <summary>
        /// 스킬 사용
        /// </summary>
        protected virtual void UseSkill(int index)
        {
            // 스킬 사용
            this.unit.skillModule.skills[index].Action();
            this.unit.skillModule.skills[index].SetCooltime();
        }

        /// <summary>
        /// 에이밍 관련 업데이트
        /// </summary>
        protected virtual void AimingUpdate()
        {
            if (InputManager.Aim() //에임 인풋이 들어오고
                && Cursor.lockState == CursorLockMode.Locked //커서가 잠금
                && !GameUI.inst.isActiveUI //UI창이 없고
                && !this.aimOn  //조준상태 아님
                && !this.aimAnimating//조준 애니메이션중이 아님
                && !this.unit.conditionModule.HasCondition(UnitCondition.Stun)) //스턴 상태이상이 아님
            {
                // 조준상태 ON
                this.aimAnimating = true;
                PlayerCamera.current.AimOn(OnAimingChanged);
            }
            // 조준상태이고  && 에임 인풋이 없고 && 조준 애니메이션이 아닐때 && 스킬사용중이 아님
            else if (this.aimOn && !InputManager.Aim() && !this.aimAnimating && !this.unit.skillModule.skillRunning)
            {
                // 조준상태 Off
                this.aimAnimating = true;
                PlayerCamera.current.AimOff(OnAimingChanged);
            }
        }

        /// <summary>
        /// 상호작용 관련 업데이트
        /// </summary>
        protected virtual void InteractionUpdate()
        {
            int   selectedIndex    = -1;
            float selectedDistance = 1000000;

            // UI가 활성화 되어 있으면 다른것들과 상호작용 못함.
            if (!UI.GameUI.inst.isActiveUI) 
            {
                // 상호작용이 가능한 오브젝트가 있는지 찾고, 있다면 아이콘 표시
                for (int i = 0 ; i < StageMaster.current.interactableObjects.Count ; i++)
                {
                    IInteractable io = StageMaster.current.interactableObjects[i];

                    // 상호작용 불가
                    if (!io.interactable)
                        continue;

                    // 적절한 거리내에 있는지 비교
                    float dist = (io.transform.position - this.unit.transform.position).sqrMagnitude;
                    float interactionDistance = io.interactionDistance * io.interactionDistance;

                    // 상호작용 가능한 거리에 있다.
                    if (dist <= interactionDistance)
                    {
                        // 이전루프에서 조건을 만족한 오브젝트가 있으면
                        // 거리비교후 제일 가까운 오브젝트와만 상호작용한다.
                        if (selectedIndex == -1)
                        {
                            // 조건을 만족한 첫 오브젝트면 바로 등록
                            selectedIndex = i;
                            selectedDistance = dist;
                        }
                        else
                        {
                            // 이전루프에서 체크된 오브젝트가 있었다.
                            if (selectedDistance > dist)
                            {
                                selectedIndex = i;
                                selectedDistance = dist;
                            }
                        }
                    }
                }

            }

            if(selectedIndex == -1) // 가까운데서 상호작용할 오브젝트가 없음
            {
                this.interactableObject = null;

                // 인터렉션 아이콘 비활성화
                if (this.interactionIcon.gameObject.activeInHierarchy)
                    this.interactionIcon.gameObject.SetActive(false);
            }
            else
            {
                this.interactableObject = StageMaster.current.interactableObjects[selectedIndex];

                // 인터렉션 아이콘 활성화
                if (!this.interactionIcon.gameObject.activeInHierarchy)
                    this.interactionIcon.gameObject.SetActive(true);

                // 아이콘의 적절한 위치를 설정한다.
                this.interactionIcon.transform.position = this.interactableObject.transform.position + this.interactableObject.GetIconOffset();

                // 상호작용
                if(InputManager.Interaction())
                {
                    this.interactableObject.InteractionEvent();
                }
            }
        }

        /// <summary>
        /// 조준 상태가 바뀔때 호출된다.
        /// </summary>
        private void OnAimingChanged(bool aiming)
        {
            this.aimAnimating = false;
        }
    }
}

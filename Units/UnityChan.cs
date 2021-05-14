using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    public class UnityChan : Unit
    {
        [SerializeField] private Gun _gun = null;


        public Gun gun => this._gun;
        public PlayerController playerCtrl { get; private set; }
        public UnityChanAnimatorController unityChanAnimCtrl { get; private set; }

        /// <summary>
        /// 공격중이다.
        /// </summary>
        public bool attacking { get; set; }

        public override float moveSpeed => CalcMoveSpeed();

        protected override void Awake()
        {
            base.Awake();

            this.playerCtrl        = this.controller as PlayerController;
            this.unityChanAnimCtrl = this.animCtrl   as UnityChanAnimatorController;

            // 에러체크
            if (this.playerCtrl == null)
                KLog.LogError("PlayerController 찾지못함", this);
            if (this.gun == null)
                KLog.LogError("Gun 없음)");
        }

        protected override void LiveUpdate()
        {
            base.LiveUpdate();

            // 총은 공격중에 무조건 정면을 바라보도록 처리.
            if (this.attacking && !this.conditionModule.HasCondition(UnitCondition.Stun))
                this.gun.transform.rotation = Quaternion.Euler(0, PlayerCamera.current.transform.eulerAngles.y, 0);
            else
                this.gun.transform.localRotation = Quaternion.identity;
        }

        public override void InitSkill(SkillModule module)
        {
            // 스킬 셋팅
            for(int i=0; i< UserManager.useSkills.Count; i++)
            {
                //사용할 수 있는 스킬만 초기화
                switch(UserManager.useSkills[i])
                {
                    case SkillKeys.PoisonShot: // 독
                        module.AddSkill(new PoisonShot(this.gun, 5));
                        break;
                    case SkillKeys.SnipeShot: // 저격
                        module.AddSkill(new SnipeShot(this.gun, 3));
                        break;
                    case SkillKeys.BombShot: // 폭탄
                        module.AddSkill(new BombShot(this.gun, 3));
                        break;
                    default:
                        KLog.LogError(string.Format("사용 불가능한 스킬. Unit: {0}, Skill: {1}", this.unitKey, UserManager.useSkills[i]));
                        break;
                }
            }
        }

        public override void InitWeapon()
        {
            this.gun.Init(this);
        }

        public override void OnDefaultAttack_ActionEvent()
        {
            //base.OnDefaultAttack_ActionEvent();
            if (PlayerCamera.current.aimOn) // 에임모드일땐 에임의 방향으로 날라감
                this.gun.fireDirection = CalcAimFireDirection();
            else // 에임모드가 아닐땐 정면으로 날라감
                this.gun.fireDirection = this.transform.forward;


            // 불릿 생성후 발사
            var bullet = ObjectPoolManager.inst.Get<StandardBullet>(PrefabPath.Projectile.StandardBullet);

            bullet.transform.position = this.gun.firePivot;
            bullet.transform.rotation = Quaternion.LookRotation(this.gun.fireDirection);


            // 불릿 초기화 및 발사
            bullet.Init(this, this.gun.power, 0.7f);
            bullet.FireDirection();

            SoundManager.inst.PlaySound(SoundKeys.EFFECT_DEFAULT_ATTACK, bullet.transform.position, 0.3f);
        }

        public float CalcMoveSpeed()
        {
            float finalSpeed = base.moveSpeed;

            if(PlayerCamera.current.aimOn) // 조준 모드일땐 이동속도 절반
                finalSpeed *= 0.5f;
            else
                finalSpeed *= (InputManager.Sprint() ? 2 : 1); // 달리기중일땐 이동속도 두배

            return finalSpeed;
        }

        /// <summary>
        /// 에임 모드일 때 날라갈 방향 계산.
        /// 이건 Unit에서 다른곳으로 옮겨야 할듯. 나중에 웨폰기능이 제대로 구현되면 옮기는걸로.ㄴ
        /// </summary>
        public Vector3 CalcAimFireDirection()
        {
            const float DISTANCE = 20;

            Ray ray = PlayerCamera.current.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            Vector3 targetPoint;

             
            if (Physics.Raycast(ray, out hit, DISTANCE, Layers.Group_Ground, QueryTriggerInteraction.Collide))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(DISTANCE);

            return targetPoint - this.gun.firePivot;
        }

        public override bool Jump()
        {
            bool ret = base.Jump();
            if (ret)
                SoundManager.inst.PlaySound(SoundKeys.EFFECT_JUMP, this.transform, Vector3.zero, 0.3f);

            return ret;
        }
    }
}

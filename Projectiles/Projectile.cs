using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 모든 발사체의 베이스 클래스.
    /// 유닛이나 다른 물체와 충돌할 때 사용함.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Projectile : MonoBehaviour
    {
        #region Define
        /// <summary>
        /// Projectile의 상태
        /// </summary>
        public enum FireState
        {
            /// <summary>

            /// 발사준비
            /// </summary>
            Ready,
            /// <summary>
            /// 발사중
            /// </summary>
            Fire,
            /// <summary>
            /// 발사끝
            /// </summary>
            End
        }

        /// <summary>
        /// 발사체 타입
        /// </summary>
        public enum ProjectileType
        {
            /// <summary>
            /// 직선
            /// </summary>
            Direct,
            /// <summary>
            /// 포물선
            /// </summary>
            Parabola,
            /// <summary>
            /// 유도탄
            /// </summary>
            Missile,
            /// <summary>
            /// 커스텀
            /// </summary>
            Custom
        }
        #endregion Define

        #region Serialize Field
        [Tooltip("리지드바디")]
        [SerializeField] private Rigidbody _rigid    = null;
        #endregion Serialize Field

        #region Property
        /// <summary>
        /// Projectie을 발사한 유닛
        /// </summary>
        public Unit owner { get; private set; }
        /// <summary>
        /// Projectile의 리지드바디
        /// </summary>
        public Rigidbody rigid => this._rigid;
        /// <summary>
        /// 발사체의 현재 이동속도
        /// </summary>
        protected float speed;
        /// <summary>
        /// 발사체의 수명(FireState가 Fire일때만 카운팅 됨)
        /// </summary>
        protected float lifeTime;
        /// <summary>
        /// 발사체의 상태
        /// </summary>
        protected FireState fireState { get; private set; }
        /// <summary>
        /// 발사체 날라가는 타입
        /// </summary>
        protected ProjectileType projectileType { get; private set; }
        
        // 포물선으로 날라갈 때 필요한 변수들
        #region Parabola Only
        private Vector2 parabola_vel;
        private float parabola_duration;
        private float parabola_elapsedTime;
        #endregion Parabola Only
        #endregion Property

        #region Effect Property
        /// <summary>
        /// 발사를 시작할 때, 보여줄 파티클 이펙트
        /// </summary>
        protected virtual string fireEffectPath => string.Empty;
        /// <summary>
        /// 발사를 시작할때 재생할 사운드
        /// </summary>
        protected virtual string fireSoundKey => string.Empty;
        /// <summary>
        /// 발사 사운드 볼륨
        /// </summary>
        protected virtual float fireSoundVolume => 1;
        /// <summary>
        /// 무언가에 맞았을때 보여줄 히트 이펙트
        /// </summary>
        protected virtual string hitEffectPath => string.Empty;
        /// <summary>
        /// 히트 이펙트 스케일(범위스킬의 경우 Override해서 적절한 크기로 설정 필요함)
        /// </summary>
        protected virtual float hitEffectScale => 1;
        /// <summary>
        /// 무언가에 맞았을때 재생할 사운드 패스
        /// </summary>
        protected virtual string hitSoundKey => string.Empty;
        /// <summary>
        /// 히트 사운드 볼륨
        /// </summary>
        protected virtual float hitSoundVolume => 1;
        #endregion Effect Property




        /// <summary>
        /// lifetime이 -1이면 무한
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="speed"></param>
        /// <param name="lifeTime"></param>
        public virtual void Init(Unit owner, float speed, float lifeTime)
        {
            this.owner = owner;

            this.fireState = FireState.Ready;
            this.lifeTime  = lifeTime;
            this.speed     = speed;
        }

        #region Unity
        protected virtual void Reset()
        {
            this._rigid = GetComponent<Rigidbody>();
        }

        protected void Update()
        {
            if(this.fireState == FireState.Fire)
            {
                FlyingUpdate(TimeManager.deltaTime);


                // 라이프타임 체크
                if (this.lifeTime >= 0)
                {
                    this.lifeTime -= TimeManager.deltaTime;
                    if (lifeTime <= 0) // 라이프타임이 종료되면
                    {
                        // 발사체를 소멸시킨다
                        this.fireState = FireState.End;
                        Finish();
                    }
                }
            }
        }

        protected void FixedUpdate()
        {
            if(this.fireState == FireState.Fire)
                FlyingFixedUpdate(TimeManager.fixedDeltaTime);
        }

        protected void OnTriggerEnter(Collider other)
        {
            Unit hitUnit = other.GetComponentInParent<Unit>();
            if (hitUnit != null)
            {
                // 충돌대상이 상대편 진영인경우
                if(hitUnit.unitSide != this.owner.unitSide)
                    Hit(hitUnit, other);
            }
            else
            {
                // 충돌 대상이 오브젝트인경우
                Hit(hitUnit, other);
            }
        }
        #endregion Unity


        #region Update
        /// <summary>
        /// Fire함수 호출 이후 수명을 다할떄까지 호출됨(프레임)
        /// </summary>
        /// <param name="dt"></param>
        protected virtual void FlyingUpdate(float dt)
        {
        }

        /// <summary>
        /// Fire함수 호출 이후 수명을 다할떄까지 호출됨(물리)
        /// </summary>
        protected virtual void FlyingFixedUpdate(float fixedDT)
        {
            switch (this.projectileType)
            {
                case ProjectileType.Direct:
                    DirectionUpdate(fixedDT);
                    break;
                case ProjectileType.Parabola:
                    ParabolaUpdate(fixedDT);
                    break;
                case ProjectileType.Missile:
                    break;
                case ProjectileType.Custom:
                    break;
            }
        }

        /// <summary>
        /// 직선으로 날라갈때 프레임으로 호출됨
        /// </summary>
        private void DirectionUpdate(float fixedDT)
        {
            this.rigid.MovePosition(this.rigid.position + this.transform.forward * this.speed * fixedDT);
        }

        /// <summary>
        /// 포물선으로 날라갈때 프레임으로 호출됨
        /// </summary>
        private void ParabolaUpdate(float fixedDT)
        {
            Vector3 pos = this.rigid.position + this.transform.TransformDirection(new Vector3(0, (this.parabola_vel.y - (-Physics.gravity.y * this.parabola_elapsedTime)) * fixedDT, this.parabola_vel.x * fixedDT));
            this.rigid.MovePosition(pos);

            this.parabola_elapsedTime += fixedDT * this.speed;
        }
        #endregion Update

        #region Fire
        /// <summary>
        /// 프로젝타일이 발사되면 호출됨. 직선 발사
        /// Projectile에 따라 지원 안하는것도 있을수 있음.
        /// </summary>
        public virtual void FireDirection()
        {
            this.fireState = FireState.Fire;
            this.projectileType = ProjectileType.Direct;

            TryFireEffect();
        }

        /// <summary>
        /// 프로젝타일이 발사되면 호출됨. 포물선 발사
        /// Projectile에 따라 지원 안하는것도 있을수 있음.
        /// </summary>
        public virtual void FireParabola(Vector3 targetPosition, float fireAngle = 45)
        {
            this.fireState = FireState.Fire;
            this.projectileType = ProjectileType.Parabola;


            float targetDist = Vector3.Distance(this.transform.position, targetPosition);
            float velocity   = targetDist / (Mathf.Sin(2 * fireAngle * Mathf.Deg2Rad) / -Physics.gravity.y);

            this.parabola_vel.x = Mathf.Sqrt(velocity) * Mathf.Cos(fireAngle * Mathf.Deg2Rad);
            this.parabola_vel.y = Mathf.Sqrt(velocity) * Mathf.Sin(fireAngle * Mathf.Deg2Rad);

            this.parabola_duration = targetDist / this.parabola_vel.x;

            this.transform.rotation = Quaternion.LookRotation(targetPosition - this.transform.position);
            this.parabola_elapsedTime = 0;

            TryFireEffect();
        }

        /// <summary>
        /// 프로젝타일이 발사되면 호출됨. 유도탄 발사
        /// Projectile에 따라 지원 안하는것도 있을수 있음.
        /// </summary>
        public virtual void FireMissile()
        {
            this.fireState      = FireState.Fire;
            this.projectileType = ProjectileType.Missile;

            TryFireEffect();
        }

        /// <summary>
        /// 프로젝타일이 발사되면 호출됨. 커스텀
        /// Projectile에 따라 지원 안하는것도 있을수 있음.
        /// </summary>
        public virtual void FireCustom()
        {
            this.fireState      = FireState.Fire;
            this.projectileType = ProjectileType.Custom;

            TryFireEffect();
        }

        /// <summary>
        /// 발사할 때, 파티클, 사운드를 재생한다.
        /// </summary>
        private void TryFireEffect()
        {
            // 파티클 재생
            if (!string.IsNullOrEmpty(this.fireEffectPath))
            {
                ParticleSystem ps = ObjectPoolManager.inst.Get<ParticleSystem>(this.fireEffectPath);

                ps.transform.position = this.transform.position;
                ps.transform.rotation = this.transform.rotation;

                ps.transform.localScale = Vector3.one;
                ps.Play();
            }

            // 사운드 재생
            if(!string.IsNullOrEmpty(this.fireSoundKey))
            {
                SoundManager.inst.PlaySound(this.fireSoundKey, this.transform.position, this.fireSoundVolume);
            }
        }
        #endregion Fire

        /// <summary>
        /// 발사체의 수명이 끝나서 오브젝트를 지움.
        /// 수명이 끝나도 바로 지우고 싶지 않으면 하위 클래스에서 오버라이드 한 후,
        /// base.EndLife() 호출을 안하면 됨.
        /// </summary>
        protected virtual void Finish()
        {
            ObjectPoolManager.inst.Return(this.gameObject);
        }

        /// <summary>
        /// 히트를 한뒤 프로젝타일을 지워야 하는경우, Finish()함수 호출할것.
        /// hitUnit이 null이면 유닛이 안맞음
        /// </summary>
        protected virtual void Hit(Unit hitUnit, Collider other)
        {
            if (!string.IsNullOrEmpty(this.hitEffectPath))
            {
                var ps = ObjectPoolManager.inst.Get<ParticleSystem>(this.hitEffectPath);

                ps.transform.position = this.transform.position;
                ps.transform.rotation = this.transform.rotation;
                ps.transform.localScale = Vector3.one * this.hitEffectScale;
                ps.Play();
            }
            if(!string.IsNullOrEmpty(this.hitSoundKey))
            {
                SoundManager.inst.PlaySound(this.hitSoundKey, this.transform.position, this.hitSoundVolume);
            }
        }
    }
}
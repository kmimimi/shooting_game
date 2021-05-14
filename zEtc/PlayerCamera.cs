using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KShooting
{
    /// <summary>
    /// 플레이어가 사용하는 카메라
    /// </summary>
    public sealed class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera current { get; private set; }


        [SerializeField] private Camera _cam = null;
        [SerializeField] private Transform _camPivot = null;
        [SerializeField] private ShakeObject shakeCom = null;

        [Tooltip("수평 회전 속도")]
        [SerializeField] private float _horizontalRotateSpeed = 6;

        [Tooltip("수직 회전 속도")]
        [SerializeField] private float _verticalRotateSpeed = 6;

        [Tooltip("기본모드일때 수직회전 범위(min, max)")]
        [SerializeField] private Vector2 _verticalRotateRange = new Vector2(-20, 40);

        [Tooltip("에임모드일때 수직회전 범위")]
        [SerializeField] private Vector2 _verticalAimingRotateRange = new Vector2(-20, 40);

        [Tooltip("에임모드일때 카메라 오프셋")]
        [SerializeField] private Vector3 _aimingOffset = new Vector3(1.3f, 1.66f, -1.36f);

        [Tooltip("에이밍 시간")]
        [SerializeField] private float _aimingTime = 0.2f;


        /// <summary>
        /// 카메라 컴포넌트
        /// </summary>
        public Camera cam         => this._cam;

        /// <summary>
        /// 카메라 피벗
        /// </summary>
        public Transform camPivot => this._camPivot;

        /// <summary>
        /// 마지막에 사용한 카메라 오일러 앵글
        /// </summary>
        private Vector3 lastEulerAngle;

        /// <summary>
        /// 카메라 초기 오프셋
        /// </summary>
        private Vector3 originOffset;

        /// <summary>
        /// 따라다닐 타겟
        /// </summary>
        private Transform followingTarget;

        /// <summary>
        /// 에임 관련 Coroutine
        /// </summary>
        private Coroutine aimCo;

        /// <summary>
        /// 카메라가 조준 모드인가
        /// </summary>
        public bool aimOn { get; private set; }


        private void Awake()
        {
            current = this;
            this.originOffset = this.camPivot.localPosition;
        }

        private void OnEnable()
        {
            this.lastEulerAngle = Vector3.zero;
            this.aimCo          = null;
            this.aimOn         = false;

            this.camPivot.transform.localPosition = this.originOffset;
        }
        
        /// <summary>
        /// 카메라가 target을 따라 다닌다
        /// </summary>
        /// <param name="target"></param>
        public void FollowUnit(Transform target)
        {
            this.followingTarget = target;
            this.transform.position = this.followingTarget.position;
        }

        /// <summary>
        /// 카메라가 따라다니는 target을 지운다
        /// </summary>
        public void UnFollowUnit()
        {
            this.followingTarget = null;
        }

        /// <summary>
        /// 회전 리셋
        /// </summary>
        public void ResetRotation(Vector3 euler)
        {
            this.transform.eulerAngles = euler;
            this.lastEulerAngle = euler;
        }

        /// <summary>
        /// 카메라 관련 프레임 업데이트
        /// </summary>
        public void CameraRotateUpdate()
        {
            // 카메라를 회전 시킬 값을 가져옴
            Vector2 cameraAxis;
            if (Cursor.lockState == CursorLockMode.Locked)
                cameraAxis = InputManager.CameraAimingAxis();
            else
                cameraAxis = Vector2.zero;

            // 카메라 회전속도 적용
            this.lastEulerAngle.y += cameraAxis.x * this._horizontalRotateSpeed;
            this.lastEulerAngle.x -= cameraAxis.y * this._verticalRotateSpeed;

            // 카메라 회전 범위 제한
            this.lastEulerAngle.y %= 360;
            if(this.aimOn)
                this.lastEulerAngle.x = Mathf.Clamp(this.lastEulerAngle.x, this._verticalAimingRotateRange.x, this._verticalAimingRotateRange.y);
            else
                this.lastEulerAngle.x = Mathf.Clamp(this.lastEulerAngle.x, this._verticalRotateRange.x, this._verticalRotateRange.y);


            // 카메라 방향 적용
            this.transform.eulerAngles = this.lastEulerAngle;
        }

        public void CameraPositionUpdate()
        {
            // 타겟 유닛이 있을경우, 타겟을 따라감
            if (this.followingTarget != null)
                this.transform.position = this.followingTarget.position;
        }

        public void ShakeCamera(float time = 1, float amount = 0.1f)
        {
            this.shakeCom.Play(time, amount);
        }

        /// <summary>
        /// 에임 기능 활성화
        /// 이벤트의 bool값은 true
        /// </summary>
        public void AimOn(UnityAction<bool> onComplete = null)
        {
            if (this.aimCo != null)
                StopCoroutine(this.aimCo);

            this.aimCo = StartCoroutine(AimOnUpdate(onComplete));
        }

        /// <summary>
        /// 에임 기능 비활성화
        /// 이벤트의 bool값은 false
        /// </summary>
        public void AimOff(UnityAction<bool> onComplete = null)
        {
            if (this.aimCo != null)
                StopCoroutine(this.aimCo);

            this.aimCo = StartCoroutine(AimOffUpdate(onComplete));
        }

        private IEnumerator AimOnUpdate(UnityAction<bool> onComplete)
        {
            float dt = 0;
            while(dt < this._aimingTime)
            {
                dt += TimeManager.deltaTime;
                this.camPivot.transform.localPosition = Vector3.Lerp(this.originOffset, this._aimingOffset, dt / this._aimingTime);
                yield return null;
            }

            this.aimOn = true;
            onComplete?.Invoke(true);
            this.aimCo = null;
        }

        private IEnumerator AimOffUpdate(UnityAction<bool> onComplete)
        {
            this.aimOn = false;

            float dt = 0;
            while (dt < this._aimingTime)
            {
                dt += TimeManager.deltaTime;
                this.camPivot.transform.localPosition = Vector3.Lerp(this._aimingOffset, this.originOffset, dt / this._aimingTime);
                yield return null;
            }

            onComplete?.Invoke(false);
            this.aimCo = null;
        }
    }
}

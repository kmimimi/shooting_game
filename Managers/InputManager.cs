using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 입력 매니저. 플레이어로부터의 입력은 전부 여기서 받아옴
    /// </summary>
    public static class InputManager
    {
        public static KeyCode useHPPotionKey => KeyCode.Delete;
        public static KeyCode useMPPotionKey => KeyCode.End;

        public const string MOUSE_X    = "Mouse X";
        public const string MOUSE_Y    = "Mouse Y";
        public const string HORIZONTAL = "Horizontal";
        public const string VERTICLA   = "Vertical";
        public const string JUMP       = "Jump";
        public const string FIRE1      = "Fire1";
        public const string SPRINT     = "Sprint";
        public const string AIM        = "Aim";
        public const string SKILL_     = "Skill";


        /// <summary>
        /// 현재 회전시킬 카메라의 Axis를 가져옴.
        /// </summary>
        public static Vector2 CameraAimingAxis()
        {
            return new Vector2(
                Mathf.Clamp(Input.GetAxis(MOUSE_X), -1, 1),
                Mathf.Clamp(Input.GetAxis(MOUSE_Y), -1, 1));
        }

        /// <summary>
        /// 캐릭터의 이동 Axis값
        /// </summary>
        /// <returns></returns>
        public static Vector2 CharacterMoveAxis()
        {
            return new Vector2(Input.GetAxisRaw(HORIZONTAL), Input.GetAxisRaw(VERTICLA));
        }

        /// <summary>
        /// 캐릭터 점프
        /// </summary>
        /// <returns></returns>
        public static bool Jump()
        {
            return Input.GetAxisRaw(JUMP) > 0.5f;
        }

        /// <summary>
        /// 기본공격
        /// </summary>
        public static bool Fire()
        {
            return Input.GetAxisRaw(FIRE1) > 0.5f;
        }

        /// <summary>
        /// 질주
        /// </summary>
        public static bool Sprint()
        {
            return Input.GetAxisRaw(SPRINT) > 0.5f;
        }

        /// <summary>
        /// 에임 활성화
        /// </summary>
        public static bool Aim()
        {
            return Input.GetAxisRaw(AIM) > 0.5f;
        }

        /// <summary>
        /// 스킬 사용
        /// </summary>
        public static bool Skill(int number)
        {
            return Input.GetAxisRaw(SKILL_ + number) > 0.5f;
        }

        /// <summary>
        /// 커서 토글링
        /// </summary>
        public static bool ToggleCursor()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }

        /// <summary>
        /// 상호작용키 
        /// </summary>
        public static bool Interaction()
        {
            return Input.GetKeyDown(KeyCode.E);
        }

        /// <summary>
        /// 다이얼로그 상호작용
        /// </summary>
        public static bool DialogInteraction()
        {
            return Input.GetMouseButtonDown(0);
        }

        /// <summary>
        /// HP포션 사용
        /// </summary>
        /// <returns></returns>
        public static bool UseHPPotion()
        {
            return Input.GetKeyDown(useHPPotionKey);
        }

        /// <summary>
        /// MP포션 사용
        /// </summary>
        /// <returns></returns>
        public static bool UseMPPotion()
        {
            return Input.GetKeyDown(useMPPotionKey);
        }
    }
}
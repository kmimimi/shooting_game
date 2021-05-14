using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 이 오브젝트는 플레이어와 상호작용이 가능하다.
    /// 
    /// 상호작용이 가능한 오브젝트는 StageMaster.current.RegistInteractable()로 등록해줘야 함.
    /// 오브젝트가 지워질때도 StageMaster.current.UnRegistInteractable()호출로 확실하게 처리해줄것.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// 게임오브젝트 접근
        /// </summary>
        GameObject gameObject { get; }

        /// <summary>
        /// 트랜스폼 접근
        /// </summary>
        Transform transform { get; }

        /// <summary>
        /// 상호작용이 가능한지
        /// </summary>
        bool interactable { get; }

        /// <summary>
        /// 상호작용이 가능할 때, Icon이 표시될 위치의 상대값
        /// </summary>
        Vector3 GetIconOffset();

        /// <summary>
        /// 인터렉션 가능한 거리(이 거리 안에 들어올때 상호작용이 가능하고, 아이콘이 활성화 된다)
        /// </summary>
        float interactionDistance { get; }

        /// <summary>
        /// 상호작용키를 눌렀다!
        /// </summary>
        void InteractionEvent();
    }
}
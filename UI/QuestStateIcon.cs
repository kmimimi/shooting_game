using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KShooting
{
    /// <summary>
    /// 퀘스트 상태를 표시해주는 아이콘
    /// </summary>
    public class QuestStateIcon : MonoBehaviour
    {
        public enum State
        {
            /// <summary>
            /// 퀘스트 없음
            /// </summary>
            Empty,
            /// <summary>
            /// 퀘스트를 가지고 있음
            /// </summary>
            HasQuest,
            /// <summary>
            /// 퀘스트 하나이상 진행중
            /// </summary>
            Progress,
            /// <summary>
            /// 퀘스트를 하나이상 완료가능
            /// </summary>
            Finish
        }


        [SerializeField] private SpriteChanger changer = null;

        public void SetState(State state)
        {
            this.changer.Change((int)state);
        }
    }
}
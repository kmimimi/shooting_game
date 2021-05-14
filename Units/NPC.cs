using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// NPC기본 클래스
    /// 
    /// - 말을 걸수 있어야 되고,
    /// - 퀘스트를 줄 수 있어야 됨.
    /// </summary>
    public abstract class NPC : MonoBehaviour, IInteractable
    {

        /// <summary>
        /// 화면에 표시되는 NPC이름
        /// </summary>
        [SerializeField] private string _npcName = "Unknown";

        /// <summary>
        /// NPC를 식별할 수 있는 키. FIXME: 제대로 된 값을 받아오기 전까지 임시로 처리
        /// </summary>
        public string npcKey  => this.gameObject.name.Replace("(Clone)", string.Empty);
        public string npcName => this._npcName;


        #region Interaction Interface
        /// <summary>
        /// 인터렉션 가능한 거리(이 거리 안에 들어올때 상호작용이 가능하고, 아이콘이 활성화 된다)
        /// </summary>
        public float interactionDistance => 3;

        /// <summary>
        /// 상호작용이 가능한지
        /// </summary>
        public virtual bool interactable { get { return true; } }

        /// <summary>
        /// 상호작용이 가능할 때, Icon이 표시될 위치의 상대값
        /// </summary>
        public Vector3 GetIconOffset()
        {
            return new Vector3(0, 2, 0); // 하드코딩
        }

        protected virtual void Start()
        {
            // StageMaster에게 이 오브젝트는 상호작용이 가능함을 알려야 한다.
            StageMaster.current.RegistInteractable(this);

            // 표시될 이름 생성
            UnitNameViewer viewer = ObjectPoolManager.inst.New<UnitNameViewer>(PrefabPath.UI.UnitNameViewer);

            viewer.transform.SetParent(this.transform, false);
            viewer.transform.localPosition = Vector3.up * 1.5f; // 위치는 하드코딩
            viewer.transform.localRotation = Quaternion.identity;
            viewer.transform.localScale = Vector3.one;

            viewer.Init(this.npcName);
        }

        protected virtual void OnDisable()
        {
            // 상호작용이 종료되면 StageMaster에게 이 오브젝트는 상호작용이 끝났음을 알려야 한다.
            if (StageMaster.current != null)
                StageMaster.current.UnRegistInteractable(this);
        }

        protected virtual void Update()
        {

        }

        /// <summary>
        /// 상호작용키를 눌렀다!
        /// </summary>
        public virtual void InteractionEvent()
        {
        }
        #endregion Interaction Interface
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KShooting
{
    /// <summary>
    /// 열매트리. 퀘스트용
    /// </summary>
    public class CuredBerriesTree : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject fruit = null;

        /// <summary>
        /// 인터렉션 가능한 거리(이 거리 안에 들어올때 상호작용이 가능하고, 아이콘이 활성화 된다)
        /// </summary>
        public float interactionDistance => 3;

        /// <summary>
        /// 상호작용이 가능한지
        /// </summary>
        public virtual bool interactable { get { return this.fruit.activeInHierarchy && QuestManager.inst.HasQuest("SubQuest_Collect_Fruit1"); } }


        private void Start()
        {
            // StageMaster에게 이 오브젝트는 상호작용이 가능함을 알려야 한다.
            StageMaster.current.RegistInteractable(this);
        }

        private void OnDisable()
        {
            // 상호작용이 종료되면 StageMaster에게 이 오브젝트는 상호작용이 끝났음을 알려야 한다.
            if (StageMaster.current != null)
                StageMaster.current.UnRegistInteractable(this);
        }

        public Vector3 GetIconOffset()
        {
            return new Vector3(0, 2, 0); // 하드코딩
        }

        public void InteractionEvent()
        {
            this.fruit.gameObject.SetActive(false);

            UserManager.AccumItem(ItemKeys.CuredBerries, 1);
            QuestManager.inst.CheckQuest_Set(QuestCheckKey.QuestKey_Collect_ + ItemKeys.CuredBerries, UserManager.GetItemCount(ItemKeys.CuredBerries));
        }
    }
}

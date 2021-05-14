using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace KShooting.UI
{
    /// <summary>
    /// 퀘스트 UI
    /// </summary>
    public class QuestUI : MonoBehaviour
    {
        [SerializeField] private ScrollRect      scrollRect = null;
        [SerializeField] private QuestDetailView detailView = null;


        private QuestUI_QuestElement[] elements = new QuestUI_QuestElement[0];


        private void Awake()
        {
            // 에러체크
            if (this.detailView == null)
                KLog.LogError("QuestDetailView is null", this);

            (this.detailView.transform as RectTransform).anchoredPosition3D = Vector3.zero;
        }

        private void OnDisable()
        {
            // UI가 꺼질때 GameUI에 UI가 꺼졌음을 알려줘야 한다.
            GameUI owner = GetComponentInParent<GameUI>();

            if (owner != null)
                owner.OnCloseChild();
        }

        public void Init()
        {
            this.detailView.gameObject.SetActive(false);

            // 이전에 있던 Element 반환
            for(int i=0; i<this.elements.Length; i++)
                ObjectPoolManager.inst.Return(this.elements[i].gameObject);

            // 현재 진행중인 퀘스트정보를 받아 리스트로 보여줌
            this.elements = new QuestUI_QuestElement[QuestManager.inst.progessingQuest.Count];
            for(int i=0; i< QuestManager.inst.progessingQuest.Count ; i++)
            {
                this.elements[i] = ObjectPoolManager.inst.Get<QuestUI_QuestElement>(PrefabPath.UI.QuestUI_QuestElement);
                this.elements[i].transform.SetParent(this.scrollRect.content, false);
                (this.elements[i].transform as RectTransform).anchoredPosition3D = Vector3.zero;
                this.elements[i].transform.localRotation = Quaternion.identity;
                this.elements[i].transform.localScale = Vector3.one;

                this.elements[i].Init(
                    this, 
                    QuestManager.inst.progessingQuest[i].key, 
                    QuestDatabase.GetInfo(QuestManager.inst.progessingQuest[i].key).title);
            }
        }

        /// <summary>
        /// Called By Element
        /// </summary>
        public void ClickElement(string questKey)
        {
            // 퀘스트 세부정보창 오픈
            this.detailView.gameObject.SetActive(true);
            this.detailView.Init(questKey);
        }

        /// <summary>
        /// Called By Button Event
        /// </summary>
        public void OnClose()
        {
            this.gameObject.SetActive(false);
        }
    }
}

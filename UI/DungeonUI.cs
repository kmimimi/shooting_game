using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace KShooting.UI
{
    /// <summary>
    /// 던전 리스트를 표시해주는 UI
    /// </summary>
    public class DungeonUI : MonoBehaviour
    {
        [SerializeField] private ScrollRect        scrollRect = null;
        [SerializeField] private DungeonDetailView detailView = null;


        private List<DungeonUI_Element> elements = new List<DungeonUI_Element>();


        private void Awake()
        {
            // 에러체크
            if (this.detailView == null)
                KLog.LogError("DungeonDetailView is null", this);

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
            for(int i=0; i<this.elements.Count; i++)
                ObjectPoolManager.inst.Return(this.elements[i].gameObject);

            this.elements.Clear();



            // 던전 리스트를 받아온다.
            for(int i=0; i< DungeonDatabase.info.Length; i++)
            {
                // 선행조건을 만족하지 않았다면 제외
                if (!UnlockManager.IsUnlock(DungeonDatabase.info[i].preKey))
                    continue;

                var element  = ObjectPoolManager.inst.Get<DungeonUI_Element>(PrefabPath.UI.DungeonUI_Element);
                element.transform.SetParent(this.scrollRect.content, false);
                (element.transform as RectTransform).anchoredPosition3D = Vector3.zero;
                element.transform.localRotation = Quaternion.identity;
                element.transform.localScale = Vector3.one;

                element.Init(this, DungeonDatabase.info[i].dungeonKey, DungeonDatabase.info[i].title);

                this.elements.Add(element);
            }
        }

        /// <summary>
        /// Called By Element
        /// </summary>
        public void ClickElement(string dungeonKey)
        {
            // 던전 세부정보창 오픈
            this.detailView.gameObject.SetActive(true);
            this.detailView.Init(this, dungeonKey);
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
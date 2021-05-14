using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace KShooting.UI
{
    /// <summary>
    /// 퀘스트 세부정보를 표시해준다.
    /// </summary>
    public class QuestDetailView : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText   = null;
        [SerializeField] private TMP_Text contentText = null;

        [SerializeField] private TMP_Text quickViewText = null;

        [SerializeField] private RectTransform rewardLayout = null;

        private QuestInfo questInfo;
        private QuestDetailView_RewardElement[] elements = new QuestDetailView_RewardElement[0];



        public void Init(string key)
        {
            this.questInfo = QuestDatabase.GetInfo(key);

            // 제목 텍스트 갱신
            this.titleText.text = this.questInfo.title;

            // 컨텐츠 텍스트 갱신
            string content = this.questInfo.content + "\n\n";
            for(int i=0; i< this.questInfo.ifText.Length; i++)
            {
                int progress = QuestManager.inst.GetProgress(this.questInfo.questKey, this.questInfo.requestKey[i]);
                content += string.Format("{0} <color={1}>[{2} / {3}]</color>\n", 
                    this.questInfo.ifText[i],
                    this.questInfo.request[i] <= progress ? Defines.TRUE_COLOR_CODE : Defines.FALSE_COLOR_CODE,
                    Mathf.Min(progress, this.questInfo.request[i]),
                    this.questInfo.request[i]);
            }
            this.contentText.text = content;

            // 보상 UI 처리
            for (int i = 0 ; i < this.elements.Length ; i++)
                ObjectPoolManager.inst.Return(this.elements[i].gameObject); // 기존 Element제거
            this.elements = new QuestDetailView_RewardElement[this.questInfo.rewardKey.Length];
            for(int i=0; i<this.questInfo.rewardKey.Length; i++)
            {
                this.elements[i] = ObjectPoolManager.inst.Get<QuestDetailView_RewardElement>(PrefabPath.UI.QuestDetailView_RewardElement);

                this.elements[i].transform.SetParent(this.rewardLayout, false);
                (this.elements[i].transform as RectTransform).anchoredPosition3D = Vector3.zero;
                this.elements[i].transform.localRotation = Quaternion.identity;
                this.elements[i].transform.localScale = Vector3.one;

                this.elements[i].Init(this.questInfo.rewardKey[i], this.questInfo.reward[i]);
            }

            // 퀵뷰버튼 처리
            UpdateQuickViewText();
        }

        /// <summary>
        /// Called By Button Event
        /// </summary>
        public void OnOKButtonClick()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Called By Button Event
        /// </summary>
        public void OnQuickViewButtonClick()
        {
            if (QuestManager.inst.HasQuickView(this.questInfo.questKey))
                QuestManager.inst.RemoveQuickView(this.questInfo.questKey);
            else
                QuestManager.inst.AddQuickView(this.questInfo.questKey);

            UpdateQuickViewText();
        }

        /// <summary>
        /// 퀵뷰 등록버튼 텍스트 갱신
        /// </summary>
        private void UpdateQuickViewText()
        {
            this.quickViewText.text = QuestManager.inst.HasQuickView(this.questInfo.questKey) ? "퀵뷰 등록 취소" : "퀵뷰 등록";
        }
    }
}
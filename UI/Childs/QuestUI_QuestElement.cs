using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KShooting.UI
{
    /// <summary>
    /// 퀘스트에 표시되는 요소
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class QuestUI_QuestElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText = null;
        [SerializeField] private Image    progressIcon = null;

        [Tooltip("퀘스트 진행중일 때 나오는 아이콘")]
        [SerializeField] private Sprite progressingSprite = null;
        [Tooltip("퀘스트 완료됬을 때 나오는 아이콘")]
        [SerializeField] private Sprite completeSprite = null;

        /// <summary>
        /// Quest Key
        /// </summary>
        public string key { get; private set; }

        private QuestUI owner;


        public void Init(QuestUI owner, string key, string title)
        {
            this.owner = owner;

            this.key            = key;
            this.titleText.text = title;

            // 퀘스트 상태에 따라 다른 아이콘 표시
            this.progressIcon.sprite = QuestManager.inst.IsFinishProgress(key) ? this.completeSprite : this.progressingSprite;
        }

        /// <summary>
        /// Called By Button Event
        /// </summary>
        public void OnClick()
        {
            this.owner.ClickElement(key);
        }
    }
}
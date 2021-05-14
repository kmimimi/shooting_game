using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KShooting.UI
{
    /// <summary>
    /// 던전 세부정보 표시용 UI
    /// </summary>
    public class DungeonDetailView : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText   = null;
        [SerializeField] private TMP_Text contentText = null;

        private DungeonInfo dungeonInfo;
        private DungeonUI owner;


        public void Init(DungeonUI owner, string key)
        {
            this.owner = owner;
            this.dungeonInfo = DungeonDatabase.GetInfo(key);

            // 제목 텍스트 갱신
            this.titleText.text = this.dungeonInfo.title;

            // 컨텐츠 텍스트 갱신
            this.contentText.text = this.dungeonInfo.content;
        }

        /// <summary>
        /// Called By Button Event
        /// </summary>
        public void OnOKButtonClick()
        {
            this.owner.gameObject.SetActive(false);

            // 맵으로 이동기능.
            StageMaster.current.ChangeScene(this.dungeonInfo.dungeonKey);
        }

        /// <summary>
        /// Called By Button Event
        /// </summary>
        public void OnCancelButtonClick()
        {
            this.gameObject.SetActive(false);
        }
    }
}
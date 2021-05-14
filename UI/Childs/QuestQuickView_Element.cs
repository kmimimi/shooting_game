using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace KShooting
{
    /// <summary>
    /// 화면 오른쪽에 퀘스트를 간단하게 보여줄때 각각의 요소
    /// </summary>
    public class QuestQuickView_Element : MonoBehaviour
    {
        /// <summary>
        /// 조건 만족
        /// </summary>
        private const string IF_CLEAR_FORMAT       = "{0} <color=" + Defines.TRUE_COLOR_CODE + ">[{1}/{2}]</color>\n";
        /// <summary>
        /// 조건 불만족
        /// </summary>
        private const string IF_PROGRESSING_FORMAT = "{0} <color=" + Defines.FALSE_COLOR_CODE + ">[{1}/{2}]</color>\n";

        [SerializeField] private TMP_Text titleText   = null;
        [SerializeField] private TMP_Text contentText = null;

        private QuestInfo questInfo;
        private float     deltaTime;

        /// <summary>
        /// 퀘스트 키
        /// </summary>
        public string key => this.questInfo.questKey;



        public void Init(string key)
        {
            this.questInfo = QuestDatabase.GetInfo(key);

            this.titleText.text = questInfo.title;

            // 퀵뷰 한번 갱신
            this.deltaTime = 100;
            Update();
        }

        private void Update()
        {
            // 매프레임 갱신할 필요는 없으므로 1초에 한번 갱신하게...
            this.deltaTime += Time.deltaTime;

            if (this.deltaTime < 1)
                return;

            this.deltaTime = 0;



            // 퀘스트 내용을 간략하게 표시해준다.
            string content = string.Empty;

            for (int i = 0 ; i < questInfo.ifText.Length ; i++)
            {
                // 현재 진행상황
                int progress = QuestManager.inst.GetProgress(this.questInfo.questKey, this.questInfo.requestKey[i]);
                // 요구량
                int request  = this.questInfo.request[i];


                // 요구량을 만족했으면 Clear색, 만족하지 못했으면 Progress색으로 처리
                content += string.Format(request <= progress ? IF_CLEAR_FORMAT : IF_PROGRESSING_FORMAT, this.questInfo.ifText[i], Mathf.Min(this.questInfo.request[i], progress), this.questInfo.request[i]);
            }

            // 컨텐츠 표시
            this.contentText.text = content;
        }
    }
}
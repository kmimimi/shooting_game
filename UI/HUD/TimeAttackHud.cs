using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace KShooting.UI
{
    /// <summary>
    /// 남은시간 표시해주는 HUD
    /// </summary>
    public class TimeAttackHud : MonoBehaviour
    {
        [SerializeField] private TMP_Text remainTimeText = null;

        private long lastTicks;
        private TimeAttackStage timeAttackStage;



        public void Init(TimeAttackStage timeAttackStage)
        {
            this.timeAttackStage = timeAttackStage;

            this.lastTicks = this.timeAttackStage.GetRemainTicks();
            UpdateTimeText();
        }

        private void OnDisable()
        {
            this.timeAttackStage = null;
        }

        private void Update()
        {
            long currTicks = this.timeAttackStage.GetRemainTicks();

            // UI에서 변경해줘야 할때만(초가 바뀜) UI업데이트
            if (currTicks + TimeSpan.TicksPerSecond <= this.lastTicks)
            {
                this.lastTicks = currTicks;
                UpdateTimeText();
            }
        }

        /// <summary>
        /// 남은시간 텍스트 UI갱신
        /// </summary>
        private void UpdateTimeText()
        {
            TimeSpan timeSpan = new TimeSpan(this.lastTicks);

            this.remainTimeText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace KShooting
{
    /// <summary>
    /// 남은 몬스터 수 표시용 HUD
    /// </summary>
    public class MonsterRemainCountHud : MonoBehaviour
    {
        [SerializeField] private TMP_Text remainCount = null;

        private int lastMonsterCount;
        private MonsterCleaningStage monsterCleaningStage;


        public void Init(MonsterCleaningStage monsterCleaningStage)
        {
            this.monsterCleaningStage = monsterCleaningStage;

            this.lastMonsterCount = this.monsterCleaningStage.remainEnemyCount;
            UpdateText();
        }

        private void Update()
        {
            if(this.monsterCleaningStage.remainEnemyCount != this.lastMonsterCount)
            {
                this.lastMonsterCount = this.monsterCleaningStage.remainEnemyCount;
                UpdateText();
            }
        }

        private void UpdateText()
        {
            this.remainCount.text = string.Format("{0} / {1}", this.monsterCleaningStage.remainEnemyCount, this.monsterCleaningStage.totalEnemyCount);
        }
    }
}

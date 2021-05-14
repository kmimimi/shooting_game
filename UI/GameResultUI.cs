using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


namespace KShooting.UI
{
    /// <summary>
    /// 게임 결과창 UI
    /// CreateElement()함수를 통해 결과정보를 출력해 줄 수 있다.
    /// </summary>
    public class GameResultUI : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect = null;
        [SerializeField] private TMP_Text   titleText = null;
        [SerializeField] private TMP_Text   totalScore = null;


        private List<GameResultUI_Element> elements = new List<GameResultUI_Element>();


        public void Init(bool isClear)
        {
            //이전에 사용된 element삭제
            for(int i=0; i<this.elements.Count; i++)
                ObjectPoolManager.inst.Return(this.elements[i].gameObject);

            this.elements.Clear();

            this.titleText.text = isClear ? "클리어" : "실패";


            TimeSpan timeSpan = new TimeSpan(StageMaster.current.scoreInfo.playTicks);
            int totalScore = 0;

            CreateElement("플레이 타임", string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds));
            if (isClear && StageMaster.current is TimeAttackStage) // 타임어택
            {
                TimeAttackStage stage = StageMaster.current as TimeAttackStage;

                // 시간 보너스
                int timeBonus = Mathf.CeilToInt((stage.timeLimit - (float)((stage.scoreInfo.endTicks - stage.scoreInfo.startTicks) / TimeSpan.TicksPerSecond)) * 1000);
                CreateElement("시간 보너스", KUtils.GetThousandCommaText(timeBonus));
                totalScore += timeBonus;
            }
            else if(isClear && StageMaster.current is MonsterCleaningStage) // 몬스터 소탕
            {
                MonsterCleaningStage stage = StageMaster.current as MonsterCleaningStage;

                // 처치보너스
                int killMonsterBonus = stage.scoreInfo.killMonster * 1500;
                if(killMonsterBonus > 0)
                {
                    CreateElement("처치 - 일반", KUtils.GetThousandCommaText(killMonsterBonus));
                    totalScore += killMonsterBonus;
                }

                int killBossBonus = stage.scoreInfo.killBoss * 30000;
                if (killBossBonus > 0)
                {
                    CreateElement("처치 - 일반", KUtils.GetThousandCommaText(killBossBonus));
                    totalScore += killBossBonus;
                }
            }

            this.totalScore.text = KUtils.GetThousandCommaText(totalScore);

            // 사운드 재생
            SoundManager.inst.StopBGM();
            if (isClear)
                SoundManager.inst.PlaySound(SoundKeys.EFFECT_VICTORY, PlayerCamera.current.transform, Vector3.zero);
            else
                SoundManager.inst.PlaySound(SoundKeys.EFFECT_GAMEOVER, PlayerCamera.current.transform, Vector3.zero);
            
        }

        public void OnDisable()
        {
            // UI가 꺼질때 GameUI에 UI가 꺼졌음을 알려줘야 한다.
            GameUI owner = GetComponentInParent<GameUI>();

            if (owner != null)
                owner.OnCloseChild();
        }

        /// <summary>
        /// 새로운 정보(Element) 생성
        /// </summary>
        public void CreateElement(string title, string value)
        {
            GameResultUI_Element element = ObjectPoolManager.inst.Get<GameResultUI_Element>(PrefabPath.UI.GameResultUI_Element);

            element.transform.SetParent(this.scrollRect.content, false);
            (element.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            element.transform.localRotation = Quaternion.identity;
            element.transform.localScale    = Vector3.one;

            element.Init(title, value);

            this.elements.Add(element);
        }

        /// <summary>
        /// Called By Button Event
        /// 마을로 이동하기
        /// </summary>
        public void OnGoHome()
        {
            this.gameObject.SetActive(false);
            StageMaster.current.ChangeScene("Town");
        }
    }
}

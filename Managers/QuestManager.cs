using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KShooting.SaveData;


namespace KShooting
{
    /// <summary>
    /// 퀘스트 관리자
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager inst { get; private set; }


        #region Get
        /// <summary>
        /// 진행중인 퀘스트
        /// </summary>
        public List<QuestProgressInfo> progessingQuest => UserData.quests;
        /// <summary>
        /// 퀵뷰에 등록되어 있는 퀘스트
        /// </summary>
        public List<string> quickViewQuests => UserData.quickViewQuest;
        #endregion Get


        #region Event Delegate
        /// <summary>
        /// 퀘스트 진행도가 변경됬을 때, 호출된다
        /// 첫번째 파라메터 : 변경된 퀘스트 키
        /// </summary>
        public event UnityAction<string> onProgressChanged;
        /// <summary>
        /// 퀘스트를 클리어 했을 때, 호출된다.
        /// 첫번째 파라메터 : 클리어된 퀘스트 키
        /// </summary>
        public event UnityAction<string> onQuestClear;
        /// <summary>
        /// 퀵뷰에 퀘스트가 등록되면 호출된다.
        /// 첫번째 파라메터 : 추가된 퀘스트 키
        /// </summary>
        public event UnityAction<string> onAddQuestQuickView;
        /// <summary>
        /// 퀵뷰에 퀘스트가 등록되면 호출된다.
        /// 첫번째 파라메터 : 삭제된 퀘스트 키
        /// </summary>
        public event UnityAction<string> onRemoveQuestQuickView;
        #endregion Event Delegate




        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Construct()
        {
            inst = new GameObject("Quest Manager").AddComponent<QuestManager>();
            DontDestroyOnLoad(inst);
        }

        /// <summary>
        /// 퀘스트를 추가한다.
        /// </summary>
        public void AddQuest(string questKey)
        {
            UserData.quests.Add(new QuestProgressInfo(questKey, new int[QuestDatabase.GetInfo(questKey).ifText.Length]));
            KLog.Log(string.Format("<color=red>{0} Add Quest</color>", questKey));
        }

        /// <summary>
        /// 퀘스트 진행도를 갱신한다.(Accum) 의미없는 키는 자동으로 무시된다.
        /// (코드 수정시 CheckQuest_Set()함수도 동일하게 수정할 것. 바꿀부분은 복붙하면 빨간줄 그어짐
        /// </summary>
        public void CheckQuest_Accum(string requestKey, int acc)
        {
            bool isDirty = false;


            // 플레이어가 받고 있는 퀘스트 중에
            for (int i = 0 ; i < UserData.quests.Count ; i++)
            {
                // 키에 맞는 퀘스트 정보를 불러온다.
                var questInfo = QuestDatabase.GetInfo(UserData.quests[i].key);

                // 퀘스트에 맞는 요구키가 있는지 찾는다.
                for (int j = 0 ; j < questInfo.requestKey.Length ; j++)
                {
                    // 요구키와 동일한 키가 있다면 퀘스트 진행도 업데이트
                    if (questInfo.requestKey[j] == requestKey)
                    {
                        isDirty = true;
                        UserData.quests[i].progress[j] += acc;
                        this.onProgressChanged?.Invoke(UserData.quests[i].key);

                        KLog.Log(string.Format("<color=red>{0} Check Quest</color>", requestKey));
                    }
                }
            }

            // 데이터가 갱신되었으면 업데이트
            if (isDirty)
                UserData.UpdateData();
        }

        /// <summary>
        /// 퀘스트 진행도를 갱신한다. 의미없는 키는 자동으로 무시된다.
        /// (코드 수정시 CheckQuest_Accum()함수도 동일하게 수정할 것. 바꿀부분은 복붙하면 빨간줄 그어짐
        /// </summary>
        public void CheckQuest_Set(string requestKey, int value)
        {
            bool isDirty = false;


            // 플레이어가 받고 있는 퀘스트 중에
            for(int i=0; i<UserData.quests.Count; i++)
            {
                // 키에 맞는 퀘스트 정보를 불러온다.
                var questInfo = QuestDatabase.GetInfo(UserData.quests[i].key);

                // 퀘스트에 맞는 요구키가 있는지 찾는다.
                for ( int j=0; j<questInfo.requestKey.Length; j++)
                {
                    // 요구키와 동일한 키가 있다면 퀘스트 진행도 업데이트
                    if(questInfo.requestKey[j] == requestKey)
                    {
                        isDirty = true;
                        UserData.quests[i].progress[j] = value;
                        this.onProgressChanged?.Invoke(UserData.quests[i].key);

                        KLog.Log(string.Format("<color=red>{0} Check Quest</color>", requestKey));
                    }
                }
            }

            // 데이터가 갱신되었으면 업데이트
            if(isDirty)
                UserData.UpdateData();
        }

        /// <summary>
        /// 현재 키에 맞는 진행도를 반환한다. 없으면 -1을 반환한다.
        /// </summary>
        public int GetProgress(string questKey, string requestKey)
        {
            // 플레이어가 받고 있는 퀘스트 중에
            for (int i = 0 ; i < UserData.quests.Count ; i++)
            {
                // 키에 맞는 퀘스트 정보를 불러온다.
                var questInfo = QuestDatabase.GetInfo(UserData.quests[i].key);

                // 퀘스트에 맞는 요구키가 있는지 찾는다.
                for (int j = 0 ; j < questInfo.requestKey.Length ; j++)
                {
                    // 요구키와 동일한 키가 있다면 진행도 반환
                    if (questInfo.requestKey[j] == requestKey)
                    {
                        return UserData.quests[i].progress[j];
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// 퀘스트를 클리어한다. 진행도 상관없이 클리어 하기 때문에
        /// 반드시 퀘스트 클리어 가능한지 여부를 확인할 것.
        /// </summary>
        /// <param name="questKey"></param>
        public void ClearQuest(string questKey)
        {
            // 현재 퀘스트 정보
            var currQuestInfo = QuestDatabase.GetInfo(questKey);

            // 아이템을 회수해야 하는경우 여기서 회수한다
            for(int i=0 ; i<currQuestInfo.requestKey.Length ; i++)
            {
                string chKey = currQuestInfo.requestKey[i];

                // 수집퀘스트키는 아이템을 회수
                if(chKey.Contains(QuestCheckKey.QuestKey_Collect_))
                {
                    UserManager.AccumItem(chKey.Substring(QuestCheckKey.QuestKey_Collect_.Length), -currQuestInfo.request[i]);
                }
            }

            // 관련 키를 언락한다.
            for (int i=0; i<currQuestInfo.unlockKey.Length; i++)
                UnlockManager.UnlockKey(currQuestInfo.unlockKey[i]);

            // 퀘스트 보상을 준다
            for(int i=0; i<currQuestInfo.rewardKey.Length; i++)
            {
                if (currQuestInfo.rewardKey[i] == "Coin")
                {
                    UserManager.AccumCoin(currQuestInfo.reward[i]);
                    StageMaster.current.playerController.hud.ViewEventText(string.Format("코인 획득 ({0})", currQuestInfo.reward[i]));
                }
                else
                {
                    UserManager.AccumItem(currQuestInfo.rewardKey[i], currQuestInfo.reward[i]);
                    StageMaster.current.playerController.hud.ViewEventText(string.Format("{0} 획득 ({1})", ItemDatabase.GetItemInfo(currQuestInfo.rewardKey[i]).itemName, currQuestInfo.reward[i]));
                }
            }

            // 퀘스트를 비운다.
            for(int i=0; i<UserData.quests.Count; i++)
            {
                if (UserData.quests[i].key == questKey)
                    UserData.quests.RemoveAt(i);
            }

            // 퀘스트 클리어됨
            UserData.clearedQuest.Add(questKey);

            // 퀵뷰에 데이터가 남아있으면 삭제
            if (HasQuickView(questKey))
                RemoveQuickView(questKey);

            // 데이터 최종 적용
            UserData.UpdateData();

            this.onQuestClear?.Invoke(questKey);
            KLog.Log(string.Format("<color=red>{0} Clear Quest</color>", questKey));
        }

        /// <summary>
        /// 퀘스트를 가지고 있는지
        /// </summary>
        public bool HasQuest(string questKey)
        {
            foreach(var q in UserData.quests)
            {
                if (q.key == questKey)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 퀘스트 조건을 모두 달성했는지 확인함.
        /// </summary>
        public bool IsFinishProgress(string questKey)
        {
            // 퀘스트 정보를 가져온다
            QuestInfo info = QuestDatabase.GetInfo(questKey);

            // 퀘스트 진행도를 가져온다
            QuestProgressInfo progress = null;
            for(int i=0; i<UserData.quests.Count; i++)
            {
                if(questKey == UserData.quests[i].key)
                {
                    progress = UserData.quests[i];
                    break;
                }
            }

            if(progress == null)
            {
                KLog.LogError("유효하지 않은 퀘스트키. " + questKey);
                return false;
            }



            // 퀘스트 조건을 모두 체크해서 진행도를 채웠는지 확인
            for(int i=0 ; i<info.request.Length ; i++)
            {
                // 여러 조건중 하나라도 만족하지 못하면 클리어 불가능
                if(info.request[i] > progress.progress[i])
                {
                    return false;
                }
            }

            // 모든 조건을 만족함
            return true;
        }

        /// <summary>
        /// 퀘스트를 받을 수 있는지
        /// </summary>
        /// <returns></returns>
        public bool IsAcceptQuest(string questKey)
        {
            //선행 조건 키를 받아와 언락을 시도함
            return UnlockManager.IsUnlock(QuestDatabase.GetInfo(questKey).preKey);
        }

        /// <summary>
        /// 퀘스트를 클리어했는지.(퀘스트를 이미 클리어 했다)
        /// </summary>
        public bool IsClearQuest(string questKey)
        {
            return UserData.clearedQuest.Contains(questKey);
        }

        #region Quick View
        /// <summary>
        /// 퀘스트를 퀵뷰에 등록한다
        /// </summary>
        public void AddQuickView(string questKey)
        {
            UserData.quickViewQuest.Add(questKey);
            this.onAddQuestQuickView?.Invoke(questKey);
        }

        /// <summary>
        /// 퀘스트를 퀵뷰에서 제거한다.
        /// </summary>
        public void RemoveQuickView(string questKey)
        {
            UserData.quickViewQuest.Remove(questKey);
            this.onRemoveQuestQuickView?.Invoke(questKey);
        }

        /// <summary>
        /// 퀵뷰에 퀘스트키가 존재하는지 확인한다.
        /// </summary>
        public bool HasQuickView(string questKey)
        {
            return UserData.quickViewQuest.Contains(questKey);
        }
        #endregion Quick View
    }
}
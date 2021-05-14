using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting.SaveData
{
    /// <summary>
    /// @@@@일반적인 코드에서 직접 접근 금지(Manager클래스 활용)@@@
    /// 
    /// 
    /// 유저 데이터 정보. 유저 고유의 정보들이 있음(스킬셋팅, 재화 등..)
    /// 여기에서 다루는 데이터는 대부분 게임이 꺼져도 가지고 있도록.. 해야 하지만 보류.
    /// 나중을 위해 UpdateData()함수는 만들어둠.
    /// </summary>
    public class UserData : MonoBehaviour
    {
        private static UserData inst;

        /// <summary>
        /// 사용중인 스킬정보
        /// </summary>
        public static List<string> useSkills = new List<string>();
        /// <summary>
        /// 진행중인 퀘스트 정보
        /// </summary>
        public static List<QuestProgressInfo> quests = new List<QuestProgressInfo>();
        /// <summary>
        /// 클리어된 퀘스트
        /// </summary>
        public static List<string> clearedQuest = new List<string>();
        /// <summary>
        /// 진행중인 퀘스트 정보(퀵뷰로 보고 있는것들)
        /// </summary>
        public static List<string> quickViewQuest = new List<string>();
        /// <summary>
        /// 게임 언락 데이터 정보
        /// </summary>
        public static HashSet<string> unlocks = new HashSet<string>();
        /// <summary>
        /// 가지고 있는 재화
        /// </summary>
        public static long coin = 1000;
        /// <summary>
        /// 클리어한 던전
        /// </summary>
        public static List<string> clearedDungeon = new List<string>();
        /// <summary>
        /// 가지고 있는 아이템들(사실 Dictionary는 정렬이 안되는 문제가 있긴 하지만...)
        /// </summary>
        public static Dictionary<string, int> items = new Dictionary<string, int>();




        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Construct()
        {
            inst = new GameObject("User Manager").AddComponent<UserData>();
            DontDestroyOnLoad(inst);

            // Load Data
            useSkills = new List<string>() { SkillKeys.SnipeShot, SkillKeys.BombShot, SkillKeys.PoisonShot };

            QuestManager.inst.AddQuest(QuestDatabase.info[0].questKey);
            QuestManager.inst.AddQuickView(QuestDatabase.info[0].questKey);

            items.Add(ItemKeys.HPPotion, 10);
            items.Add(ItemKeys.MPPotion, 10);
        }

        /// <summary>
        /// 데이터를 최종적으로 적용한다. 호출전엔 데이터가 저장되지 않는다.
        /// </summary>
        public static void UpdateData()
        {
            // 미구현
        }
    }
}
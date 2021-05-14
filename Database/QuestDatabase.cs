using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 퀘스트 관련 Database
    /// </summary>
    public static class QuestDatabase
    {
        public static QuestInfo[] info = new QuestInfo[] {
            /*
             * 메인 퀘스트
             */
            new QuestInfo(
                "MainQuest_Story1",
                new string[0],
                new string[] { "MainQuest_Story2" },
                "촌장에게 말걸기",
                "촌장에게 말을 걸어보자",
                new string[] { "촌장에게 말걸기" },
                new string[] { "QuestKey_Talk_Mayor" },
                new int[]    { 1 },
                new string[0],
                new int[0]),

            new QuestInfo(
                "MainQuest_Story2",
                new string[] { "MainQuest_Story2" },
                new string[] { "MainQuest_Story3" },
                "머쉬룸 사냥",
                "머쉬룸을 잡아라!",
                new string[] { "머쉬룸 레드 사냥", "머쉬룸 그린 사냥", "머쉬룸 블루 사냥" },
                new string[] { "QuestKey_Hunt_Mushroom_Red", "QuestKey_Hunt_Mushroom_Green", "QuestKey_Hunt_Mushroom_Blue" },
                new int[]    { 1, 1, 1 },
                new string[] { "HPPotion", "Coin" },
                new int[]    { 5, 1000 }),

            new QuestInfo(
                "MainQuest_Story3",
                new string[] { "MainQuest_Story3" },
                new string[] { "MainQuest_Story4" },
                "보스 거미 사냥",
                "보스 거미를 잡아라!",
                new string[] { "보스 거미 사냥" },
                new string[] { "QuestKey_Hunt_Metalon" },
                new int[]    { 1 },
                new string[] { "Coin" },
                new int[]    { 5000 }),

            new QuestInfo(
                "MainQuest_Story4", // 받을수 없는 퀘스트. 나중에 다음퀘스트를 받기 위해 잠금.
                new string[] { "MainQuest_Story3", "MainQuest_Lock" },
                new string[] { "MainQuest_Story5" },
                "잠금",
                "잠금",
                new string[] { "잠금" },
                new string[] { "QuestKey_Lock" },
                new int[]    { 9999 },
                new string[0],
                new int[0]),


            /*
             * 서브 퀘스트
             */
             new QuestInfo(
                 "SubQuest_Collect_Fruit1",
                 new string[] { "MainQuest_Story3" },
                 new string[0],
                 "열매 수집",
                 "열매 수집 하기",
                 new string[] { "열매수집" },
                 new string[] { "QuestKey_Collect_CuredBerries" },
                 new int[]    { 3 },
                 new string[] { "HPPotion", "MPPotion", "Coin" },
                 new int[]    { 10, 10, 1000 } ),
        };

        public static QuestInfo GetInfo(string key)
        {
            for(int i=0; i<info.Length; i++)
            {
                if (info[i].questKey == key)
                    return info[i];
            }

            throw new KeyNotFoundException("키를 찾지 못함. " + key);
        }
    }
}
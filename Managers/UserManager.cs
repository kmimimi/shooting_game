using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KShooting.SaveData;


namespace KShooting
{
    /// <summary>
    /// 유저와 관련된 데이터들을 처리해주는 매니저
    /// </summary>
    public static class UserManager
    {
        /// <summary>
        /// 사용중인 스킬들
        /// </summary>
        public static List<string> useSkills => UserData.useSkills;

        /// <summary>
        /// 가지고 있는아이템들
        /// </summary>
        public static Dictionary<string, int> items => UserData.items;

        /// <summary>
        /// 코인
        /// </summary>
        public static long coin => UserData.coin;


        public static void AccumCoin(long acc)
        {
            UserData.coin += acc;
        }

        /// <summary>
        /// 아이템을 누적한다.
        /// </summary>
        public static void AccumItem(string itemKey, int acc)
        {
            if(UserData.items.ContainsKey(itemKey))
            {
                UserData.items[itemKey] += acc;
            }
            else
            {
                UserData.items.Add(itemKey, acc);
            }


            // 아이템이 0개 이하라면 리스트에서 제거
            if (UserData.items[itemKey] <= 0)
                UserData.items.Remove(itemKey);
        }

        /// <summary>
        /// 아이템의 현재 개수를 반환한다.
        /// </summary>
        public static int GetItemCount(string itemKey)
        {
            if (UserData.items.ContainsKey(itemKey))
                return UserData.items[itemKey];
            else
                return 0;
        }

        /// <summary>
        /// 던전을 클리어 했다.
        /// </summary>
        /// <param name="dungeonKey"></param>
        public static void ClearDungeon(string dungeonKey)
        {
            if (!UserData.clearedDungeon.Contains(dungeonKey))
                UserData.clearedDungeon.Add(dungeonKey);

            KLog.Log("Clear Dungeon. " + dungeonKey);
        }

        /// <summary>
        /// 던전을 한번이상 클리어 했는지.
        /// </summary>
        public static bool IsClearDungeon(string dungeonKey)
        {
            return UserData.clearedDungeon.Contains(dungeonKey);
        }
    }
}
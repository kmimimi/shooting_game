using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 던전관련 Database
    /// </summary>
    public class DungeonDatabase : MonoBehaviour
    {
        public static DungeonInfo[] info = new DungeonInfo[] {
            new DungeonInfo("Dungeon_1_1",
                new string[] { "MainQuest_Story2" },
                new string[] { "Dungeon_1_2" },
                "던전 1",
                "머쉬룸들이 서식하고 있는 곳",
                false),
            new DungeonInfo("Dungeon_1_2",
                new string[] { "MainQuest_Story3" },
                new string[0],
                "던전 2",
                "거미들이 서식하고 있는 곳",
                true),
        };

        public static DungeonInfo GetInfo(string key)
        {
            for(int i=0; i<info.Length; i++)
            {
                if(info[i].dungeonKey == key)
                    return info[i];
            }

            throw new KeyNotFoundException("키를 찾지 못함. " + key);
        }
    }
}
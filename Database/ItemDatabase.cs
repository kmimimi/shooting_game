using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace KShooting
{
    public static class ItemDatabase
    {
        public static ItemInfo[] itemInfo = new ItemInfo[] {
            // Eat
            new ItemInfo("HPPotion", ItemKind.Potion, "HP포션", "체력을 50 회복한다", string.Empty, 50, 0),
            new ItemInfo("MPPotion", ItemKind.Potion, "MP포션", "마력을 10 회복한다.", string.Empty, 10, 0),

            // Quest
            new ItemInfo("CuredBerries", ItemKind.Quest, "치료열매", "HP포션의 재료로 사용된다.(퀘스트)", string.Empty, 0, 0)
        };

        public static StoreInfo[] storeInfo = new StoreInfo[] {
            new StoreInfo("HPPotion", 100),
            new StoreInfo("MPPotion", 100)
        };

        public static ItemInfo GetItemInfo(string key)
        {
            for(int i=0; i<itemInfo.Length; i++)
            {
                if (itemInfo[i].itemKey == key)
                    return itemInfo[i];
            }

            throw new KeyNotFoundException("키가 없음." + key);
        }

        public static StoreInfo GetStoreInfo(string key)
        {
            for(int i=0; i<storeInfo.Length; i++)
            {
                if (storeInfo[i].itemKey == key)
                    return storeInfo[i];
            }

            throw new KeyNotFoundException("키가 없음." + key);
        }
    }
}
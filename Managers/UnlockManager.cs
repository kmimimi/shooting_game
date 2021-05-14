using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KShooting.SaveData;


namespace KShooting
{
    /// <summary>
    /// 언락 관리자
    /// </summary>
    public static class UnlockManager
    {
        /// <summary>
        /// 키를 언락한다. 중복으로 넣어도 알아서 처리됨
        /// </summary>
        public static void UnlockKey(string key)
        {
            if (!UserData.unlocks.Contains(key))
            {
                UserData.unlocks.Add(key);
                UserData.UpdateData();

                KLog.LogWarning(string.Format("[{0}] Unlock Key", key));
            }
        }

        /// <summary>
        /// 키가 언락되어있는지 확인한다.
        /// </summary>
        public static bool IsUnlock(params string[] keys)
        {
            for(int i=0; i<keys.Length; i++)
            {
                if (!UserData.unlocks.Contains(keys[i]))
                    return false;
            }

            return true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 계산관련 처리는 모두 여기를 통해 진행한다.
    /// </summary>
    public static class Calculator
    {
        /// <summary>
        /// 최종대미지 값을 랜덤으로 바꿔준다
        /// </summary>
        public static float RandomDamage(float damage)
        {
            return damage + Random.Range(-damage * Defines.DamageRange, damage * Defines.DamageRange);
        }
    }
}
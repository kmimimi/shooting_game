using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 모든무기의 베이스 클래스
    /// </summary>
    public abstract class Weapon : MonoBehaviour
    {
        public Unit owner { get; private set; }

        public virtual void Init(Unit owner)
        {
            this.owner = owner;
        }
    }
}
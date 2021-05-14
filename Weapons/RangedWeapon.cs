using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 원거리 무기
    /// Projectile을 발사할 수 있다.
    /// </summary>
    public abstract class RangedWeapon : Weapon
    {
        [Tooltip("발사할수 있는 피벗")]
        [SerializeField] private Transform _firePivot = null;
        [Tooltip("발사체 파워")]
        [SerializeField] private float _power = 3;


        /// <summary>
        /// Projectile이 발사되는 지점
        /// </summary>
        public virtual Vector3 firePivot => this._firePivot.position;
        public float           power     => this._power;

        /// <summary>
        /// 발사방향.
        /// </summary>
        public Vector3 fireDirection { get; set; }
    }
}

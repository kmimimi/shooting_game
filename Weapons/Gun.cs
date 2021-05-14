using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 총. 
    /// </summary>
    public class Gun : RangedWeapon
    {
        /// <summary>
        /// TODO: 적절하게 처리할 수 있는 애니메이션이 아니기 떄문에 위치는 그냥 하드코딩함
        /// </summary>
        public override Vector3 firePivot => this.owner.transform.position + new Vector3(0, 0.7f, 0) + this.owner.transform.TransformDirection(0.1f, 0, 0.52f);
    }
}
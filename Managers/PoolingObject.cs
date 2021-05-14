using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 풀링된 오브젝트는 이 컴포넌트가 붙어 있다.
    /// </summary>
    public sealed class PoolingObject : MonoBehaviour
    {
        /// <summary>
        /// 부모 정보
        /// </summary>
        public string path;

        /// <summary>
        /// 씬이 전환될 때, 자동으로 회수를 시도한다.
        /// </summary>
        public bool autoCollection;
    }
}
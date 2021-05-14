using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KShooting
{
    /// <summary>
    /// 공격범위 표시하는 프로젝터
    /// </summary>
    public class AOEProjector : MonoBehaviour
    {
        [SerializeField] private Projector projector = null;



        public void Init(float radius)
        {
            projector.orthographicSize = radius;
        }
    }
}

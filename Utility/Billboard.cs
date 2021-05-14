using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 카메라 빌보딩
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        private void LateUpdate()
        {
            this.transform.LookAt(PlayerCamera.current.camPivot);
        }
    }
}
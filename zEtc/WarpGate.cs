using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KShooting.UI;


namespace KShooting
{
    /// <summary>
    /// 워프 게이트
    /// </summary>
    public class WarpGate : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            GameUI.inst.ViewDungeonUI();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(this.transform.position, GetComponent<SphereCollider>().radius);
        }
    }
}
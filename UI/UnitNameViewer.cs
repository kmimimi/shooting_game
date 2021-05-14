using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace KShooting
{
    /// <summary>
    /// 이름표시해주는 UI
    /// </summary>
    public class UnitNameViewer : MonoBehaviour
    {
        [SerializeField] private TextMesh nameText = null;

        public void Init(string text)
        {
            this.nameText.text = text;
        }
    }
}
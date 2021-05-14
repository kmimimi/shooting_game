using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KShooting.UI
{
    /// <summary>
    /// 선택지 버튼
    /// </summary>
    public class SelectionUI_Element : MonoBehaviour
    {
        [SerializeField] private TMP_Text text = null;

        private SelectionUI owner;



        public void Init(SelectionUI owner, SelectionData data)
        {
            this.owner     = owner;
            this.text.text = data.text;
        }

        /// <summary>
        /// Called By Button Click Event
        /// </summary>
        public void OnClick()
        {
            SoundManager.inst.PlaySound(SoundKeys.EFFECT_DIALOG, PlayerCamera.current.cam.transform.position);
            this.owner.ClickElement(this.transform.GetSiblingIndex());
        }
    }
}

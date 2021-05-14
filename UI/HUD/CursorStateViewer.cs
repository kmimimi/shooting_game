using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KShooting
{
    /// <summary>
    /// 커서의 현재상태를 보여준다
    /// </summary>
    public class CursorStateViewer : MonoBehaviour
    {
        [SerializeField] private TMP_Text stateText = null;
        private CursorLockMode lastMode;


        private void Start()
        {
            this.lastMode = Cursor.lockState;
            this.stateText.text = "Cursor:" + this.lastMode;
        }

        void Update()
        {
            if(this.lastMode != Cursor.lockState)
            {
                this.lastMode = Cursor.lockState;
                this.stateText.text = "Cursor:" + this.lastMode;
            }
        }
    }
}
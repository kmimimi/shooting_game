using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KShooting.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GameEventViewer_TextElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text text = null;
        [SerializeField] private CanvasGroup canvasGroup = null;

        private float lifeTime;


        public void Init(string text)
        {
            this.lifeTime = 3;
            this.text.text = text;
            this.canvasGroup.alpha = 1;
        }

        private void Update()
        {
            this.canvasGroup.alpha = Mathf.Clamp01(this.lifeTime);


            this.lifeTime -= Time.deltaTime;

            if(this.lifeTime <= 0)
                ObjectPoolManager.inst.Return(this.gameObject);
        }

        private void Reset()
        {
            this.canvasGroup = GetComponent<CanvasGroup>();
        }
    }
}
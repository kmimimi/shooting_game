using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace KShooting
{
    /// <summary>
    /// Sprite를 간편하게 교체해주는 클래스
    /// </summary>
    public class SpriteChanger : MonoBehaviour
    {
        public Sprite[] sprites = new Sprite[0];

        private Image          image;
        private SpriteRenderer spriteRenderer;



        private void Awake()
        {
            this.image          = GetComponent<Image>();
            this.spriteRenderer = GetComponent<SpriteRenderer>();
            Change(0);
        }

        public void Change(int index)
        {
            if (this.image != null)
                this.image.sprite = this.sprites[index];
            else if (this.spriteRenderer != null)
                this.spriteRenderer.sprite = this.sprites[index];
        }
    }
}
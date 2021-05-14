using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace KShooting.UI
{
    /// <summary>
    /// 스킬 상태(사용여부, 쿨타임)를 표시해주는 UI Element
    /// </summary>
    public class ItemViewer_IconElement : MonoBehaviour
    {
        [SerializeField] private Image    _iconImage = null;
        [SerializeField] private TMP_Text _countText = null;
        [SerializeField] private TMP_Text _hotKey   = null;

        private string key;
        private int lastCount;


        public void Init(string itemKey, string hotKey)
        {
            this.key = itemKey;
            this._iconImage.overrideSprite = SpriteManager.Get(SpritePath.Item_Path + itemKey);
            this._hotKey.text = hotKey;

            this.lastCount = UserManager.GetItemCount(this.key);
            this._countText.text = this.lastCount.ToString();
        }

        private void Update()
        {
            int currItemCount = UserManager.GetItemCount(this.key);

            if(this.lastCount != currItemCount)
            {
                this.lastCount = currItemCount;
                this._countText.text = this.lastCount.ToString();
            }
        }
    }
}
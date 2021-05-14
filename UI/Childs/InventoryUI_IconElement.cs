using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


namespace KShooting.UI
{
    /// <summary>
    /// 인벤토리UI에서 사용하는 아이콘
    /// </summary>
    public class InventoryUI_IconElement : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image    image = null;
        [SerializeField] private TMP_Text count = null;

        public  ItemInfo info { get; private set; }
        public  Sprite icon => this.image.overrideSprite;
        private InventoryUI owner;


        public void Init(InventoryUI owner, string key, int count)
        {
            this.owner = owner;
            this.info = ItemDatabase.GetItemInfo(key);

            this.image.overrideSprite = SpriteManager.Get(SpritePath.Item_Path + key);
            SetCount(count);
        }

        public void SetCount(int count)
        {
            this.count.text = count.ToString();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                this.owner.ClickElement(this);
            }
        }
    }
}
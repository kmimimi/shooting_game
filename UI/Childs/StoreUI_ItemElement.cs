using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace KShooting.UI
{
    public class StoreUI_ItemElement : MonoBehaviour
    {
        public const int MAX_ITEM_COUNT = 99;

        [SerializeField] private Image iconImage = null;
        [SerializeField] private TMP_Text nameText = null;

        [SerializeField] private TMP_Text priceText = null;
        [SerializeField] private TMP_Text countText = null;
        [SerializeField] private Button priceButton = null;
        [SerializeField] private Button upButton = null;
        [SerializeField] private Button downButton = null;


        private StoreUI owner;
        public StoreInfo storeInfo { get; private set; }
        public int count           { get; private set; }



        public void Init(StoreUI owner, StoreInfo storeInfo)
        {
            this.owner = owner;
            this.storeInfo = storeInfo;
            ItemInfo itemInfo = ItemDatabase.GetItemInfo(this.storeInfo.itemKey);

            // UI 셋팅
            this.iconImage.overrideSprite = SpriteManager.Get(SpritePath.Item_Path + this.storeInfo.itemKey);
            this.nameText.text = itemInfo.itemName;


            // 가격표 갱신
            this.count = 1;
            UpdateData();
        }

        public void OnPriceButtonClick()
        {
            // 구매확인 UI필요
            this.owner.BuyItem(this);
            this.count = 1;
            UpdateData();
        }

        public void OnUpButtonClick()
        {
            SoundManager.inst.PlaySound(SoundKeys.EFFECT_DIALOG, PlayerCamera.current.cam.transform.position, 0.7f);
            this.count = Mathf.Clamp(this.count + 1, 1, MAX_ITEM_COUNT);
            UpdateData();
        }

        public void OnDownButtonClick()
        {
            SoundManager.inst.PlaySound(SoundKeys.EFFECT_DIALOG, PlayerCamera.current.cam.transform.position, 0.7f);
            this.count = Mathf.Clamp(this.count - 1, 1, MAX_ITEM_COUNT);
            UpdateData();
        }

        public void UpdateData()
        {
            // 버튼들 활성화 비활성화 여부처리
            this.upButton.interactable    = this.count < MAX_ITEM_COUNT;
            this.downButton.interactable  = this.count > 1;
            this.priceButton.interactable = UserManager.coin >= this.storeInfo.price * count;

            this.countText.text = this.count.ToString();
            this.priceText.text = "$ " + KUtils.GetThousandCommaText(this.storeInfo.price * this.count);
        }
    }
}
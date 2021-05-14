using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace KShooting.UI
{
    /// <summary>
    /// 인벤토리 아이템 누르면 나오는 정보
    /// </summary>
    public class InventoryDetailView : MonoBehaviour
    {
        [SerializeField] private Image    iconImage   = null;
        [SerializeField] private TMP_Text titleText   = null;
        [SerializeField] private TMP_Text explainText = null;

        [SerializeField] private Button useButton = null;

        private ItemInfo info;
        private InventoryUI owner;


        public void Init(InventoryUI owner, ItemInfo info, Sprite sprite)
        {
            this.owner                    = owner;
            this.info                     = info;
            this.iconImage.overrideSprite = sprite;
            this.titleText.text           = info.itemName;
            this.explainText.text         = info.explain;

            // 포션 아이템만 사용가능
            this.useButton.gameObject.SetActive(info.itemKind == ItemKind.Potion);
        }

        public void OnUseButtonClick()
        {
            // 플레이어 유닛이 유효할 때,
            if(StageMaster.current.player != null)
            {
                // 아이템 사용을 시도한다
                if (StageMaster.current.playerController.TryUseItem(this.info.itemKey, null))
                {
                    // UI갱신
                    this.owner.RefreshElement(this.info.itemKey);
                }
            }

            this.gameObject.SetActive(false);
        }

        public void OnCancelButtonClick()
        {
            this.gameObject.SetActive(false);
        }
    }
}
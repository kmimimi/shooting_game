using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace KShooting.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private ScrollRect          scrollRect = null;
        [SerializeField] private TMP_Text            coinText   = null;
        [SerializeField] private InventoryDetailView detailView = null;

        private List<InventoryUI_IconElement> elements = new List<InventoryUI_IconElement>();



        public void Init()
        {
            // 기존 element들 삭제
            for(int i=0; i<this.elements.Count; i++)
            {
                ObjectPoolManager.inst.Return(this.elements[i].gameObject);
            }

            this.detailView.gameObject.SetActive(false);
        
            // 아이템 데이터 업데이트
            foreach(var i in UserManager.items)
            {
                // 아이템 없는건 무시.
                if (i.Value <= 0)
                    continue;

                var element = ObjectPoolManager.inst.Get<InventoryUI_IconElement>(PrefabPath.UI.InventoryUI_IconElement);

                element.transform.SetParent(this.scrollRect.content, false);
                (element.transform as RectTransform).anchoredPosition3D = Vector3.zero;
                element.transform.localRotation = Quaternion.identity;
                element.transform.localScale = Vector3.one;

                element.Init(this, i.Key, i.Value);
                this.elements.Add(element);
            }

            // 코인 데이터 업데이트
            UpdateCoinText();
        }

        private void Awake()
        {
            (this.detailView.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            this.detailView.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            // UI가 꺼질때 GameUI에 UI가 꺼졌음을 알려줘야 한다.
            GameUI owner = GetComponentInParent<GameUI>();

            if (owner != null)
                owner.OnCloseChild();
        }

        /// <summary>
        /// Called By Button Event
        /// </summary>
        public void OnCloseButtonClick()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// 코인량을 업데이트
        /// </summary>
        private void UpdateCoinText()
        {
            this.coinText.text = KUtils.GetThousandCommaText(UserManager.coin);
        }

        /// <summary>
        /// 아이템중 하나를 선택하면 세부정보창이 뜬다.
        /// </summary>
        public void ClickElement(InventoryUI_IconElement element)
        {
            this.detailView.gameObject.SetActive(true);
            this.detailView.Init(this, element.info, element.icon);
        }

        /// <summary>
        /// 해당 엘리먼트의 개수를 갱신한다
        /// </summary>
        public void RefreshElement(string itemKey)
        {
            int count = UserManager.GetItemCount(itemKey);

            for(int i=0; i<this.elements.Count; i++)
            {
                if (this.elements[i].info.itemKey == itemKey)
                {
                    if (count <= 0)
                    {
                        ObjectPoolManager.inst.Return(this.elements[i].gameObject);
                        this.elements.RemoveAt(i);
                        break;
                    }
                    else
                    {
                        this.elements[i].SetCount(count);
                    }
                }
            }
        }
    }
}

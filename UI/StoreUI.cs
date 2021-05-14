using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;


namespace KShooting.UI
{
    public class StoreUI : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect = null;
        [SerializeField] private TMP_Text coinText = null;

        private StoreUI_ItemElement[] elements = new StoreUI_ItemElement[0];
        private UnityAction onBuy;
        private UnityAction onComplete;


        /// <summary>
        /// 상점 페이지를 연다.
        /// </summary>
        /// <param name="onBuy">상점에서 아이템을 구매했을 때 나오는 이벤트</param>
        /// <param name="onComplete">상점을 종료했을 때 나오는 이벤트
        public void Init(UnityAction onBuy, UnityAction onComplete)
        {
            // 기존 Element들 제거
            for (int i = 0 ; i < this.elements.Length ; i++)
                ObjectPoolManager.inst.Return(this.elements[i].gameObject);

            // 새 요소들 추가
            this.elements = new StoreUI_ItemElement[ItemDatabase.storeInfo.Length];
            for(int i=0; i<ItemDatabase.storeInfo.Length; i++)
            {
                this.elements[i] = ObjectPoolManager.inst.Get<StoreUI_ItemElement>(PrefabPath.UI.StoreUI_ItemElement);

                this.elements[i].transform.SetParent(this.scrollRect.content, false);
                (this.elements[i].transform as RectTransform).anchoredPosition3D = Vector3.zero;
                this.elements[i].transform.localRotation = Quaternion.identity;
                this.elements[i].transform.localScale = Vector3.one;

                this.elements[i].Init(this, ItemDatabase.storeInfo[i]);
            }

            // 코인 데이터 업데이트
            UpdateCoinText();

            //이벤트 등록
            this.onBuy      = onBuy;
            this.onComplete = onComplete;
        }

        private void OnDisable()
        {
            // UI가 꺼질때 GameUI에 UI가 꺼졌음을 알려줘야 한다.
            GameUI owner = GetComponentInParent<GameUI>();

            if (owner != null)
                owner.OnCloseChild();
        }

        public void OnClose()
        {
            SoundManager.inst.PlaySound(SoundKeys.EFFECT_DIALOG, PlayerCamera.current.cam.transform.position, 0.7f);
            this.onComplete?.Invoke();
            this.gameObject.SetActive(false);
        }

        public void BuyItem(StoreUI_ItemElement element)
        {
            // 구매전 다시 확인
            if(UserManager.coin >= element.storeInfo.price * element.count)
            {
                UserManager.AccumItem(element.storeInfo.itemKey, element.count);
                UserManager.AccumCoin(-element.storeInfo.price * element.count);


                // UI업데이트
                UpdateCoinText();
                for ( int i=0; i<this.elements.Length; i++)
                {
                    this.elements[i].UpdateData();
                }

                this.onBuy?.Invoke();
            }
        }
        /// <summary>
        /// 코인량을 업데이트
        /// </summary>
        private void UpdateCoinText()
        {
            this.coinText.text = KUtils.GetThousandCommaText(UserManager.coin);
        }
    }
}
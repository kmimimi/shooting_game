using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace KShooting.UI
{
    /// <summary>
    /// 선택지 UI. 대화 선택할때 사용
    /// </summary>
    public class SelectionUI : MonoBehaviour
    {
        [SerializeField] private RectTransform root = null;

        private SelectionUI_Element[] elements;
        private UnityAction<int> onComplete;



        private void Awake()
        {
            if (this.root == null)
                KLog.Log("root is null", this);

            // 선택지로 사용할 버튼들 캐싱 + 비활성화
            this.elements = this.root.GetComponentsInChildren<SelectionUI_Element>();
            InactiveAllElement();
        }

        private void OnDisable()
        {
            // UI가 꺼질때 GameUI에 UI가 꺼졌음을 알려줘야 한다.
            GameUI owner = GetComponentInParent<GameUI>();

            if (owner != null)
                owner.OnCloseChild();
        }

        /// <summary>
        /// 선택지는 최대 4개까지 지원한다.
        /// </summary>
        public void Init(SelectionData[] list, UnityAction<int> onComplete)
        {
            if (list.Length >= 4)
                KLog.Log("선택지는 최대 4개까지 지원함");

            InactiveAllElement();
            for (int i = 0; i<list.Length; i++)
            {
                this.elements[i].gameObject.SetActive(true);
                this.elements[i].Init(this, list[i]);
            }

            this.onComplete = onComplete;
        }

        /// <summary>
        /// Called by Element.
        /// Element의 버튼이 선택됬을 때 호출된다.
        /// </summary>
        public void ClickElement(int index)
        {
            this.onComplete?.Invoke(index);
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Element들을 모두 비활성화 한다.
        /// </summary>
        private void InactiveAllElement()
        {
            for (int i = 0 ; i < this.elements.Length ; i++)
                this.elements[i].gameObject.SetActive(false);
        }
    }
}

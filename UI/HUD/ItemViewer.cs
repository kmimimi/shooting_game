using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting.UI
{
    /// <summary>
    /// 플레이어의 아이템 정보 표시
    /// 일단 간단하게 해둠
    /// </summary>
    public sealed class ItemViewer : MonoBehaviour
    {
        [SerializeField] private RectTransform _layout = null;

        private ItemViewer_IconElement[] elements = new ItemViewer_IconElement[0];



        public void Init(string[] itemKeys, string[] hotKeys)
        {
            // 기존에 사용하고 있던 element제거
            for (int i = 0 ; i < this.elements.Length ; i++)
                ObjectPoolManager.inst.Return(this.elements[i].gameObject);


            // 새로운 element생성
            this.elements = new ItemViewer_IconElement[itemKeys.Length];
            for (int i = 0 ; i < itemKeys.Length ; i++)
            {
                this.elements[i] = ObjectPoolManager.inst.New<ItemViewer_IconElement>(PrefabPath.UI.ItemViewer_IconElement);

                this.elements[i].transform.SetParent(this._layout);
                (this.elements[i].transform as RectTransform).anchoredPosition3D = Vector3.zero;
                this.elements[i].transform.localRotation = Quaternion.identity;
                this.elements[i].transform.localScale = Vector3.one;

                this.elements[i].Init(itemKeys[i], hotKeys[i]);
            }
        }
    }
}

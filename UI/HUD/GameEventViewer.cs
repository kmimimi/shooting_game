using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace KShooting.UI
{
    /// <summary>
    /// 게임 이벤트를 표시해줌
    /// </summary>
    public class GameEventViewer : MonoBehaviour
    {
        [SerializeField] private VerticalLayoutGroup layout = null;

        bool rebuild;

        public void Init()
        {

        }

        public void View(string text)
        {
            var element = ObjectPoolManager.inst.Get<GameEventViewer_TextElement>(PrefabPath.UI.GameEventViewer_TextElement);

            element.transform.SetParent(this.layout.transform, false);
            (element.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            element.transform.localRotation = Quaternion.identity;
            element.transform.localScale = Vector3.one;

            element.Init(text);
        }
    }
}
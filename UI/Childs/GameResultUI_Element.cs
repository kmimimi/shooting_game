using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KShooting.UI
{
    /// <summary>
    /// 게임 결과에서 각각의 요소들을 표시할 떄 사용
    /// </summary>
    public class GameResultUI_Element : MonoBehaviour
    {
        [SerializeField] private TMP_Text title = null;
        [SerializeField] private TMP_Text value = null;



        public void Init(string title, string value)
        {
            this.title.text = title;
            this.value.text = value;
        }
    }
}
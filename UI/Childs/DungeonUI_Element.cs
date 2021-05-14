using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace KShooting.UI
{
    /// <summary>
    /// 던전 UI에서 표시되는 던전정보(Element)
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class DungeonUI_Element : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText = null;
        [SerializeField] private Graphic bossIcon = null;
        [SerializeField] private Graphic newText = null;


        /// <summary>
        /// Dungeon Key
        /// </summary>
        public string key { get; private set; }

        private DungeonUI owner;


        public void Init(DungeonUI owner, string key, string title)
        {
            this.owner = owner;

            this.key = key;
            this.titleText.text = title;
            this.bossIcon.enabled = DungeonDatabase.GetInfo(key).isBoss;
            this.newText.enabled = !UserManager.IsClearDungeon(key);
        }

        /// <summary>
        /// Called By Button Event
        /// </summary>
        public void OnClick()
        {
            this.owner.ClickElement(key);
        }
    }
}
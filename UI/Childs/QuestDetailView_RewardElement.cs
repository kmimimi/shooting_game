using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace KShooting.UI
{
    public class QuestDetailView_RewardElement : MonoBehaviour
    {
        [SerializeField] private Image    iconImage = null;
        [SerializeField] private TMP_Text rewardText = null;


        public void Init(string itemKey, int reward)
        {
            this.iconImage.overrideSprite = SpriteManager.Get(SpritePath.Item_Path + itemKey);
            this.rewardText.text          = KUtils.GetThousandCommaText(reward);
        }
    }
}
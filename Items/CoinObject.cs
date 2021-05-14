using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KShooting
{
    /// <summary>
    /// 코인. 먹으면 돈을 얻는다.
    /// </summary>
    public class CoinObject : Item
    {
        [SerializeField] private Transform modelRoot = null;

        private int value;


        public void Init(int value)
        {
            this.value = value;
        }

        private void Update()
        {
            this.modelRoot.Rotate(new Vector3(0, 360 * Time.deltaTime, 0));
        }

        public override void Eat()
        {
            SoundManager.inst.PlaySound(SoundKeys.EFFECT_COIN, this.transform.position);
            StageMaster.current.playerController.hud.ViewEventText(string.Format("코인 획득 ({0})", value));
            UserManager.AccumCoin(value);
            value = 0;
        }
    }
}

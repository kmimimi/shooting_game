using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KShooting
{
    /// <summary>
    /// 단순한 유닛용 AI. 목표에게 다가가고 일반공격만 시도한다.
    /// </summary>
    public class SimpleAIController : AIController
    {
        public override void OnLiveUpdate()
        {
            base.OnLiveUpdate();

            // 타겟이 있을경우
            if (target != null)
            {
                if (MoveTargetUpdate(this.target.transform.position, this.unit.attackRange))
                {
                    if (this.unit.skillModule.defaultAttack.Usable())
                        this.unit.skillModule.defaultAttack.Action();
                }
            }
            else // 타겟이 없을경우
            {
                TrySearchTarget();
            }
        }
    }
}
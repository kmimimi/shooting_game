using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KShooting
{
    /// <summary>
    /// 몬스터 소탕 스테이지.
    /// TODO: 추가적으로 나오는 적유닛들이 있다면 처리를 어떻게 해야 할지 모르니
    /// 일단 나와있는 유닛만 기준으로 처리함
    /// </summary>
    public class MonsterCleaningStage : StageMaster
    {
        public int totalEnemyCount  { get; private set; }
        public int remainEnemyCount { get; private set; }


        protected override void InitGame()
        {
            base.InitGame();

            // 게임을 시작하는 순간 살아 있는 유닛들을 모두 가져와 목표치를 수정한다.
            for(int i=0; i<UnitManager.units.Count ; i++)
            {
                if(UnitManager.units[i].unitSide == UnitSide.Enemy)
                    this.totalEnemyCount++;
            }

            this.remainEnemyCount = totalEnemyCount;

            // 유닛이 죽을때마다 이벤트를 받을수 있게
            UnitManager.onRemovedUnit += OnRemovedUnit;

            // 게임허드에 필요한 UI활성화
            this.playerController.hud.LoadMonsterRemainCountHud(this);
        }

        protected override void EndGame()
        {
            base.EndGame();
            // 이벤트 할당 해제
            UnitManager.onRemovedUnit -= OnRemovedUnit;
        }

        private void OnRemovedUnit(Unit unit)
        {
            // 적군일경우, 
            if (unit.unitSide == UnitSide.Enemy)
                this.remainEnemyCount--;
        }

        protected override bool CheckVictory()
        {
            // 남은 적 유닛의 숫자가 0이면 승리한다.
            //return base.CheckVictory();
            return this.remainEnemyCount <= 0;
        }
    }
}
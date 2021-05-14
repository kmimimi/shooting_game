using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace KShooting
{
    /// <summary>
    /// 타임어택 스테이지
    /// 
    /// TODO: 타임어택이 종류가 여러가지 이긴 한데...
    /// 일단 보스를 잡으면 게임 끝나는 스테이지로만 한정해둠.
    /// </summary>
    public class TimeAttackStage : StageMaster
    {
        [SerializeField] private float _timeLimit = 300;
        [SerializeField] private Unit  _boss = null;

        private bool huntBoss = false;
        public float timeLimit => this._timeLimit;



        protected override void StartGame()
        {
            base.StartGame();
            // 유닛이 죽을때마다 이벤트를 받을수 있게
            UnitManager.onRemovedUnit += OnRemovedUnit;

            // 게임허드에 필요한 UI활성화
            this.playerController.hud.LoadTimeAttackHud(this);
        }

        protected override void EndGame()
        {
            base.EndGame();

            // 이벤트 사용이 끝나면 제거
            UnitManager.onRemovedUnit += OnRemovedUnit;
        }

        /// <summary>
        /// 남은시간(Ticks)
        /// </summary>
        /// <returns></returns>
        public long GetRemainTicks()
        {
            long limitTicks = this.scoreInfo.startTicks + (long)(this._timeLimit * TimeSpan.TicksPerSecond);

            return limitTicks - DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// 보스를 사냥할 경우 성공한다.
        /// </summary>
        protected override bool CheckVictory()
        {
            return this.huntBoss;
            //return base.CheckVictory();
        }

        /// <summary>
        /// 시간초과 하면 게임 오버
        /// </summary>
        /// <returns></returns>
        protected override bool CheckGameOver()
        {
            return GetRemainTicks() <= 0;
        }

        private void OnRemovedUnit(Unit unit)
        {
            // 보스를 죽였다.
            if (unit == this._boss)
                this.huntBoss = true;
        }
    }
}
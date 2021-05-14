using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace KShooting
{
    public static class UnitManager
    {
        /// <summary>
        /// 메모리 할당없이 리스트 반환용
        /// </summary>
        private static List<Unit> _alockMemory = new List<Unit>(100);

        /// <summary>
        /// 모든 씬에 배치되어 있는 유닛들
        /// </summary>
        public static List<Unit> units { get; private set; } = new List<Unit>();

        /// <summary>
        /// 유닛이 새로 추가될때 호출된다
        /// </summary>
        public static event UnityAction<Unit> onAddedUnit;
        /// <summary>
        /// 유닛이 제거될 때 호출된다.
        /// </summary>
        public static event UnityAction<Unit> onRemovedUnit;


        /// <summary>
        /// 유닛을 등록한다
        /// </summary>
        public static void AddUnit(Unit unit)
        {
            units.Add(unit);
            onAddedUnit?.Invoke(unit);
        }

        /// <summary>
        /// 유닛을 제거한다.
        /// </summary>
        /// <param name="unit"></param>
        public static void RemoveUnit(Unit unit)
        {
            units.Remove(unit);
            onRemovedUnit?.Invoke(unit);
        }

        /// <summary>
        /// 특정 포지션으로부터 일정 범위내에 있고, 살아있는 유닛들을 찾아준다.
        /// 제일 가까운 유닛이 0번째 인덱스
        /// </summary>
        public static List<Unit> GetNearUnitsNonAlloc(Vector3 position, float range)
        {
            _alockMemory.Clear();

            for(int i=0; i<units.Count; i++)
            {
                // 살아있는 유닛이 아니면 무시
                if (units[i].unitState != UnitState.Live)
                    continue;

                // range안으로 들어온 유닛들만 추가
                if ((position - units[i].transform.position).sqrMagnitude <= range * range)
                {
                    _alockMemory.Add(units[i]);
                }
            }

            // 오름차순 정렬
            _alockMemory.Sort((a, b) => (position - a.transform.position).sqrMagnitude.CompareTo((position - b.transform.position).sqrMagnitude));
            return _alockMemory;
        }
        /// <summary>
        /// 특정 포지션으로부터 일정 범위내에 있고, 살아있는 유닛들을 찾아준다.
        /// 제일 가까운 유닛이 0번째 인덱스
        /// </summary>
        public static List<Unit> GetNearUnitsNonAlloc(Vector3 position, float range, UnitSide side)
        {
            _alockMemory.Clear();

            for (int i = 0 ; i < units.Count ; i++)
            {
                // 살아있는 유닛이 아니면 무시
                if (units[i].unitState != UnitState.Live)
                    continue;

                // range안으로 들어온 유닛들만 추가
                if ((position - units[i].transform.position).sqrMagnitude <= range * range)
                {
                    if(units[i].unitSide == side)
                        _alockMemory.Add(units[i]);
                }
            }

            // 오름차순 정렬
            _alockMemory.Sort((a, b) => (position - a.transform.position).sqrMagnitude.CompareTo((position - b.transform.position).sqrMagnitude));
            return _alockMemory;
        }
    }
}
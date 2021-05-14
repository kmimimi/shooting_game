using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace KShooting.UI
{
    /// <summary>
    /// 플레이어용 허드
    /// </summary>
    public class GameHud : MonoBehaviour
    {
        [SerializeField] private RectTransform  _rootLayout = null;
        [SerializeField] private UnitStatusBar  _statusBar = null;
        [SerializeField] private SkillViewer    _skillViewer = null;
        [SerializeField] private ItemViewer     _itemViewer = null;
        [SerializeField] private Image          _crosshair = null;
        [SerializeField] private QuestQuickView _questQuickView = null;
        [SerializeField] private BossStatusBar  _bossStatusBar = null;
        [SerializeField] private GameEventViewer _eventViewer = null;



        public void Init(Unit player)
        {
            this._statusBar.Init(player);
            this._skillViewer.Init(player);
            this._itemViewer.Init(
                new string[] { ItemKeys.HPPotion, ItemKeys.MPPotion },
                new string[] { InputManager.useHPPotionKey.ToString(), InputManager.useMPPotionKey.ToString() });
            this._eventViewer.Init();
        }

        private void Awake()
        {
            // 에러체크
            if (this._rootLayout == null)
                KLog.LogError("RootLayout is null");
            if (this._statusBar == null)
                KLog.LogError("UnitStatusBar is null");
            if (this._crosshair == null)
                KLog.LogError("Crosshair is null");
            if (this._skillViewer == null)
                KLog.LogError("SkillViewer is null");
            if (this._itemViewer == null)
                KLog.LogError("ItemViewer is null");
            if (this._questQuickView == null)
                KLog.LogError("QuestQuickView is null");
            if (this._bossStatusBar == null)
                KLog.LogError("BossStatusBar is null");
            if(this._eventViewer == null)
                KLog.LogError("EventViewer is null");
        }

        private void Update()
        {
            // 크로스헤어 활성화 여부
            if(this._crosshair.enabled != PlayerCamera.current.aimOn)
                this._crosshair.enabled = PlayerCamera.current.aimOn;
        }

        public BossStatusBar LoadBossStatusBar(Unit unit)
        {
            this._bossStatusBar.gameObject.SetActive(true);
            this._bossStatusBar.Init(unit);

            return this._bossStatusBar;
        }

        /// <summary>
        /// 남은 몬스터수를 표시하는 HUD를 추가한다.
        /// </summary>
        /// <param name="stage"></param>
        public void LoadMonsterRemainCountHud(MonsterCleaningStage stage)
        {
            MonsterRemainCountHud hud = ObjectPoolManager.inst.New<MonsterRemainCountHud>(PrefabPath.UI.MonsterRemainCountHud);

            hud.transform.SetParent(this._rootLayout, false);
            (hud.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            hud.transform.localRotation = Quaternion.identity;
            hud.transform.localScale = Vector3.one;

            hud.Init(stage);
        }

        /// <summary>
        /// 제한시간을 표시해주는 HUD를 추가한다.
        /// </summary>
        /// <param name="stage"></param>
        public void LoadTimeAttackHud(TimeAttackStage stage)
        {
            TimeAttackHud hud = ObjectPoolManager.inst.New<TimeAttackHud>(PrefabPath.UI.TimeAttackHud);

            hud.transform.SetParent(this._rootLayout, false);
            (hud.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            hud.transform.localRotation = Quaternion.identity;
            hud.transform.localScale = Vector3.one;

            hud.Init(stage);
        }

        /// <summary>
        /// 이벤트 텍스트 표시(Toast)
        /// </summary>
        public void ViewEventText(string text)
        {
            this._eventViewer.View(text);
        }
    }
}
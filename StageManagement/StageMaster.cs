using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KShooting.UI;
using UnityEngine.SceneManagement;


namespace KShooting
{
    /// <summary>
    /// 스테이지 관리자
    /// 
    /// 이걸 상속해서 게임모드를 만든다.
    /// 
    /// 게임초기화 -> 게임시작    -> 게임플레이중 -> 승리, 패배조건 체크 -> 게임종료
    /// InitGame() -> StartGame() -> PlayUpdate() -> CheckVictory()      -> EndGame()
    ///                                           -> CheckGameOver()
    /// </summary>
    public class StageMaster : MonoBehaviour
    {
        public static StageMaster current { get; private set; }

        /// <summary>
        /// 상호작용 가능한 오브젝트 리스트.
        /// </summary>
        public List<IInteractable> interactableObjects { get; private set; } = new List<IInteractable>();


        [Tooltip("전투가능. false면 일반공격 및 스킬 사용이 불가능하다.")]
        public bool isCombatPossible = true;

        [Tooltip("시작할 때 나오는 BGM. 비어있으면 재생안함")]
        [SerializeField] private string entryBGM = string.Empty;
        /// <summary>
        /// 플레이어의 시작위치
        /// </summary>
        [SerializeField] private Transform _playerStatingPoint = null;

        /// <summary>
        /// 생성된 플레이어. Null일 경우도 있으므로 항상 체크 필요
        /// </summary>
        public Unit player { get; private set; }

        /// <summary>
        /// 생성된 플레이어 컨트롤러. Null일 경우도 있으므로 항상 체크 필요
        /// </summary>
        public PlayerController playerController { get; private set; }

        /// <summary>
        /// 스코어 정보
        /// </summary>
        public ScoreInfo scoreInfo;

        /// <summary>
        /// 게임이 종료 되었다.
        /// </summary>
        protected bool isEndGame;

        /// <summary>
        /// 게임이 시작되었다
        /// </summary>
        protected bool playing;

        /// <summary>
        /// true가 되면 게임이 무조건 승리한다.
        /// </summary>
        private bool triggerGameVictory = false;
        /// <summary>
        /// true가 되면 게임이 무조건 패배한다.
        /// </summary>
        private bool triggerGameOver = false;



        #region Unity
        protected virtual void Awake()
        {
            current = this;
        }

        protected void Start()
        {
            InitGame();
            StartGame();
        }
        
        protected virtual void Update()
        {
            // 게임이 시작되지 않았다
            if (!this.playing)
                return;

            // 게임이 끝났다.
            if (this.isEndGame)
                return;

            PlayUpdate();

            if(this.triggerGameVictory || CheckVictory()) // 승리조건 체크
            {
                // 게임 클리어
                EndGame();

                // 던전 클리어
                UserManager.ClearDungeon(SceneManager.GetActiveScene().name);

                // UI처리
                Invoke("ViewVictoryUI", 3);
            }
            else if(this.triggerGameOver || CheckGameOver()) // 패배조건 체크
            {
                // 게임 패배
                EndGame();

                // UI처리
                Invoke("ViewGameOverUI", 3);
            }
        }
        #endregion Init

        #region Stage Event
        /// <summary>
        /// 게임이 초기화될 때 호출된다.
        /// </summary>
        protected virtual void InitGame()
        {
            // 플레이어 생성
            //this.player = ObjectPoolManager.inst.New<UnityChan>(PrefabPath.Unit.UnityChan);
            this.player = ObjectPoolManager.inst.New<Metalon>(PrefabPath.Unit.PlayerMetalon);


            this.player.chCtrl.enabled = false; // 캐릭터 컨트롤러가 붙어 있으면 포지션을 강제로 Set할경우 문제가 생김.
            this.player.transform.position = this._playerStatingPoint.position;
            this.player.transform.rotation = this._playerStatingPoint.rotation;
            this.player.chCtrl.enabled = true;

            this.player.Init();

            this.playerController = this.player.controller as PlayerController;

            // BGM재생
            if (!string.IsNullOrEmpty(this.entryBGM))
            {
                SoundManager.inst.PlayBGM(this.entryBGM);
            }
            else
            {
                SoundManager.inst.StopBGM();
            }
        }

        /// <summary>
        /// 게임이 시작되는 순간 호출된다.
        /// </summary>
        protected virtual void StartGame()
        {
            // 씬 진입시간 체크
            this.scoreInfo.startTicks = DateTime.UtcNow.Ticks;
            this.playing = true;
        }

        /// <summary>
        /// 게임이 플레이중일때, 지속적으로 호출된다.
        /// </summary>
        protected virtual void PlayUpdate()
        {

        }

        /// <summary>
        /// 게임이 종료되는 순간 호출된다.
        /// </summary>
        protected virtual void EndGame()
        {
            this.isEndGame = true;
            this.scoreInfo.endTicks = DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// 승리조건을 체크한다(프레임단위로 체크함)
        /// </summary>
        protected virtual bool CheckVictory()
        {
            return false;
        }

        /// <summary>
        /// 패배조건을 체크한다(프레임단위로 체크함)
        /// </summary>
        protected virtual bool CheckGameOver()
        {
            return false;
        }

        /// <summary>
        /// 강제적으로 게임 승리 호출
        /// </summary>
        public virtual void VictoryGame()
        {
            this.triggerGameVictory = true;
        }
        
        /// <summary>
        /// 강제적으로 게임오버 호출
        /// </summary>
        public virtual void GameOver()
        {
            this.triggerGameOver = true;
        }
        #endregion Stage Event

        /// <summary>
        /// 상호작용 오브젝트 등록
        /// </summary>
        /// <param name="interactable"></param>
        public void RegistInteractable(IInteractable interactable)
        {
            StageMaster.current.interactableObjects.Add(interactable);
        }

        /// <summary>
        /// 상호작용 오브젝트 등록 취소
        /// </summary>
        /// <param name="interactable"></param>
        public void UnRegistInteractable(IInteractable interactable)
        {
            StageMaster.current.interactableObjects.Remove(interactable);
        }

        /// <summary>
        /// 씬을 전환한다.
        /// </summary>
        /// <param name="sceneName"></param>
        public void ChangeScene(string sceneName)
        {
            ObjectPoolManager.inst.CollectAll();
            SceneManager.LoadScene(sceneName);
        }

        public void ViewVictoryUI()
        {
            this.playerController.hud.gameObject.SetActive(false);
            GameUI.inst.ViewGameResultUI(true);
        }

        public void ViewGameOverUI()
        {
            this.playerController.hud.gameObject.SetActive(false);
            GameUI.inst.ViewGameResultUI(false);
        }
    }
}
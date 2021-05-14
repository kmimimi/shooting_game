using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KDialogSystem;


namespace KShooting.UI
{
    /// <summary>
    /// GameUI.
    /// 여기에 붙는 UI들은 비활성화 되면 OnChildClose를 호출해줘야함.
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        public static GameUI inst { get; private set; }


        [SerializeField] private Transform buttonRoot = null;
        [SerializeField] private Transform uiRoot     = null;

        /// <summary>
        /// 퀘스트 UI
        /// </summary>
        private QuestUI questUI;
        /// <summary>
        /// 다이얼로그 UI
        /// </summary>
        private UITextViewer dialogViewer;
        /// <summary>
        /// 선택지 UI
        /// </summary>
        private SelectionUI  selectionUI;
        /// <summary>
        /// 던전 UI
        /// </summary>
        private DungeonUI    dungeonUI;

        /// <summary>
        /// 게임 결과창 UI
        /// </summary>
        private GameResultUI gameResultUI;

        /// <summary>
        /// 인벤토리 UI
        /// </summary>
        private InventoryUI inventoryUI;

        private StoreUI storeUI;

        /// <summary>
        /// UI가 하나이상 활성화 되어있다.
        /// </summary>
        public bool isActiveUI
        {
            get
            {
                for (int i = 0 ; i < this.uiRoot.childCount ; i++)
                {
                    if (this.uiRoot.GetChild(i).gameObject.activeInHierarchy)
                        return true;
                }

                return false;
            }
        }





        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Construct()
        {
            inst = Instantiate(Resources.Load<GameUI>(PrefabPath.UI.GameUI));
            DontDestroyOnLoad(inst);

            inst.transform.position = new Vector3(0, -10000, 0); // UI가 SceneView를 가리지 않도록 처리
        }

        private void Start()
        {
            // 프리팹들이 UI들을 모두 활성화 해서 보기좋게 해뒀기 떄문에, 위치 초기화가 필요함
            this.questUI = GetComponentInChildren<QuestUI>();
            (this.questUI.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            this.questUI.gameObject.SetActive(false);

            this.dungeonUI = GetComponentInChildren<DungeonUI>();
            (this.dungeonUI.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            this.dungeonUI.gameObject.SetActive(false);

            this.dialogViewer = GetComponentInChildren<UITextViewer>();
            (this.dialogViewer.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            this.dialogViewer.gameObject.SetActive(false);

            this.selectionUI = GetComponentInChildren<SelectionUI>();
            (this.selectionUI.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            this.selectionUI.gameObject.SetActive(false);

            this.gameResultUI = GetComponentInChildren<GameResultUI>();
            (this.gameResultUI.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            this.gameResultUI.gameObject.SetActive(false);

            this.inventoryUI = GetComponentInChildren<InventoryUI>();
            (this.inventoryUI.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            this.inventoryUI.gameObject.SetActive(false);

            this.storeUI = GetComponentInChildren<StoreUI>();
            (this.storeUI.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            this.storeUI.gameObject.SetActive(false);
        }

        private void Update()
        {
            this.buttonRoot.gameObject.SetActive(!this.isActiveUI);
        }

        #region Events
        /// <summary>
        /// Called By Button Event
        /// </summary>
        public void OnQuestButtonClick()
        {
            ViewQuestUI();
        }

        /// <summary>
        /// Called By Button Event
        /// </summary>
        public void OnInventoryButtonClick()
        {
            ViewInventoryUI();
        }
        #endregion Events

        /// <summary>
        /// 퀘스트 UI를 활성화한다.
        /// </summary>
        /// <param name="key"></param>
        public void ViewQuestUI()
        {
            this.questUI.gameObject.SetActive(true);
            this.questUI.Init();
        }

        /// <summary>
        /// 인벤토리UI를 활성화한다
        /// </summary>
        public void ViewInventoryUI()
        {
            this.inventoryUI.gameObject.SetActive(true);
            this.inventoryUI.Init();
        }

        /// <summary>
        /// 상점 페이지를 연다.
        /// </summary>
        /// <param name="onBuy">상점에서 아이템을 구매했을 때 나오는 이벤트</param>
        /// <param name="onComplete">상점을 종료했을 때 나오는 이벤트</param>
        public void ViewStoreUI(UnityAction onBuy, UnityAction onComplete)
        {
            this.storeUI.gameObject.SetActive(true);
            this.storeUI.Init(onBuy, onComplete);
        }

        /// <summary>
        /// 던전 UI를 활성화 한다.
        /// </summary>
        public void ViewDungeonUI()
        {
            this.dungeonUI.gameObject.SetActive(true);
            this.dungeonUI.Init();
        }

        /// <summary>
        /// 다이얼로그 기능을 활성화한다. 반환된 UITextWritter를 통해
        /// 대사 스크립트를 재생하면 된다.
        /// </summary>
        public UITextViewer ViewDialogViewer()
        {
            this.dialogViewer.gameObject.SetActive(true);

            return this.dialogViewer;
        }

        /// <summary>
        /// 선택지를 표시한다
        /// </summary>
        public void ViewSelectionUI(SelectionData[] list, UnityAction<int> onComplete)
        {
            this.selectionUI.gameObject.SetActive(true);
            this.selectionUI.Init(list, onComplete);
        }

        /// <summary>
        /// 게임 결과창을 표시한다
        /// </summary>
        public void ViewGameResultUI(bool isClear)
        {
            this.gameResultUI.gameObject.SetActive(true);
            this.gameResultUI.Init(isClear);
        }

        /// <summary>
        /// 선택지가 활성화 되어 있다.
        /// </summary>
        /// <returns></returns>
        public bool IsSelecting()
        {
            return this.selectionUI.gameObject.activeInHierarchy;
        }

        /// <summary>
        /// Called By Child UI
        /// </summary>
        public void OnCloseChild()
        {
            if(!this.isActiveUI)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
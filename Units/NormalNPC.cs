using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KShooting.UI;
using KDialogSystem;


namespace KShooting
{
    /// <summary>
    /// 최소한의 기능만 구현하기 위해 하드코팅이 많아서 코드가 좀 복잡함.. 
    /// 
    /// 
    /// 이벤트 호출순서
    /// 
    /// 상호작용 이벤트    -> 대사가 다 나오면 선택지 보여줌 -> 선택지를 선택하면 이벤트 발생  -> 퀘스트의 경우 퀘스트 수락여부 확인
    /// InteractionEvent() -> OnFirstDialogComplete()        -> OnSelectionComplete()          -> OnSelectQuest()
    /// </summary>
    public class NormalNPC : NPC
    {
        private static List<NormalNPC> _normalNPCList = new List<NormalNPC>();
        #region Define
        private enum QuestState
        {
            /// <summary>
            /// 퀘스트를 처음받음
            /// </summary>
            First,
            /// <summary>
            /// 퀘스트 진행중
            /// </summary>
            Progrssing,
            /// <summary>
            /// 퀘스트 완료됨
            /// </summary>
            Complete
        }
        
        /// <summary>
        /// 퀘스트 다이얼로그. 심플하게 구현해둠.
        /// </summary>
        [System.Serializable]
        public struct QuestDialog
        {
            [Tooltip("퀘스트 키")]
            public string questKey;
            [Tooltip("퀘스트를 처음받을때 나오는 대사")]
            public string[] questText;
            [Tooltip("퀘스트를 수락했을때 나올 대사")]
            public string[] acceptText;
            [Tooltip("퀘스트를 거절했을때 나올 대사")]
            public string[] rejectText;
            [Tooltip("퀘스트가 완료되지 않았을때 말걸면 나오는 대사")]
            public string[] progressingText;
            [Tooltip("퀘스트가 완료됬을때 나올 대사")]
            public string[] completeText;
        }
        #endregion Define

        /// <summary>
        /// 이 NPC가 줄 수 있는 퀘스트
        /// </summary>
        [SerializeField] private QuestDialog[] _quest = null;

        [Tooltip("상점사용여부")]
        public bool useStore;
        [Tooltip("처음 말 걸었을때 나오는 대사.. 이중 하나만 랜덤으로 등장한다.")]
        public string[] firstDialog;
        [Tooltip("잡담")]
        public string[] chat = new string[] { "날씨가 좋네요", "오늘도 힘찬 하루 되세요" };
        [Tooltip("상점 선택지를 눌렀을때 나올 대사")]
        public string selectStoreText = "천천히 보세용";
        [Tooltip("상점에서 아이템을 구매했을 때 나올 대사")]
        public string buyText = "감사합니다!~";


        private UITextViewer dialogBox;
        private int textIndex;

        /// <summary>
        /// 선택지 정보. 마지막 인덱스는 잡담으로 사용. 일단 잡담을 제외한 나머지는 퀘스트
        /// </summary>
        private SelectionData[] selectionList;

        /// <summary>
        /// 선택지로 선택된 인덱스
        /// </summary>
        private int selectedIndex;

        /// <summary>
        /// 현재 대화중인 대사 큐
        /// </summary>
        private string[] dialogQueue;

        /// <summary>
        /// 퀘스트 대사 전용. 현재 진행중인 퀘스트 상태가 지정됨
        /// </summary>
        private QuestState questState;

        /// <summary>
        /// 현재 선택된 퀘스트
        /// </summary>
        private QuestDialog selectedDialog;

        /// <summary>
        /// 다음대사로 자동으로 넘어가게 할 것인지.
        /// </summary>
        private bool autoNextDialog;

        private QuestStateIcon stateIcon;

        /// <summary>
        /// 대사가 모두 끝나면 종료된다.
        /// </summary>
        private bool isEndDialog;




        #region Unity
        protected override void Start()
        {
            base.Start();
            
            // 퀘스트 상태 아이콘 초기화
            this.stateIcon = ObjectPoolManager.inst.New<QuestStateIcon>(PrefabPath.UI.QuestStateIcon);
            this.stateIcon.transform.SetParent(this.transform, false);
            this.stateIcon.transform.localPosition = new Vector3(0, 1.2f, 0);
            this.stateIcon.transform.localRotation = Quaternion.identity;
            this.stateIcon.transform.localScale = Vector3.one;

            QuestIconUpdate();

            _normalNPCList.Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _normalNPCList.Remove(this);
        }

        protected override void Update()
        {
            base.Update();
            DialogUpdate();
        }
        #endregion Unity

        #region Unpdate
        private void DialogUpdate()
        {
            if (this.dialogBox == null)
                return;

            if(this.dialogQueue.Length > 0) // 출력할 대사가 남아있음
            {
                // (다이얼로그 상호작용키를 눌렀거나 || 자동으로 넘어가야 할 경우) && 선택지가 비활성화 되어 있어야 함
                if ((InputManager.DialogInteraction() || this.autoNextDialog) && !GameUI.inst.IsSelecting())
                {
                    this.autoNextDialog = false;

                    // 대사가 나오는 중이라면 모두 보여주고 끝낸다.
                    if(this.dialogBox.running)
                    {
                        this.dialogBox.PrintTextAll();
                        return;
                    }

                    // 대사 큐에 있는 텍스트가 더이상 없다.
                    if(this.textIndex >= this.dialogQueue.Length)
                    {
                        // 더 출력할 대사 없음
                        if (this.isEndDialog)
                        {
                            EndDialog();
                            return;
                        }
                        else
                        {
                            // 대사가 끝난후 이벤트 처리
                            switch (this.selectionList[this.selectedIndex].type)
                            {
                                case SelectionType.Quest:
                                    {
                                        if (this.questState == QuestState.First)
                                        {
                                            // 퀘스트 수락여부 체크
                                            GameUI.inst.ViewSelectionUI(new SelectionData[] {
                                                new SelectionData(SelectionType.Chat, null, "네"),
                                                new SelectionData(SelectionType.Chat, null, "아니요") }, OnSelectQuest);
                                        }
                                        else if (this.questState == QuestState.Complete)
                                        {
                                            // 체크는 이전에 했었기때문에 그냥 Clear함수 바로 호출함.
                                            QuestManager.inst.ClearQuest(this.selectedDialog.questKey);

                                            SoundManager.inst.PlaySound(SoundKeys.EFFECT_CLEAR_QUEST, PlayerCamera.current.cam.transform, Vector3.zero);

                                            // 대사 종료
                                            EndDialog();
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        // 대사가 남아있음
                        this.dialogBox.PrintText(this.npcName, this.dialogQueue[this.textIndex], false);
                        SoundManager.inst.PlaySound(SoundKeys.EFFECT_DIALOG, PlayerCamera.current.cam.transform.position, 0.7f);
                    }

                    this.textIndex++;
                }
            }
        }
        #endregion Update


        #region Event
        /// <summary>
        /// 상호작용 버튼(E키) 눌렀을때 제일 처음호출되는 부분
        /// </summary>
        public override void InteractionEvent()
        {
            base.InteractionEvent();

            // 대화관련 퀘스트가 있을 경우 달성됨
            QuestManager.inst.CheckQuest_Accum(QuestCheckKey.QuestKey_Talk_ + this.npcKey, 1);

            // 다이얼로그 박스를 UI
            this.dialogBox = GameUI.inst.ViewDialogViewer();

            // 대사 큐 초기화
            ReadyDialog(new string[0]);
            this.isEndDialog = false;

            // 제일 먼저 보여줄 선택지 리스트 초기화.
            this.selectedIndex = -1;
            this.selectionList = CalcSelectionList();

            // 다이얼로그 초기화
            this.dialogBox.onComplete += OnFirstDialogComplete; // 첫대사가 끝나고 이벤트를 받을 수 있게
            this.dialogBox.PrintText(this.npcName, this.firstDialog[Random.Range(0, firstDialog.Length)], false);
            SoundManager.inst.PlaySound(SoundKeys.EFFECT_DIALOG, PlayerCamera.current.cam.transform.position, 0.7f);
        }

        /// <summary>
        /// 처음 다이얼로그대사가 출력되고 나서 호출된다.
        /// </summary>
        private void OnFirstDialogComplete()
        {
            // 등록 이벤트 취소
            this.dialogBox.onComplete -= OnFirstDialogComplete;

            // 선택지 활성화
            GameUI.inst.ViewSelectionUI(this.selectionList, OnSelectionComplete);
        }

        /// <summary>
        /// 선택이 종료되었을떄 호출된다
        /// Called By SelectionUI
        /// </summary>
        private void OnSelectionComplete(int index)
        {
            this.selectedIndex = index;

            if (this.selectionList[this.selectedIndex].type == SelectionType.Chat)
            {
                // 잡담
                ReadyDialog(this.chat);
                this.autoNextDialog = true;

                // 잡담이 끝나면 대사 종료
                this.isEndDialog    = true;
            }
            else if (this.selectionList[this.selectedIndex].type == SelectionType.Quest)
            {
                // 퀘스트
                string questKey     = this.selectionList[this.selectedIndex].key;

                // 퀘스트에 맞는 다이얼로그 정보를 불러온다.
                this.selectedDialog = GetDialog(questKey);


                if (!QuestManager.inst.HasQuest(questKey))
                {
                    // 퀘스트를 처음 받는다
                    ReadyDialog(this.selectedDialog.questText);
                    this.questState = QuestState.First;
                }
                else
                {
                    if (!QuestManager.inst.IsFinishProgress(questKey))
                    {
                        // 퀘스트를 진행중이지만 완료를 못했다
                        ReadyDialog(this.selectedDialog.progressingText);
                        this.questState = QuestState.Progrssing;

                        this.isEndDialog = true;
                    }
                    else
                    {
                        // 퀘스트를 완료할 준비가 되었다.
                        ReadyDialog(this.selectedDialog.completeText);
                        this.questState = QuestState.Complete;
                    }
                }
                this.autoNextDialog = true;
            }
            else if (this.selectionList[this.selectedIndex].type == SelectionType.Store)
            {
                //상점
                ReadyDialog(new string[] { this.selectStoreText });
                this.autoNextDialog = true;

                GameUI.inst.ViewStoreUI(() => {
                    // 구매 완료 메세지
                    ReadyDialog(new string[] { this.buyText });
                    this.autoNextDialog = true;
                    SoundManager.inst.PlaySound(SoundKeys.EFFECT_PURCHASE, this.transform.position);

                }, EndDialog);
            }
        }

        /// <summary>
        /// 퀘스트 수락여부 확인
        /// Called By SelectionUI
        /// </summary>
        private void OnSelectQuest(int index)
        {
            if(index == 0) // 수락
            {
                QuestManager.inst.AddQuest(this.selectedDialog.questKey);
                QuestManager.inst.AddQuickView(this.selectedDialog.questKey);
                ReadyDialog(this.selectedDialog.acceptText);
            }
            else // 거절
            {
                ReadyDialog(this.selectedDialog.rejectText);
            }

            this.isEndDialog = true;
            this.autoNextDialog = true;
        }

        public QuestDialog GetDialog(string questKey)
        {
            for (int i = 0 ; i < this._quest.Length ; i++)
            {
                if (this._quest[i].questKey == questKey)
                    return this._quest[i];
            }

            throw new KeyNotFoundException("퀘스트 키에 해당되는 스크립트 정보가 없음. " + questKey);
        }
        #endregion Event

        /// <summary>
        /// NPC로부터 선택할 수 있는 선택지를 만든다.
        /// </summary>
        private SelectionData[] CalcSelectionList()
        {
            List<SelectionData> data = new List<SelectionData>();

            int remainViewQuestCount = this.useStore ? 2 : 3; // Count가 0이되면 더이상 퀘스트창에 나타나지 않는다.

            // 받을 수 있는 퀘스트 체크
            for(int i=0; i<this._quest.Length; i++)
            {
                // 보여줄 수 있는 최대 슬롯초과하면 무시
                if (remainViewQuestCount <= 0)
                    break;

                string key = this._quest[i].questKey;
                var questInfo = QuestDatabase.GetInfo(key);

                // 이미 진행중인 퀘스트
                if (QuestManager.inst.HasQuest(key))
                {
                    data.Add(new SelectionData(SelectionType.Quest, key, questInfo.title));
                    remainViewQuestCount--;
                }
                else
                {
                    // 이미 클리어 된 퀘스트
                    if (QuestManager.inst.IsClearQuest(key))
                        continue;

                    // 받을 수 있는 퀘스트인지 체크
                    if(QuestManager.inst.IsAcceptQuest(key))
                    {
                        data.Add(new SelectionData(SelectionType.Quest, key, questInfo.title));
                        remainViewQuestCount--;
                    }
                }
            }

            if (this.useStore)
                data.Add(new SelectionData(SelectionType.Store, null, "상점"));

            data.Add(new SelectionData(SelectionType.Chat, null, "잡담"));

            return data.ToArray();
        }

        /// <summary>
        /// 다이얼로그에 출력할 텍스트를 넣는다
        /// </summary>
        /// <param name="dialogText"></param>
        private void ReadyDialog(string[] dialogText)
        {
            this.dialogQueue = dialogText;
            this.textIndex = 0;
        }


        private void QuestIconUpdate()
        {
            QuestStateIcon.State state = QuestStateIcon.State.Empty;
            
            for (int i=0; i<this._quest.Length; i++)
            {
                if(QuestManager.inst.HasQuest(this._quest[i].questKey))
                {
                    if (QuestManager.inst.IsFinishProgress(this._quest[i].questKey))
                    {
                        // 클리어된 퀘스트가 하나라도 있으면 Finish
                        this.stateIcon.SetState(QuestStateIcon.State.Finish);
                        return;
                    }
                    else
                    {
                        // 진행중인 퀘스트가 있다면 Progress
                        state = QuestStateIcon.State.Progress;
                    }
                }
                else if(!QuestManager.inst.IsClearQuest(this._quest[i].questKey) 
                    && QuestManager.inst.IsAcceptQuest(this._quest[i].questKey))
                {
                    // 진행중인 퀘스트가 없고, 퀘스트를 받을 수 있을때
                    if(state != QuestStateIcon.State.Progress)
                        state = QuestStateIcon.State.HasQuest;
                }
            }

            this.stateIcon.SetState(state);
        }

        /// <summary>
        /// 대화를 완전히 종료한다
        /// </summary>
        private void EndDialog()
        {
            this.dialogBox.gameObject.SetActive(false);
            this.dialogBox = null;

            // 퀘스트 상태표시를 업데이트 한다.
            for(int i=0; i<_normalNPCList.Count; i++)
                _normalNPCList[i].QuestIconUpdate();
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using KShooting.UI;
 
 
namespace KDialogSystem
{
    public class UITextViewer : MonoBehaviour
    {
        private const string FORMAT = "{0}{1}";
 
        /// <summary>
        /// 대사가 출력중이다.
        /// </summary>
        public bool printing { get; protected set; }
        /// <summary>
        /// 대사창(UI)가 활성화 되는 중(페이드)
        /// </summary>
        public bool waiting  { get; protected set; }
        /// <summary>
        /// 대사 텍스트가 활성화 되는중(페이드)
        /// </summary>
        public bool fading   { get; protected set; }


        /* UI */
        /// <summary>
        /// 대사창(UI)이 활성화 될지 판단해줌
        /// </summary>
        private bool isView = true;

        [SerializeField] protected Image nameImage;
        [SerializeField] protected TMP_Text  nameText;
        [SerializeField] protected Image dialogImage;
        [SerializeField] protected TMP_Text dialogText;

        /* Option */
        /// <summary>
        /// 텍스트 글자 출력 사이 간격
        /// </summary>
        public float writeInterval = 0.02f;

        /*public*/  float dialogFadeTime = 0;//.3f;
        /// <summary>
        /// 텍스트 각각의 페이드 시간
        /// </summary>
        private float fadeTime = 0.2f;
 
        /// <summary>
        /// 텍스트 출력이 전부 끝났을 때, 커서 끝에 끝났음을 표시해줌
        /// </summary>
        public    bool   alterCursor              = false;
        public    string alternateCursorCharacter = "_";
        protected float  alterCursorInterval      = 0.5f;
 
        /* Text : 이름이 있는 대시의 경우, 별도로 접두랑 접미에 추가적으로 표시해줌(쌍따옴표처럼..)*/
        public string beforeTalk = "『";
        public string afterTalk  = "』";
 
        /// <summary>
        /// 문장 텍스트 처리 클래스
        /// </summary>
        protected DialogText dialog       = new DialogText();
        protected int        dialogCursor = 0;

        /* Internal */
        /// <summary>
        /// 현재 사용중인 텍스트 커서 데이터
        /// </summary>
        private string textCursor;
        /// <summary>
        /// 텍스트를 writeInterval에 맞게 출력해주도록 도와줌(deltaTime)
        /// </summary>
        private float schedule;
 
        /// <summary>
        /// 대사창 페이딩 할때 사용
        /// </summary>
        private float startDialogImageAlpha;
        /// <summary>
        /// 대사 페이딩 할때 사용
        /// </summary>
        private float startDialogTextAlpha;

        /* Delegate */
        public event UnityAction onStart;
        public event UnityAction onUpdate;
        public event UnityAction onComplete;

        /* 최적화 */
        private int lastTextCursor; // 가비지 컬렉팅 작업 최소화(지금은 미사용)

        /// <summary>
        /// 텍스트가 표시중이다.
        /// </summary>
        public bool       running       { get { return this.printing || this.waiting || this.fading; } }
        public TextAlignmentOptions alignment     { get { return this.dialogText.alignment; } set { this.dialogText.alignment = value; } }
        /// <summary>
        /// 출력된 이름
        /// </summary>
        public string     characterName { get { return this.nameText.text; } set { ViewName(true); this.nameText.text = value; } }
        /// <summary>
        /// 출력된 텍스트
        /// </summary>
        public string     text
        {
            get { return dialog.originText; }
            set
            {
                this.dialog.Clear();
                this.dialog.Append(value);
                this.dialog.ClearFadeTime();
 
                bool originTalk = this.dialog.isTalk;
 
                this.dialog.isTalk = true;
                this.dialogCursor = dialog.length;
                this.dialog.isTalk = originTalk;

                ViewDialog(true, true);
            }
        }



        #region Unity
        private void Awake()
        {
            // 데이터 초기화
            dialog.SetTalkText(beforeTalk, afterTalk);
            
            this.startDialogImageAlpha = (this.dialogImage != null) ? this.dialogImage.color.a : 0;
            this.startDialogTextAlpha  = (this.dialogText  != null) ? this.dialogText .color.a : 0;
 
            ViewName(false);
            ViewDialog(false, true);
        }

        private void OnDisable()
        {
            this.printing = false;
            this.waiting  = false;                   
            this.fading   = false;

            GameUI owner = GetComponentInParent<GameUI>();

            if (owner != null)
                owner.OnCloseChild();
        }

        private void LateUpdate()
        {
            UpdateText();
            UpdateDialog();

            if (this.running && this.onUpdate != null)
                this.onUpdate();
        }
        #endregion

        #region Update
        /// <summary>
        /// 현재 출력되고 있는 텍스트정보는 여기서 업데이트 된다.
        /// </summary>
        private void UpdateText()
        {
            if (this.printing)
            {
                this.schedule += Time.deltaTime;

                if (this.schedule > this.writeInterval)
                {
                    this.schedule = 0;
 
                    if (this.dialogCursor < this.dialog.length)
                        this.dialogCursor++;
                    else
                        Stop();
                }
            }
        }

        /// <summary>
        /// 텍스트를 UI에 표시한다.
        /// </summary>
        private void UpdateDialog()
        {
            this.dialogText.text = string.Format(FORMAT, dialog.ToString(dialogCursor), textCursor);
        }
        #endregion

        #region Dialog
        /// <summary>
        /// 텍스트 출력을 시작함
        /// </summary>
        private void Run()
        {
            this.printing = true;
            this.schedule = 0;
            this.onStart?.Invoke();
        }

        /// <summary>
        /// 텍스트 출력을 중지한다.
        /// </summary>
        private void Stop()
        {
            this.printing = false;
            this.waiting  = false;                   
            this.fading   = true;

            StartCoroutine(CheckFading());
        }

        /// <summary>
        /// 대사창 활성화 될때, 
        /// </summary>
        private void FadeDialog(bool isIn)
        {
            // 대사창 UI를 페이드 인 시킨다.
            if (this.dialogImage != null)
            {
                Color c = this.dialogImage.color;
                this.dialogImage.color = new Color(c.r, c.g, c.b, isIn ? 0 : this.startDialogImageAlpha);
                this.dialog.InitializeFadeTime(this.fadeTime, 0, isIn);

                this.dialogImage.CrossFadeAlpha(isIn ? this.startDialogImageAlpha : 0, this.dialogFadeTime, false);
            }
        }
 
        /// <summary>
        /// 이름 출력
        /// </summary>
        /// <param name="isView"></param>
        public void ViewName(bool isView)
        {
            if(this.nameImage != null)
                this.nameImage.enabled = isView;
            if(this.nameText != null)
                this.nameText.enabled = isView;
        }
 
        /// <summary>
        /// 대사 출력
        /// </summary>
        public void ViewDialog(bool isView, bool immediate=false)
        {
            if (isView == this.isView)
                return;
 
            if(this.dialogImage != null)
            { 
                if (this.dialogFadeTime > 0)
                {
                    if(immediate)
                    {
                        Color c           = this.dialogImage.color;
                        this.dialogImage.color = new Color(c.r, c.g, c.b, isView ? this.startDialogImageAlpha : 0);
                    }
                    else
                    {
                        FadeDialog(isView);
                    }
                }
                else
                {
                    Color c           = this.dialogImage.color;
                    this.dialogImage.color = new Color(c.r, c.g, c.b, isView ? this.startDialogImageAlpha : 0);
                    this.dialog.InitializeFadeTime(0, 0, false);
                }
            }
 
            this.isView = isView;
        }
 
        /// <summary>
        /// 대사 출력(통합함수)
        /// </summary>
        /// <param name="connect">대사를 뒤에 이어서 붙일 것인지</param>
        public void PrintText(string name, string text, bool connect, TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft)
        {
            if (this.running)  PrintTextAll(); // 일하는 중엔 출력중인 대사를 한번에 보여줌
            if (!connect)      ClearText();    // 대사를 이어붙이지 않을경우, 이전 대사를 지워줌

            if (this.nameText != null)
                this.nameText.text = name;
 

            ////////////////////////////////////////////////////
            // 값 셋팅
            this.alignment             = alignment;
            this.dialog.isTalk         = !string.IsNullOrEmpty(name);
            this.dialog.defaultColor   = this.dialogText.color;
            this.dialog.defaultColor.a = (byte)(startDialogTextAlpha*255);
 
            this.dialogCursor = dialog.length;
            this.dialog.Append(text);
 
 
            if (isView) // 대사창 애니메이션
            {
                this.dialog.InitializeFadeTime(this.fadeTime, this.dialogCursor, true);
                ViewName(this.dialog.isTalk);
                Run();
            }
            else // 그냥 보여줌
            {
                ViewDialog(true);
                ViewName(this.dialog.isTalk);
                this.waiting = true;
                StartCoroutine(WaitPrint());
            }
        }
 
        private void StartPrint()
        {
            this.waiting = false;
            this.dialog.InitializeFadeTime(this.fadeTime, this.dialogCursor, true);
            ViewName(this.dialog.isTalk);
            Run();
        }

        private IEnumerator WaitPrint()
        {
            yield return new WaitForSeconds(this.dialogFadeTime);

            if (this.waiting)
                StartPrint();
        }
 
        /// <summary>
        /// 대사 한번에 출력
        /// </summary>
        public void PrintTextAll()
        {
            if (this.waiting || this.fading)
                return;

            this.dialogCursor = this.dialog.length;
            StartPrint();
        }

        private IEnumerator CheckFading()
        {
            yield return new WaitForSeconds(this.fadeTime);
            this.fading = false;

            this.onComplete?.Invoke();
        }
 
        public void ClearText()
        {
            this.dialog.Clear();
            this.dialogCursor = 0;
        }

        public void ClearAll()
        {
            ClearText();
            ViewName(false);
            ViewDialog(false, true);
        }

        /// <summary>
        /// 대사창 끝의 커서깜빡임 효과
        /// </summary>
        /// <returns></returns>
        private IEnumerator ChangeCursor()
        {
            while (true)
            {
                if (this.alterCursor)
                {
                    if (string.Empty == this.textCursor)
                        this.textCursor = this.alternateCursorCharacter;
                    else
                        this.textCursor = string.Empty;
                }
                else
                {
                    this.textCursor = string.Empty;
                }
 
                yield return new WaitForSeconds(this.alterCursorInterval);
            }
        }
        #endregion Dialog
    }
}
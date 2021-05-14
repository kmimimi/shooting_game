using UnityEngine;
using System.Collections.Generic;
using System.Text;


namespace KDialogSystem
{
    public class DialogText
    {
        private const string START_HIDE_COLOR_TAG = "<color=#00000000>";
        private const string END_COLOR_TAG        = "</color>";


        public string originText;

        public Color32 defaultColor;
        /// <summary>
        /// 캐릭터 대사일경우 true
        /// </summary>
        public bool isTalk = false;

        private StringBuilder _builder = new StringBuilder();
        private List<Glyph>   _glyphs  = new List<Glyph>();
        private Glyph[]       _before  = new Glyph[0];
        private Glyph[]       _after   = new Glyph[0];

        public int length { get { return this._glyphs.Count; } }


        /// <summary>
        /// 페이드 초기화
        /// </summary>
        public void InitializeFadeTime(float fadeTime, bool isIn)
        {
            InitializeFadeTime(fadeTime, 0, isIn);
        }

        /// <summary>
        /// 페이드 초기화
        /// </summary>
        public void InitializeFadeTime(float fadeTime, int startIndex, bool isIn)
        {
            int count = 0;

            for (int i = 0; i < this._before.Length; i++)
            {
                if (count <= startIndex) { count++; continue; }
                this._before[i].SetFadeTime(isIn, fadeTime, this.defaultColor);
            }

            for (int i = 0; i < this._glyphs.Count; i++)
            {
                if (count <= startIndex) { count++; continue; }
                this._glyphs[i].SetFadeTime(isIn, fadeTime, this.defaultColor);
            }

            for (int i = 0; i < this._after.Length; i++)
            {
                if (count <= startIndex) { count++; continue; }
                this._after[i].SetFadeTime(isIn, fadeTime, this.defaultColor);
            }
        }

        /// <summary>
        /// 페이드 클리어
        /// </summary>
        public void ClearFadeTime()
        {
            InitializeFadeTime(0, true);
        }

        /// <summary>
        /// 전체 클리어
        /// </summary>
        public void Clear()
        {
            this.originText = string.Empty;
            this._glyphs.Clear();
        }

        public void SetTalkText(string beforeTalk, string afterTalk)
        {
            this._before = ToGlyphs(beforeTalk).ToArray();
            this._after = ToGlyphs(afterTalk).ToArray();
        }

        public void SubString(int index, int count)
        {
            this._glyphs = this._glyphs.GetRange(index, count);
        }

        public virtual void Append(string text)
        {
            this._glyphs.AddRange(ToGlyphs(text));
        }

        /// <summary>
        /// 텍스트 데이터를 글리프로 변환한다.
        /// </summary>
        private List<Glyph> ToGlyphs(string text)
        {
            DialogParser.Token[] tokens = DialogParser.ToParse(text);
            List<Glyph>          glyphs = new List<Glyph>();

            bool bold    = false;
            bool italic  = false;
            string size  = string.Empty;
            string color = string.Empty;

            // 적절하게 값 파싱
            this.originText += text;
            foreach (DialogParser.Token token in tokens)
            {
                switch (token.type)
                {
                    case DialogParser.Token.Type.text:
                        for (int i = 0; i < token.data.Length; i++)
                        {
                            Glyph glyph = new Glyph
                            {
                                text     = token.data[i],
                                isBold   = bold,
                                isItalic = italic,
                                size     = size,
                                color    = color
                            };

                            glyphs.Add(glyph);
                        }
                        break;
                    case DialogParser.Token.Type.boldStart:
                        bold = true;
                        break;
                    case DialogParser.Token.Type.boldEnd:
                        bold = false;
                        break;
                    case DialogParser.Token.Type.italicStart:
                        italic = true;
                        break;
                    case DialogParser.Token.Type.italicEnd:
                        italic = false;
                        break;
                    case DialogParser.Token.Type.sizeStart:
                        size = token.data;
                        break;
                    case DialogParser.Token.Type.sizeEnd:
                        size = string.Empty;
                        break;
                    case DialogParser.Token.Type.colorStart:
                        color = token.data;
                        break;
                    case DialogParser.Token.Type.colorEnd:
                        color = string.Empty;
                        break;
                }
            }

            return glyphs;
        }

        public override string ToString()
        {
            return ToString(this._glyphs.Count);
        }

        public string ToString(int endIndex)
        {
            if (endIndex > this.length)
            {
                endIndex = this.length;
            }

            this._builder.Remove(0, this._builder.Length);



            // 글자 페이드 관련 처리(시작, 끝부분에 투명 태그 삽입)
            bool isHide = false;
            if (this.isTalk)
            {
                for (int i = 0; i < this._before.Length; i++)
                {
                    if (!isHide)
                    {
                        isHide = endIndex <= i;

                        if (isHide)
                            this._builder.Append(START_HIDE_COLOR_TAG);
                    }

                    this._builder.Append(this._before[i].ToString(endIndex > 0 ? isHide : true));
                }
            }

            for (int i = 0; i < this._glyphs.Count; i++)
            {
                if (!isHide)
                {
                    isHide = endIndex <= i;

                    if (isHide)
                        this._builder.Append(START_HIDE_COLOR_TAG);
                }

                this._builder.Append(this._glyphs[i].ToString(endIndex <= i));
            }

            if (this.isTalk)
            {
                for (int i = 0; i < this._after.Length; i++)
                {
                    if (!isHide)
                    {
                        isHide = endIndex <= i;

                        if (isHide)
                            this._builder.Append(START_HIDE_COLOR_TAG);
                    }

                    this._builder.Append(this._after[i].ToString(endIndex > 0 ? isHide : true));
                }
            }


            if (isHide)
            {
                this._builder.Append(END_COLOR_TAG);
            }

            return this._builder.ToString();
        }
    }
}

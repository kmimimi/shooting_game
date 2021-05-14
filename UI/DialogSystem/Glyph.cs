using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KDialogSystem
{ 
    /// <summary>
    /// 문자 하나의 정보. 굵게 이탤릭같은 HTML태그속성, 그리고 불투명도(텍스트를 부드럽게 출력하는용도) 정보가 담겨있다.
    /// </summary>
    internal class Glyph
    {
        private const string FORMAT_DEFAULT    = "{0}{1}{2}";
        private const string FORMAT_COLOR      = "<color=#00000000>{0}</color>";
        private const string FORMAT_FADE_COLOR = "<color=#{0}{1:X2}>";

        private const string BOLD       = "<b>";
        private const string END_BOLD   = "</b>";
        private const string ITALIC     = "<i>";
        private const string END_ITALIC = "</i>";
        private const string END_SIZE   = "</size>";
        private const string END_COLOR  = "</color>";


        private bool fadeIn = true;

        /// <summary>
        /// 문자
        /// </summary>
        public char   text;
        public string defaultColor;
        public bool   isBold;
        public bool   isItalic;
        public bool   isResize;
        public bool   isColor;

        private string _size;
        private string _color;
        private float  _fadeTime;
        private float  _fadeDeltaTime = -1;

        private bool   _fading { get { return 0 < this._fadeTime; } }

        public string size
        {
            get { return this._size; }
            set { this._size = value; if (!string.IsNullOrEmpty(value)) { isResize = true; } }
        }

        public string color
        {
            get { return this._color; }
            set { this._color = value; if (!string.IsNullOrEmpty(value)) { isColor = true; } }
        }

        public string fadeColor
        {
            get
            {
                if (!this.fadeIn || this._fading)
                {
                    string originColor, colorValue;

                    if (!string.IsNullOrEmpty(this.color))
                    {
                        originColor = this.color;
                        colorValue = this.color.Substring(8, 6);
                    }
                    else
                    {
                        originColor = this.defaultColor;
                        colorValue = this.defaultColor.Substring(8, 6);
                    }

                    int alpha = int.Parse(originColor.Substring(14, 2), System.Globalization.NumberStyles.HexNumber);
                    float cal = this._fadeDeltaTime > 0 ? alpha * Mathf.Abs((this.fadeIn ? 1 : 0) - (this._fadeDeltaTime / this._fadeTime)) : 0;

                    return string.Format(FORMAT_FADE_COLOR, colorValue, (int)cal);
                }
                else
                {
                    return this.color;
                }
            }
        }

        /// <summary>
        /// 글리프의 불투명도값 설정
        /// </summary>
        public void SetFadeTime(bool isIn, float time, Color32 defaultColor)
        {
            this.fadeIn           = isIn;
            this._fadeTime         = time;
            this._fadeDeltaTime    = -1;
            this.defaultColor = string.Format("<color=#{0}>", ColorToHex(defaultColor));
        }

        /// <summary>
        /// 글리프를 출력한다.
        /// </summary>
        public string ToStringAndNonHide()
        {
            this._fadeTime = 0;
            return ToString(false);
        }

        /// <summary>
        /// 글리프를 출력한다.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary>
        /// 글리프를 출력한다.
        /// </summary>
        public string ToString(bool isHide)
        {
            string ret = this.text.ToString();

            if (this.isBold)   ret = string.Format(FORMAT_DEFAULT, BOLD, ret, END_BOLD);
            if (this.isItalic) ret = string.Format(FORMAT_DEFAULT, ITALIC, ret, END_ITALIC);
            if (this.isResize) ret = string.Format(FORMAT_DEFAULT, this.size, ret, END_SIZE);

            if (isHide)
            {
                ret = string.Format(FORMAT_COLOR, ret);
            }
            else
            {
                if (this._fadeTime > 0)
                {
                    if (this._fadeDeltaTime == - 1)
                        this._fadeDeltaTime = this._fadeTime;

                    this._fadeDeltaTime -= Time.deltaTime;

                    if (this._fadeDeltaTime < 0)
                        this._fadeTime = 0;
                }


                if (!this.fadeIn || this._fading || this.isColor)
                {
                    ret = string.Format(FORMAT_DEFAULT, this.fadeColor, ret, END_COLOR);
                }
            }

            return ret;
        }

        private static string ColorToHex(Color32 color)
        {
            return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
        }

        private static Color32 HexToColor(string hex)
        {
            hex = hex.Replace("0x", string.Empty);
            hex = hex.Replace("#", string.Empty);

            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte a = (hex.Length == 8) ? byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) : (byte)255;

            return new Color32(r, g, b, a);
        }
    }
}
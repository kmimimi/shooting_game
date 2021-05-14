using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace KDialogSystem
{ 
    public class DialogParser
    {
        /// <summary>
        /// 문자열로 되어 있는 텍스트를 Token단위로 파싱
        /// </summary>
        public static Token[] ToParse(string value)
        {
            List<Token> tokens = new List<Token>();

            Regex regex = new Regex(@"\<.*?\>");
            Match m = regex.Match(value);

            int index = 0;
            while (m.Success)
            {
                AddText(tokens, value.Substring(index, m.Index - index));
                AddTag(tokens, m.Value);

                index = m.Index + m.Value.Length;
                m = m.NextMatch();
            }

            if (index < value.Length)
            {
                string postText = value.Substring(index, value.Length - index);

                if (postText.Length > 0)
                    AddText(tokens, postText);
            }

            return tokens.ToArray();
        }

        /// <summary>
        /// 문자를 더함
        /// </summary>
        private static void AddText(List<Token> tokens, string value)
        {
            tokens.Add(new Token(Token.Type.text, value));
        }

        /// <summary>
        /// 태그를 더함
        /// </summary>
        private static void AddTag(List<Token> tokens, string value)
        {
            if (value.Length < 3 || value[0] != '<' || value[value.Length - 1] != '>')
                return;


            Token.Type type = Token.Type.text;


            if (value.Contains("<b>"))
            {
                type = Token.Type.boldStart;
            }
            else if (value.Contains("</b>"))
            {
                type = Token.Type.boldEnd;
            }
            else if (value.Contains("<i>"))
            {
                type = Token.Type.italicStart;
            }
            else if (value.Contains("</i>"))
            {
                type = Token.Type.italicEnd;
            }
            else if (value.Contains("<size="))
            {
                type = Token.Type.sizeStart;
            }
            else if (value.Contains("</size>"))
            {
                type = Token.Type.sizeEnd;
            }
            else if (value.Contains("<color="))
            {
                type = Token.Type.colorStart;
                value = value.Replace("'", string.Empty);

                if (!value.Contains("#"))
                {
                    value = StringToHexValue(value);
                }
                else if ((value.Substring(8, value.Length - 9)).Length == 6)
                {
                    value = SetAlphaValue(value);
                }
            }
            else if (value.Contains("</color>"))
            {
                type = Token.Type.colorEnd;
            }

            tokens.Add(new Token(type, value));
        }

        /// <summary>
        /// 미리 정의된 컬러이름을 코드로 변환
        /// </summary>
        private static string StringToHexValue(string text)
        {
            text = text.Replace("aqua"     , "#00ffffff");
            text = text.Replace("black"    , "#000000ff");
            text = text.Replace("blue"     , "#0000ffff");
            text = text.Replace("cyan"     , "#00ffffff");
            text = text.Replace("brown"    , "#a52a2aff");
            text = text.Replace("darkblue" , "#0000a0ff");
            text = text.Replace("fuchsia"  , "#ff00ffff");
            text = text.Replace("green"    , "#008000ff");
            text = text.Replace("grey"     , "#808080ff");
            text = text.Replace("lightblue", "#add8e6ff");
            text = text.Replace("lime"     , "#00ff00ff");
            text = text.Replace("magenta"  , "#ff00ffff");
            text = text.Replace("maroon"   , "#800000ff");
            text = text.Replace("navy"     , "#000080ff");
            text = text.Replace("olive"    , "#808000ff");
            text = text.Replace("orange"   , "#ffa500ff");
            text = text.Replace("purple"   , "#800080ff");
            text = text.Replace("red"      , "#ff0000ff");
            text = text.Replace("silver"   , "#c0c0c0ff");
            text = text.Replace("teal"     , "#008080ff");
            text = text.Replace("white"    , "#ffffffff");
            text = text.Replace("yellow"   , "#ffff00ff");

            return text;
        }

        /// <summary>
        /// 컬러코드에 알파값 추가
        /// </summary>
        private static string SetAlphaValue(string text)
        {
            return string.Format("<color=#{0}ff>", text.Substring(8, text.Length - 9));
        }


        /*
            Class
        */
        /// <summary>
        /// 문자 하나의 정보. 텍스트 혹은 태그속성하나를 나타냄
        /// </summary>
        public class Token
        {
            public enum Type
            {
                text,
                boldStart,
                boldEnd,
                italicStart,
                italicEnd,
                sizeStart,
                sizeEnd,
                colorStart,
                colorEnd
            }

            public Type type;
            public string data;


            public Token() : this(Type.text, string.Empty) { }
            public Token(Type type, string data)
            {
                this.type = type;
                this.data = data;
            }

            public override string ToString()
            {
                return string.Format("Type: " + type + " / Data: " + data);
            }
        }
    }
}

using System;
using System.Text;

/* 
 csc /t:library FixHalfWidthWord.cs /r:.\ASpecialWord.dll /r:.\AheadTextReader.dll /r:.\SimpleTokenParser.dll /r:.\Token.dll
 csc /t:exe FixHalfWidthWord.cs /r:.\ASpecialWord.dll /r:.\AheadTextReader.dll /r:.\SimpleTokenParser.dll /r:.\Token.dll
 */

namespace Parser.Word
{
    public class FixHalfWidthWord : ASpecialWord
    {
        static Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
        private string TOKEN_TYPE_TOKEN_TYPE_FIXBYTESIZE = "TOKEN_TYPE_FIXBYTESIZE";
        private int startIndex;
        public int StartIndex { get { return startIndex; } }
        public int EndIndex { get { return startIndex + halfWidthSize; } }
        private int halfWidthSize;


        public FixHalfWidthWord(int startIndex, int halfWidthSize)
        {
            this.startIndex = startIndex;
            this.halfWidthSize = halfWidthSize;
        }

        public override string GetMatchingWord()
        {
            return "";
        }

        public override bool Matching(AheadTextReader source)
        {
            if (source.MatchForward("\n"))
            {
                return false;
            }
            else
            {
                return source.BufferHeadPosition == startIndex;
            }
        }

        public override Token Parse(AheadTextReader source)
        {
            if (source.BufferHeadPosition == startIndex)
            {
                string result = "";
                while (sjisEnc.GetByteCount(result) < halfWidthSize)
                {
                    if (source.MatchForward("\n"))
                    {
                        break;
                    }
                    else
                    {
                        result += source.PopForward(1);
                    }
                }

                if (sjisEnc.GetByteCount(result) > halfWidthSize)
                {
                    throw new Exception("最終文字として全角文字を読み込んだことで、指定した半角文字数を超えてしまいました。行位置=" + source.LinePositoin + ", 文字位置=" + source.BufferHeadPosition);
                }

                return new Token(TOKEN_TYPE_TOKEN_TYPE_FIXBYTESIZE, result);
            }
            else
            {
                return Token.EMPTY_TOKEN;
            }
        }

        public override int CalcurateWordLength()
        {
            return 0;
        }


    }
}
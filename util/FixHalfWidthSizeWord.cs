using System;
using System.Text;

/* 
 csc /t:exe SkipWord.cs /r:.\ISpecialWord.dll /r:.\AheadTextReader.dll /r:.\SimpleTokenParser.dll /r:.\Token.dll
 */

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
            // 最終読込文字がマルチバイト文字で指定した半角文字サイズをオーバーしたとしても正常処理する。
            // => test Codeで示せ。
            /*if (sjisEnc.GetByteCount(result) > halfWidthSize)
            {
                throw new Exception("解析エラー：" + source.BufferHeadPosition + "付近でエラーが発生しました。");
            }*/

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

    public static void Main(string[] args)
    {
        var parser = new SimpleTokenParser();
        parser.AddFixHalfWidthWord(new FixHalfWidthWord(0, 6));
        parser.AddFixHalfWidthWord(new FixHalfWidthWord(6, 1));
        parser.AddFixHalfWidthWord(new FixHalfWidthWord(78, Int32.MaxValue));


        foreach (var token in parser.iteratorToken(System.Console.In))
        {
            System.Console.WriteLine(token);
        }
    }
}
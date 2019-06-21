using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

/* 
  csc /t:library .\SimpleTokenParser.cs /r:.\AheadTextReader.dll /r:.\ASpecialWord.dll /r:.\Token.dll /r:.\FixHalfWidthWord.dll
*/

public class SimpleTokenParser
{
    private List<ASpecialWord> specialWords = new List<ASpecialWord>();
    private List<FixHalfWidthWord> fixHalfWidthWords = new List<FixHalfWidthWord>();
    private const string TOKEN_TYPE_NORMAL = "TOKEN_TYPE_NORMAL";

    public void AddSpecialWord(ASpecialWord specialWord)
    {
        specialWords.Add(specialWord);
    }

    public void AddFixHalfWidthWord(FixHalfWidthWord FixHalfSizeWord)
    {
        fixHalfWidthWords.Add(FixHalfSizeWord);
    }

    public IEnumerable<string> iteratorTokenString(TextReader source)
    {
        foreach (var token in iteratorToken(source))
        {
            yield return token.Body;
        }
    }

    public IEnumerable<Token> iteratorToken(TextReader source)
    {
        var currentSpWords = fixHalfWidthWords.Concat(specialWords).ToList();

        if (currentSpWords.Count() == 0)
        {
            throw new Exception("SpecialWordが登録されていないため、解析を行うことが出来ません。AddSpecialWordメソッドで登録してからiteratorTokenを呼び出して下さい。");
        }

        var aheadReader = new AheadTextReader(source, countMaxBufferSize());
        fixHalfWidthWords.ForEach(
            word =>
            {
                aheadReader.addSplitIndexWithHalfWidh(word.StartIndex);
                aheadReader.addSplitIndexWithHalfWidh(word.EndIndex);
            }
        );

        var normalToken = "";

        while (!aheadReader.AtEndOfStream)
        {
            bool matched = false;
            foreach (var spWord in currentSpWords)
            {

                if (spWord.Matching(aheadReader))
                {
                    matched = true;

                    if (normalToken != "")
                    {
                        yield return new Token(TOKEN_TYPE_NORMAL, normalToken);
                        normalToken = "";
                    }
                    var token = spWord.Parse(aheadReader);
                    if (!token.IsEmpty())
                    {
                        yield return token;
                    }

                    break;
                }
            }

            if (!matched)
            {
                normalToken += aheadReader.PopForward(1);
            }
        }
        yield return new Token(TOKEN_TYPE_NORMAL, normalToken);
    }

    private int countMaxBufferSize()
    {
        return specialWords.Aggregate(0, (continueNum, item) => continueNum > item.CalcurateWordLength() ? continueNum : item.CalcurateWordLength());
    }

    public static void Main()
    {
        var parser = new SimpleTokenParser();
        parser.AddFixHalfWidthWord(new FixHalfWidthWord(0, 6));
        parser.AddFixHalfWidthWord(new FixHalfWidthWord(6, 1));
        parser.AddFixHalfWidthWord(new FixHalfWidthWord(16, Int32.MaxValue));
 
        var stringReader = new StringReader("999999*xyz      comment" + "\n" + "111111 aiueoc");
        var result = new List<Token>(parser.iteratorToken(stringReader));

        foreach (var t in result)
        {
            System.Diagnostics.Debug.WriteLine(t.ToString());
        }
    }
}
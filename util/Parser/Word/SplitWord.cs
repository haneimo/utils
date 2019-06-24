// SkipWord
/* 
 csc /t:exe SplitWord.cs /r:.\ISpecialWord.dll /r:.\AheadTextReader.dll /r:.\SimpleTokenParser.dll /r:.\Token.dll
 */

namespace Parser.Word
{
    public class SplitWord : ASpecialWord
    {
        private string word;
        private const string TOKEN_TYPE_SPLIT = "TOKEN_TYPE_SPLIT";

        public SplitWord(string word)
        {
            this.word = word;
        }

        public override string GetMatchingWord()
        {
            return word;
        }

        public override Token Parse(AheadTextReader source)
        {
            if (source.MatchForward(word))
            {
                return new Token(TOKEN_TYPE_SPLIT, source.PopForward(word.Length));
            }
            else
            {
                return Token.EMPTY_TOKEN;
            }
        }

        public override int CalcurateWordLength()
        {
            return word.Length;
        }


    }

}
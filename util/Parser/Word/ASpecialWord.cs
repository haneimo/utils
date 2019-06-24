using System;

/*
  csc /t:library .\ASpecialWord.cs /r:.\AheadTextReader.dll /r:./Token.dll
*/

namespace Parser.Word
{

    public abstract class ASpecialWord
    {

        public abstract string GetMatchingWord();

        public virtual bool Matching(AheadTextReader source)
        {
            return source.MatchForward(GetMatchingWord());
        }

        public abstract Token Parse(AheadTextReader source);
        public abstract int CalcurateWordLength();
    }
}
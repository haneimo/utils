using System;

/*
  csc /t:library .\ISpecialWord.cs /r:.\AheadTextReader.dll /r:./Token.dll
*/

public abstract class ASpecialWord{

  public abstract string GetMatchingWord();

  public virtual bool Matching( AheadTextReader source ) {
     return source.MatchForward( GetMatchingWord() );
  }

  public abstract Token Parse( AheadTextReader source );
  public abstract int CalcurateWordLength();
}
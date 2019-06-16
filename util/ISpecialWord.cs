using System;

/*
  csc /t:library .\ISpecialWord.cs /r:.\AheadTextReader.dll /r:./Token.dll
*/

public abstract class ISpecialWord{

  public abstract string GetMatchingWord();

  public bool  Matching( AheadTextReader source ) {
     return aheadReader.MatchForward( spWord.GetMatchingWord() );
  }

  public abstract Token Parse( AheadTextReader source );
  public abstract int CalcurateWordLength();
}
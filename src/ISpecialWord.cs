using System;

/*
  csc /t:library .\ISpecialWord.cs /r:.\AheadTextReader.dll /r:./Token.dll
*/

// “ÁŽê’PŒêInterFace
public interface ISpecialWord{
  string GetMatchingWord();
  Token Parse( AheadTextReader source );
  int CalcurateWordLength();
}
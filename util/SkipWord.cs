// SkipWord
/* 
 csc /t:exe SkipWord.cs /r:.\ISpecialWord.dll /r:.\AheadTextReader.dll /r:.\SimpleTokenParser.dll /r:.\Token.dll
 */

public class SkipWord : ASpecialWord {
  private string TOKEN_TYPE_SKIP = "TOKEN_TYPE_SKIP";
  private string word;
  
  
  public SkipWord( string word ){
    this.word = word;
  }
  
   public override string GetMatchingWord(){
    return word;
  }

   public override Token Parse( AheadTextReader source ){
    if( source.MatchForward( word ) ) {
      var w = source.PopForward( word.Length );
      return new Token( TOKEN_TYPE_SKIP, w );
    } else {
      return Token.EMPTY_TOKEN;
    }
  }

   public override int CalcurateWordLength(){
    return word.Length;
  }


}


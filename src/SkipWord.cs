// SkipWord
/* 
 csc /t:exe SkipWord.cs /r:.\ISpecialWord.dll /r:.\AheadTextReader.dll /r:.\SimpleTokenParser.dll /r:.\Token.dll
 */

public class SkipWord : ISpecialWord {
  private string TOKEN_TYPE_SKIP = "TOKEN_TYPE_SKIP";
  private string word;
  
  
  public SkipWord( string word ){
    this.word = word;
  }
  
  public string GetMatchingWord(){
    return word;
  }
  
  public Token Parse( AheadTextReader source ){
    if( source.MatchForward( word ) ) {
      var w = source.PopForward( word.Length );
      return new Token( TOKEN_TYPE_SKIP, w );
    } else {
      return Token.EMPTY_TOKEN;
    }
  }

  public int CalcurateWordLength(){
    return word.Length;
  }

  public static void Main(string[] args){
    var parser = new SimpleTokenParser();
    parser.AddSpecialWord( new SkipWord(" ") );
    parser.AddSpecialWord( new SkipWord("\n") );
    
    foreach( var token in parser.iteratorToken( System.Console.In ) ){
      System.Console.WriteLine( token );
    }
  }
}


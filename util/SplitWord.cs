// SkipWord
/* 
 csc /t:exe SplitWord.cs /r:.\ISpecialWord.dll /r:.\AheadTextReader.dll /r:.\SimpleTokenParser.dll /r:.\Token.dll
 */

public class SplitWord : ASpecialWord {
  private string word;
  private const string TOKEN_TYPE_SPLIT = "TOKEN_TYPE_SPLIT";
  
  public SplitWord( string word ){
    this.word = word;
  }
  
  public override string GetMatchingWord(){
    return word;
  }
  
  public override Token Parse( AheadTextReader source ){
    if( source.MatchForward( word ) ) {
      return new Token( TOKEN_TYPE_SPLIT, source.PopForward( word.Length ));
    } else {
      return Token.EMPTY_TOKEN;
    }
  }
  
  public override int CalcurateWordLength(){
    return word.Length;
  }

  public static void Main(string[] args){
    var parser = new SimpleTokenParser();
    parser.AddSpecialWord( new SplitWord("select") );
    parser.AddSpecialWord( new SplitWord("from") );
    parser.AddSpecialWord( new SplitWord("where") );
    
    foreach( var token in parser.iteratorToken( System.Console.In ) ){
      System.Console.WriteLine( token );
    }
  }
}


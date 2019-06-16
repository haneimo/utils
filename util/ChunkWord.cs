using System;

/* 
 csc /t:exe ChunkWord.cs /r:.\ISpecialWord.dll /r:.\AheadTextReader.dll /r:.\SimpleTokenParser.dll /r:.\Token.dll
 */

public class ChunkWord : ASpecialWord {
  private string startWord;
  private string endWord;
  private string escapeWord;
  private const string TOKEN_TYPE_CHUNK = "TOKEN_TYPE_CHUNK";
  
  private int chunkSizeLimit;
  private const int DEFAULT_CHUNK_SIZE_LIMIT = 1000000;
  private const string DEFAULT_ESCAPEWORD = "";

  public ChunkWord( string startWord, string endWord):this( startWord, endWord, DEFAULT_ESCAPEWORD ){}
  public ChunkWord( string startWord, string endWord, string escapeWord ):this( startWord, endWord, escapeWord, DEFAULT_CHUNK_SIZE_LIMIT ){}
  
  public ChunkWord( string startWord, string endWord, string escapeWord, int chunkSizeLimit){
    this.startWord = startWord;
    this.endWord = endWord;
    this.chunkSizeLimit = chunkSizeLimit;
    this.escapeWord = escapeWord;
  }
  
  public override string GetMatchingWord(){
    return startWord;
  }
  
  public override int CalcurateWordLength(){
    return Math.Max( Math.Max( startWord.Length, endWord.Length), escapeWord.Length );
  }
  
  public override Token Parse( AheadTextReader source ){
    if( source.MatchForward( startWord ) ) {
      var buff = source.PopForward( startWord.Length );
      while( !source.MatchForward( endWord ) || source.MatchForward( escapeWord ) ){
        if( buff.Length >= chunkSizeLimit ){
          throw new Exception("");
        }
        
        if( source.MatchForward( escapeWord ) ){
          buff += source.PopForward( escapeWord.Length );
        }else{
          buff += source.PopForward( 1 );
        }
        
      }
      return new Token( TOKEN_TYPE_CHUNK, buff + source.PopForward( endWord.Length ) );
      
    } else {
      return Token.EMPTY_TOKEN;
    }
  }

  public static void Main(string[] args){
    var parser = new SimpleTokenParser();
    parser.AddSpecialWord( new ChunkWord("\"","\"", "\\\"") );
    parser.AddSpecialWord( new ChunkWord("\'", "\'", "\\\'") );
    parser.AddSpecialWord( new ChunkWord("/*", "*/") );
    parser.AddSpecialWord( new ChunkWord("//", "\n") );
    
    foreach( var token in parser.iteratorToken( System.Console.In ) ){
      System.Console.WriteLine( token );
    }
  }
}


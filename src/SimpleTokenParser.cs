using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

/* 
  csc /t:library .\SimpleTokenParser.cs /r:.\AheadTextReader.dll /r:.\ISpecialWord.dll /r:.\Token.dll
*/

public class SimpleTokenParser {
  private List<ISpecialWord> specialWords = new List<ISpecialWord>();
  private const string TOKEN_TYPE_NORMAL = "TOKEN_TYPE_NORMAL";
  
  public void AddSpecialWord( ISpecialWord specialWord ){
    specialWords.Add( specialWord );
  }
  
  public IEnumerable<string> iteratorTokenString( TextReader source ){
    foreach( var token in iteratorToken(source) ){
      yield return token.Body;
    }
  }
  
  public IEnumerable<Token> iteratorToken( TextReader source ){
    if( specialWords.Count() == 0 ){
      throw new Exception("SpecialWordが登録されていないため、解析を行うことが出来ません。AddSpecialWordメソッドで登録してからiteratorTokenを呼び出して下さい。");
    }
    
    var aheadReader = new AheadTextReader( source, countMaxBufferSize() );
    var normalToken = "";
    
    while( !aheadReader.AtEndOfStream ){
      bool matched  = false;
      foreach( var spWord in specialWords ){
      
        if( aheadReader.MatchForward( spWord.GetMatchingWord() ) ){
          matched = true;
          
          if( normalToken != ""){ 
            yield return new Token( TOKEN_TYPE_NORMAL, normalToken );
            normalToken = "";
          }
          var token = spWord.Parse( aheadReader );
          if( !token.IsEmpty() ){
            yield return token;
          }
          
          break;
        }
      }
      
      if( !matched ){
        normalToken += aheadReader.PopForward(1);
      }
    }
  }
  
  private int countMaxBufferSize(){
     return specialWords.Aggregate( 0, ( continueNum, item ) =>  continueNum > item.CalcurateWordLength() ? continueNum : item.CalcurateWordLength() );
  }
}
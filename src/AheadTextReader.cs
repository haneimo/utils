using System;
using System.IO;

public class AheadTextReader{
  private TextReader reader;
  private int aheadCount;
  private string _aheadBuffer = "";
  public string AheadBuffer { get{ return _aheadBuffer; } }
  private bool _atEndOfStream = false;
  public bool AtEndOfStream { get{ return _atEndOfStream; } }
  
  public AheadTextReader( TextReader sourceReader, int aheadCount ){
    reader = sourceReader;
    this.aheadCount = aheadCount;
    
    FillAheadBuffer();
  }
  
  public string PopForward( int charCount ){
    if( aheadCount < charCount ){
      throw new Exception("バッファより大きい文字列をPopForwardしようとしたため、エラーとなりました。");
    }
    
    string resultValue;
    if( _aheadBuffer.Length >= charCount ){
      resultValue  = _aheadBuffer.Substring( 0, charCount );
      _aheadBuffer = _aheadBuffer.Substring( charCount, _aheadBuffer.Length - charCount );
    } else {
      resultValue  = _aheadBuffer;
      _aheadBuffer = "";
    }
    
    FillAheadBuffer();
    
    return resultValue;
  }
  
  
  public bool MatchForward( string targetWord ){
    if( _aheadBuffer.Length < targetWord.Length ){
      return false;
    }
    
    if( _aheadBuffer.Substring(0, targetWord.Length ) == targetWord ){
      return true;
    } else {
      return false;
    }
  }
  
  
  private void FillAheadBuffer(){
    char[] readChars = new Char[ aheadCount - _aheadBuffer.Length ];
    var result = reader.Read( readChars, 0, readChars.Length );
    if( result != 0 ){
      _aheadBuffer = _aheadBuffer + new String( readChars );
    } else {
      if( _aheadBuffer.Length == 0 ){
        _atEndOfStream = true;
      }
    }
  }
  
  public static void Main(){
    var reader = new AheadTextReader(System.Console.In, 8);
    string popWord;
    while( !reader.AtEndOfStream ){
      
      if( reader.MatchForward("end") ) break;
      
      
      if( reader.MatchForward("hello") ){
        popWord = reader.PopForward(5);
      } else {
        popWord = reader.PopForward(1);
      }
      
      System.Console.WriteLine( "Popword:" + popWord + ", Buffer:" + reader.AheadBuffer );
    }
  }
}

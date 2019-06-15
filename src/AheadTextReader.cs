using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

/*
 $env:PATH += "C:\Windows\Microsoft.NET\Framework64\v4.0..."(最後の...は環境によって書き換え。)
 csc /t:library AheadTextReader.cs 
 csc /t:exe AheadTextReader.cs  
*/

public class AheadTextReader{
  static Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
  private TextReader reader;
  private int aheadCount;
  private string _aheadBuffer = "";
    
  public string AheadBuffer { get{ return _aheadBuffer; } }
  private bool _atEndOfStream = false;
  public bool AtEndOfStream { get{ return _atEndOfStream; } }
  private List<int> splitHalfWidthIndexes = new List<int>();
  private int bufferHeadWidthCount;
  public int BufferHeadWidthCount{ get{return bufferHeadWidthCount;} }
  public AheadTextReader( TextReader sourceReader, int aheadCount ){
      reader = sourceReader;
      this.aheadCount = aheadCount;    
      FillAheadBuffer();
  }
  
  public void addSplitIndexWithHalfWidh( int splitIndex ){
    splitHalfWidthIndexes.Add(splitIndex);
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
    
    UpdateBufferHeadWidthCount( resultValue );
    FillAheadBuffer();
    
    return resultValue;
  }
  
  private void UpdateBufferHeadWidthCount( string s ){
    var lines = s.Split('\n');
    if( lines.Count() == 1){
      bufferHeadWidthCount = bufferHeadWidthCount + sjisEnc.GetByteCount( lines[0] );
    }else{
      bufferHeadWidthCount = sjisEnc.GetByteCount( lines[ lines.Count() -1 ] );
    }
  }
  
  public bool MatchForward( string targetWord ){
   string validBuffer;
   if( splitHalfWidthIndexes.Any() ){
     if(  splitHalfWidthIndexes.Max() <= bufferHeadWidthCount ){
       validBuffer = _aheadBuffer;
     }else{
       int recentIndex = splitHalfWidthIndexes.Where( a => bufferHeadWidthCount < a ).Min();
       if( recentIndex - bufferHeadWidthCount <= aheadCount ){
         validBuffer = _aheadBuffer.Substring(0,recentIndex - bufferHeadWidthCount);    
       }else{
         validBuffer = _aheadBuffer;
       }
     }
   }else{
     validBuffer = _aheadBuffer;
   }


    if( targetWord == "" ){
      return false;
    }
  
    if( validBuffer.Length < targetWord.Length ){
      return false;
    }
    
    if( validBuffer.Substring(0, targetWord.Length ) == targetWord ){
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
    reader.addSplitIndexWithHalfWidh(0);
    reader.addSplitIndexWithHalfWidh(6);
    reader.addSplitIndexWithHalfWidh(7);
    reader.addSplitIndexWithHalfWidh(73);

    string popWord;
    while( !reader.AtEndOfStream ){
      if( reader.MatchForward("end") ) break;
      if( reader.MatchForward("hello") ){
        popWord = reader.PopForward(5);
      } else {
        popWord = reader.PopForward(1);
      }    
      System.Console.WriteLine( "Popword:" + popWord + ", Buffer:" + reader.AheadBuffer +  ", BufferHeadWidthCount" + reader.BufferHeadWidthCount );
    }
  }
}
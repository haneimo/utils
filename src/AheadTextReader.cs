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
  private int startHalfWidthIndex;
  private int outHalfWidthIndex;
  
  private bool readArea = false;
  
  private int lineCount = 0;
  public int LineCount{ get{ return lineCount; } }
  private int widthCount = 0;
  public int WidthCount{ get{ return widthCount;} }
  
  public string AheadBuffer { get{ return _aheadBuffer; } }
  private bool _atEndOfStream = false;
  public bool AtEndOfStream { get{ return _atEndOfStream; } }
  private List<int> splitHalfWidthIndexes = new List<int>();
  private int bufferHeadWidthCount;
  public int BufferHeadWidthCount{ get{return bufferHeadWidthCount;} }
  public AheadTextReader( TextReader sourceReader, int aheadCount ):this( sourceReader, aheadCount, 0, Int32.MaxValue ){}
  
  public AheadTextReader( TextReader sourceReader, int aheadCount, int startHalfWidthIndex, int outHalfWidthIndex ){
    reader = sourceReader;
    this.aheadCount = aheadCount;
    
    setReadPosition( startHalfWidthIndex, outHalfWidthIndex );
    FillAheadBuffer();
  }
  
  public void addSplitIndexWithHalfWidh( int splitIndex ){
    splitHalfWidthIndexes.Add(splitIndex);
  }

  public void setReadPosition( int startHalfWidthIndex, int outHalfWidthIndex ){
    if( lineCount == 0 && widthCount == 0 ){
      this.startHalfWidthIndex = startHalfWidthIndex;
      this.outHalfWidthIndex = outHalfWidthIndex;
      
      if( startHalfWidthIndex == 0 ){
        readArea = true;
      }else{
        readArea = false;
      }
    }else{
      throw new Exception("ソースコード解析中はリードポジションを変更できません。現在 "+lineCount +"行"+widthCount+"文字目" );
    }
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
  
  
  private int readWithPosition( char[] buffer, int startIndex, int maxLength ){
    int bufferIndex = startIndex;
    for( int i = startIndex; i < maxLength || bufferIndex < maxLength; i++ ){
      int readChar = reader.Read();
      if( readChar == -1 ){
        return bufferIndex - startIndex;
      }else{
        char[] buff = new Char[1];
        buff[0] = (char)readChar;
        string readCharStr = new String( buff );
        int readCharWidthSize = isHalfWidthChar( readCharStr ) ? 1 : 2;
        
        if( readChar == '\n' ){
          if( startHalfWidthIndex == 0 ){
            readArea = true;
          } else {
            readArea = false;
          }
          
          widthCount = 0;
          lineCount++;
          
          buffer[bufferIndex] = (char)readChar;
          bufferIndex++;
        }else{
          if(startHalfWidthIndex <= (widthCount) && (widthCount) < outHalfWidthIndex ){
            if( !readArea ){
              if( startHalfWidthIndex != widthCount ){
                throw new Exception("読み取り開始位置：" + startHalfWidthIndex + "に対して、開始位置を跨いでマルチバイト文字が読み込まれました。対象ストリームは読み込めません。行:" + lineCount + ", 文字位置:" + widthCount);
              }
              readArea = true;
            }
            buffer[bufferIndex] = (char)readChar;
            bufferIndex++;
          }else{
            if( readArea ){
              if( outHalfWidthIndex != widthCount ){
                throw new Exception("読み取り終了位置：" + startHalfWidthIndex + "に対して、終了位置を跨いでマルチバイト文字が読み込まれました。対象ストリームは読み込めません。行:" + lineCount + ", 文字位置:" + widthCount);
              }
              readArea = false;
            }
          }        
          widthCount += readCharWidthSize;
        }
        
      }
    }
    return bufferIndex - startIndex;
    
  }
  
  private bool isHalfWidthChar(string str){
    int num = sjisEnc.GetByteCount(str);
    return num == str.Length;
  }

  private void FillAheadBuffer(){
    char[] readChars = new Char[ aheadCount - _aheadBuffer.Length ];
    var result = readWithPosition( readChars, 0, readChars.Length );
    if( result != 0 ){
      _aheadBuffer = _aheadBuffer + new String( readChars );
    } else {
      if( _aheadBuffer.Length == 0 ){
        _atEndOfStream = true;
      }
    }
  }
  
  public static void Main(){
    //var COBOL_START_INDEX = 6;
    //var COBOL_OUT_INDEX = 72;
    
    //var reader = new AheadTextReader(System.Console.In, 8, COBOL_START_INDEX, COBOL_OUT_INDEX);
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
      
      System.Console.WriteLine( "Popword:" + popWord + ", Buffer:" + reader.AheadBuffer +  ", LineCount:" + reader.LineCount + ", WidthCout:" +reader.WidthCount +", BufferHeadWidthCount" +reader.BufferHeadWidthCount );
    }
  }
}
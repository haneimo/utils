using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Util.Exceptions;

/*
 $env:PATH += "C:\Windows\Microsoft.NET\Framework64\v4.0..."(最後の...は環境によって書き換え。)
 csc /t:library AheadTextReader.cs 
 csc /t:exe AheadTextReader.cs  
*/

public class AheadTextReader
{
    static Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
    private TextReader reader;
    private int aheadCount;
    private string _aheadBuffer = "";

    public string AheadBuffer { get { return _aheadBuffer; } }
    private bool _atEndOfStream = false;
    public bool AtEndOfStream { get { return _atEndOfStream; } }
    private HashSet<int> splitHalfWidthIndexes = new HashSet<int>();
    private int bufferHeadWidthCount;
    public int BufferHeadPosition { get { return bufferHeadWidthCount; } }
    public int lineCount;

    public bool IsNextReadingOnSplitArea(int length)
    {
        return splitHalfWidthIndexes.Where(i => bufferHeadWidthCount <= i && i <= bufferHeadWidthCount + length).Count() > 0;
    }

    public int LinePositoin { get { return lineCount; } }

    public AheadTextReader(TextReader sourceReader, int aheadCount)
    {
        if (aheadCount <= 0)
        {
            throw new InvalidBufferSizeException(aheadCount);
        }

        bufferHeadWidthCount = 0;
        lineCount = 0;
        reader = sourceReader;
        this.aheadCount = aheadCount;
        FillAheadBuffer();
    }

    public void addSplitIndexWithHalfWidh(int splitIndex)
    {
        splitHalfWidthIndexes.Add(splitIndex);
    }

    public string PopForward(int charCount)
    {
        if (aheadCount < charCount)
        {
            throw new Exception("バッファより大きい文字列をPopForwardしようとしたため、エラーとなりました。");
        }

        string resultValue;
        if (_aheadBuffer.Length >= charCount)
        {
            resultValue = _aheadBuffer.Substring(0, charCount);
            _aheadBuffer = _aheadBuffer.Substring(charCount, _aheadBuffer.Length - charCount);
        }
        else
        {
            resultValue = _aheadBuffer;
            _aheadBuffer = "";
        }

        UpdateBufferHeadWidthCount(resultValue);
        FillAheadBuffer();

        return resultValue;
    }

    private void UpdateBufferHeadWidthCount(string s)
    {
        var l = s.Split('\n');
        if (l.Count() == 1)
        {
            bufferHeadWidthCount = bufferHeadWidthCount + sjisEnc.GetByteCount(l[0]);
        }
        else
        {
            bufferHeadWidthCount = sjisEnc.GetByteCount(l[l.Count() - 1]);
            lineCount += l.Count() - 1;
        }
    }

    public bool MatchForward(string targetWord)
    {
        string validBuffer;
        if (splitHalfWidthIndexes.Any())
        {
            if (splitHalfWidthIndexes.Max() <= bufferHeadWidthCount)
            {
                validBuffer = _aheadBuffer;
            }
            else
            {
                int recentIndex = splitHalfWidthIndexes.Where(a => bufferHeadWidthCount < a).Min();
                if (recentIndex - bufferHeadWidthCount <= aheadCount)
                {
                    validBuffer = _aheadBuffer.Substring(0, recentIndex - bufferHeadWidthCount);
                }
                else
                {
                    validBuffer = _aheadBuffer;
                }
            }
        }
        else
        {
            validBuffer = _aheadBuffer;
        }


        if (targetWord == "")
        {
            return false;
        }

        if (validBuffer.Length < targetWord.Length)
        {
            return false;
        }

        if (validBuffer.Substring(0, targetWord.Length) == targetWord)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void FillAheadBuffer()
    {
        char[] readChars = new Char[aheadCount - _aheadBuffer.Length];
        var result = reader.Read(readChars, 0, readChars.Length);
        if (result != 0)
        {
            _aheadBuffer = _aheadBuffer + new String(readChars);
        }
        else
        {
            if (_aheadBuffer.Length == 0)
            {
                _atEndOfStream = true;
            }
        }
    }
}
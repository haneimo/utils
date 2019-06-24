using System;

/* 
 csc /t:exe ChunkWord.cs /r:.\ISpecialWord.dll /r:.\AheadTextReader.dll /r:.\SimpleTokenParser.dll /r:.\Token.dll
 */

namespace Parser.Word
{

    public class ChunkWord : ASpecialWord
    {
        private string startWord;
        private string endWord;
        private string escapeWord;
        private const string TOKEN_TYPE_CHUNK = "TOKEN_TYPE_CHUNK";

        private int chunkSizeLimit;
        private const int DEFAULT_CHUNK_SIZE_LIMIT = 1000000;
        private const string DEFAULT_ESCAPEWORD = "";

        public ChunkWord(string startWord, string endWord) : this(startWord, endWord, DEFAULT_ESCAPEWORD) { }
        public ChunkWord(string startWord, string endWord, string escapeWord) : this(startWord, endWord, escapeWord, DEFAULT_CHUNK_SIZE_LIMIT) { }

        public ChunkWord(string startWord, string endWord, string escapeWord, int chunkSizeLimit)
        {
            this.startWord = startWord;
            this.endWord = endWord;
            this.chunkSizeLimit = chunkSizeLimit;
            this.escapeWord = escapeWord;
        }

        public override string GetMatchingWord()
        {
            return startWord;
        }

        public override int CalcurateWordLength()
        {
            return Math.Max(Math.Max(startWord.Length, endWord.Length), escapeWord.Length);
        }

        public override Token Parse(AheadTextReader source)
        {
            if (source.MatchForward(startWord))
            {
                var buff = source.PopForward(startWord.Length);
                while (!source.MatchForward(endWord) || source.MatchForward(escapeWord))
                {
                    if (buff.Length >= chunkSizeLimit)
                    {
                        throw new Exception("");
                    }

                    if (source.MatchForward(escapeWord))
                    {
                        if (source.IsNextReadingOnSplitArea(escapeWord.Length))
                        {
                            throw new Exception("次回読込で境界をまたいでしまうため、例外を送出します。半角固定文字のワードを設定した場合、半角文字の境界を越えてChunkWordを抽出することはできません。");
                        }

                        buff += source.PopForward(escapeWord.Length);
                    }
                    else
                    {
                        if (source.IsNextReadingOnSplitArea(endWord.Length))
                        {
                            throw new Exception("次回読込で境界をまたいでしまうため、例外を送出します。半角固定文字のワードを設定した場合、半角文字の境界を越えてChunkWordを抽出することはできません。");
                        }

                        buff += source.PopForward(1);
                    }

                }
                return new Token(TOKEN_TYPE_CHUNK, buff + source.PopForward(endWord.Length));

            }
            else
            {
                return Token.EMPTY_TOKEN;
            }
        }


    }

}
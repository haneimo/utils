using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Exceptions
{
    public class InvalidBufferSizeException : Exception
    {
        public InvalidBufferSizeException(int size) :base( buildMessage(size) )
        {
        }

        private static string buildMessage(int size)
        {
            return "バッファサイズに不正な数値が指定されました。(指定数値＝" + size + ")";
        }
    }
}

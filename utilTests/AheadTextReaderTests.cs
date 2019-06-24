using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;
using Parser.Exceptions;

namespace Tests
{
    [TestClass()]
    public class AheadTextReaderTests
    {
        [TestMethod()]
        public void AheadCountにゼロを指定した場合例外がスローされること()
        {
            bool wasThrowingException = false;
            try
            {
                var reader = new AheadTextReader(new StringReader("dummy"), 0);
            }
            catch (InvalidBufferSizeException e)
            {
                wasThrowingException = true;
            }

            if (!wasThrowingException)
            {
                Assert.Fail();
            }
        }

        //[TestMethod()]
        //public void addSplitIndexWithHalfWidhTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void PopForwardTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void MatchForwardTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void MainTest()
        //{
        //    Assert.Fail();
        //}
        //public static void Main()
        //{
        //    var reader = new AheadTextReader(System.Console.In, 8);
        //    reader.addSplitIndexWithHalfWidh(0);
        //    reader.addSplitIndexWithHalfWidh(6);
        //    reader.addSplitIndexWithHalfWidh(7);
        //    reader.addSplitIndexWithHalfWidh(73);

        //    string popWord;
        //    while (!reader.AtEndOfStream)
        //    {
        //        if (reader.MatchForward("end")) break;
        //        if (reader.MatchForward("hello"))
        //        {
        //            popWord = reader.PopForward(5);
        //        }
        //        else
        //        {
        //            popWord = reader.PopForward(1);
        //        }
        //        System.Console.WriteLine("Popword:" + popWord + ", Buffer:" + reader.AheadBuffer + ", BufferHeadPosition" + reader.BufferHeadPosition);
        //    }
        //}
    }
}

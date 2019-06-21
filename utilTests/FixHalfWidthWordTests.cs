using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tests
{
    [TestClass()]
    public class FixHalfWidthWordTests
    {
        //[TestMethod()]
        //public void FixHalfWidthWordTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void GetMatchingWordTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void MatchingTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void ParseTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void CalcurateWordLengthTest()
        //{
        //    Assert.Fail();
        //}

        [TestMethod()]
        public void MainTest()
        {
                var parser = new SimpleTokenParser();
                parser.AddFixHalfWidthWord(new FixHalfWidthWord(0, 6));
                parser.AddFixHalfWidthWord(new FixHalfWidthWord(6, 1));
                parser.AddFixHalfWidthWord(new FixHalfWidthWord(16, Int32.MaxValue));

                var stringReader = new StringReader("999999*aaa      comment" + "\n" + "111111 aiueoc");
                var result = new  List<Token>(parser.iteratorToken(stringReader));

            Assert.AreEqual(result[0].ToString(), "999999");
                
        }
    }
}
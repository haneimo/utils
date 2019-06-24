using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Parser;
using Parser.Word;

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
        public void コボル風の文字FixHalfWidthWord機能にて区切ることができること()
        {
            var parser = new SimpleTokenParser();
            parser.AddFixHalfWidthWord(new FixHalfWidthWord(0, 6));
            parser.AddFixHalfWidthWord(new FixHalfWidthWord(6, 1));
            parser.AddFixHalfWidthWord(new FixHalfWidthWord(16, Int32.MaxValue));

            var stringReader = new StringReader("999999*zxy   cbacomment" + "\n" + "111111 aiueoc");
            var result = new List<Token>(parser.iteratorToken(stringReader));

            Assert.AreEqual<string>(result[0].Body, "999999");
            Assert.AreEqual<string>(result[1].Body, "*");
            Assert.AreEqual<string>(result[2].Body, "zxy   cba");
            Assert.AreEqual<string>(result[3].Body, "comment");
            Assert.AreEqual<string>(result[4].Body, "\n");
            Assert.AreEqual<string>(result[5].Body, "111111");
            Assert.AreEqual<string>(result[6].Body, " ");
            Assert.AreEqual<string>(result[7].Body, "aiueoc");
        }
    }
}
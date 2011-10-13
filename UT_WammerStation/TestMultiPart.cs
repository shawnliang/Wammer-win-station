using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UT_WammerStation
{
    [TestClass]
    public class TestMultiPart
    {
        [TestMethod]
        //[Ignore]
        public void TestSimple()
        {
            
            FileStream f = new FileStream("SimpleMultiPartContent.txt", FileMode.Open);
            Wammer.MultiPart.Parser parser = new Wammer.MultiPart.Parser("simple boundary");
            Wammer.MultiPart.Part[] parts = parser.Parse(f);
            Assert.AreEqual(22, parts.Length);
            Assert.AreEqual(
                "This is implicitly typed plain ASCII text.\r\n" +
                "It does NOT end with a linebreak.",
                parts[0].Text);

            Assert.AreEqual(
                "This is implicitly typed plain ASCII text.\r\n" +
                "It DOES end with a linebreak.\r\n",
                parts[0].Text);
        }
    }
}

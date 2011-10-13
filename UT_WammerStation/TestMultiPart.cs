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
        public void TestSimple()
        {
            
            FileStream f = new FileStream("SimpleMultiPartContent.txt", FileMode.Open);
            Wammer.MultiPart.Parser parser = new Wammer.MultiPart.Parser("simple boundary");
            Wammer.MultiPart.Part[] parts = parser.Parse(f);
            Assert.AreEqual(2, parts.Length);
            Assert.AreEqual(
                "This is implicitly typed plain ASCII text.\r\n" +
                "It does NOT end with a linebreak.",
                parts[0].Text);

            Assert.AreEqual(
                "This is implicitly typed plain ASCII text.\r\n" +
                "It DOES end with a linebreak.\r\n",
                parts[1].Text);
        }

        [TestMethod]
        public void TestSingle()
        {
            FileStream f = new FileStream("SingeMultiPart.txt", FileMode.Open);
            Wammer.MultiPart.Parser parser = new Wammer.MultiPart.Parser("simple boundary");
            Wammer.MultiPart.Part[] parts = parser.Parse(f);
            Assert.AreEqual(1, parts.Length);
            Assert.AreEqual(
                "This is implicitly typed plain ASCII text.\r\n" +
                "It does NOT end with a linebreak.",
                parts[0].Text);
        }

        [TestMethod]
        public void TestSimpleWithHeaders()
        {
            FileStream f = new FileStream("SimpleWithHeaders.txt", FileMode.Open);
            Wammer.MultiPart.Parser parser = new Wammer.MultiPart.Parser("simple boundary");
            Wammer.MultiPart.Part[] parts = parser.Parse(f);
            Assert.AreEqual(2, parts.Length);
            Assert.AreEqual(
                "This is implicitly typed plain ASCII text.\r\n" +
                "It does NOT end with a linebreak.",
                parts[0].Text);
            Assert.AreEqual("value 1", parts[0].Headers["Header1"]);
            Assert.AreEqual("value 2", parts[0].Headers["Header2"]);

            Assert.AreEqual(
                "This is implicitly typed plain ASCII text.\r\n" +
                "It DOES end with a linebreak.\r\n",
                parts[1].Text);
            Assert.AreEqual("value a", parts[1].Headers["Header1"]);
            Assert.AreEqual("value b", parts[1].Headers["Header2"]);
        }
    }
}

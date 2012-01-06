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

			FileStream f = new FileStream("SimpleMultiPartContent.txt",
																FileMode.Open);
			Wammer.MultiPart.Parser parser =
								new Wammer.MultiPart.Parser("simple boundary");
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
			using (FileStream f = new FileStream("SingeMultiPart.txt", FileMode.Open))
			{
				Wammer.MultiPart.Parser parser =
								   new Wammer.MultiPart.Parser("simple boundary");
				Wammer.MultiPart.Part[] parts = parser.Parse(f);
				Assert.AreEqual(1, parts.Length);
				Assert.AreEqual(
					"This is implicitly typed plain ASCII text.\r\n" +
					"It does NOT end with a linebreak.",
					parts[0].Text);
			}
		}

		[TestMethod]
		public void TestSimpleWithHeaders()
		{
			FileStream f = new FileStream("SimpleWithHeaders.txt",
																FileMode.Open);
			Wammer.MultiPart.Parser parser =
								new Wammer.MultiPart.Parser("simple boundary");
			Wammer.MultiPart.Part[] parts = parser.Parse(f);
			Assert.AreEqual(2, parts.Length);
			Assert.AreEqual(
				"This is implicitly typed plain ASCII text.\r\n" +
				"It does NOT end with a linebreak.",
				parts[0].Text);

			// headers are case insesitive
			Assert.AreEqual("value 1", parts[0].Headers["header1"]);
			Assert.AreEqual("value 2", parts[0].Headers["header2"]);
			Assert.AreEqual("value 1", parts[0].Headers["Header1"]);
			Assert.AreEqual("value 2", parts[0].Headers["Header2"]);

			Assert.AreEqual(
				"This is implicitly typed plain ASCII text.\r\n" +
				"It DOES end with a linebreak.\r\n",
				parts[1].Text);

			// headers are case insesitive
			Assert.AreEqual("value a", parts[1].Headers["header1"]);
			Assert.AreEqual("value b", parts[1].Headers["header2"]);
			Assert.AreEqual("value a", parts[1].Headers["heaDEr1"]);
			Assert.AreEqual("value b", parts[1].Headers["hEader2"]);
		}

		[TestMethod]
		public void TestBinaryContent()
		{
			FileStream f = new FileStream("BinaryMultiPart.dat", FileMode.Open);
			Wammer.MultiPart.Parser parser =
								new Wammer.MultiPart.Parser("simple boundry");
			Wammer.MultiPart.Part[] parts = parser.Parse(f);
			Assert.AreEqual(1, parts.Length);

			Assert.AreEqual("binary",
								parts[0].Headers["content-transfer-encoding"]);
			for (int i = 0; i < 20; i++)
			{
				ArraySegment<byte> bytes = parts[0].Bytes;
				Assert.AreEqual((byte)i, bytes.Array[bytes.Offset + i]);
			}

			Assert.AreEqual(null, parts[0].Text);
			Assert.IsNull(parts[0].ContentDisposition);
		}

		[TestMethod]
		public void TestContentDisposition()
		{
			FileStream f = new FileStream("ContentDisposition.txt", FileMode.Open);
			Wammer.MultiPart.Parser parser =
								new Wammer.MultiPart.Parser("simple boundary");
			Wammer.MultiPart.Part[] parts = parser.Parse(f);
			Assert.AreEqual(3, parts.Length);


			Assert.AreEqual("form-data", parts[0].ContentDisposition.Value);
			Assert.AreEqual("12345.txt", parts[0].ContentDisposition.Parameters["name"]);

			Assert.AreEqual("form-data", parts[1].ContentDisposition.Value);
			Assert.AreEqual("abcde.txt", parts[1].ContentDisposition.Parameters["name"]);

			Assert.AreEqual("form-data", parts[2].ContentDisposition.Value);
			Assert.AreEqual("ABCDE.txt", parts[2].ContentDisposition.Parameters["name"]);
			Assert.AreEqual("kkkkk", parts[2].ContentDisposition.Parameters["name2"]);
		}
	}
}

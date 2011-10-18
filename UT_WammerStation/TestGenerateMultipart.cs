using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Specialized;

using Wammer.MultiPart;

namespace UT_WammerStation
{
	[TestClass]
	public class TestGenerateMultipart
	{
		private byte[] part1;
		private byte[] part2;
		private byte[] part3;

		public TestGenerateMultipart()
		{
			part1 = Encoding.UTF8.GetBytes("1234567890");
			part2 = Encoding.UTF8.GetBytes("abcdefghij");
			part3 = Encoding.UTF8.GetBytes("QAZWSXEDCR");
		}

		[TestMethod]
		public void TestSimple()
		{
			MemoryStream m = new MemoryStream();
			Serializer serializer = new Serializer(m);
			
			serializer.Put(new Part(part1, 0, part1.Length));
			serializer.Put(new Part(part2, 0, part2.Length));
			serializer.Put(new Part(part3, 0, part3.Length));
			serializer.Close();

			m.Position = 0;

			Parser parser = new Parser(serializer.Boundary);
			Part[] parts = parser.Parse(m);
			
			Assert.AreEqual(3, parts.Length);
			Assert.AreEqual(Encoding.UTF8.GetString(part1), parts[0].Text);
			Assert.AreEqual(Encoding.UTF8.GetString(part2), parts[1].Text);
			Assert.AreEqual(Encoding.UTF8.GetString(part3), parts[2].Text);
		}

		[TestMethod]
		public void TestGenerateHeader_ContentDisposition()
		{
			MemoryStream m = new MemoryStream();
			Serializer serializer = new Serializer(m);

			Part part = new Part(part1, 0, part1.Length);
			part.ContentDisposition = new Disposition("form-data");
			part.ContentDisposition.Parameters.Add("name1", "value1");
			part.ContentDisposition.Parameters.Add("name2", "value2");
			serializer.Put(part);
			serializer.Close();

			m.Position = 0;

			Parser parser = new Parser(serializer.Boundary);
			Part[] parts = parser.Parse(m);

			Assert.AreEqual(1, parts.Length);
			Assert.AreEqual(Encoding.UTF8.GetString(part1), parts[0].Text);
			Assert.AreEqual("form-data", parts[0].ContentDisposition.Value);
			Assert.AreEqual("value1", parts[0].ContentDisposition.Parameters["name1"]);
			Assert.AreEqual("value2", parts[0].ContentDisposition.Parameters["name2"]);
		}

		[TestMethod]
		public void TestGenerateHeader_ContentDisposition_ContentType()
		{
			MemoryStream m = new MemoryStream();
			Serializer serializer = new Serializer(m);

			Part part = new Part(part1, 0, part1.Length);
			part.ContentDisposition = new Disposition("form-data");
			part.ContentDisposition.Parameters.Add("name1", "value1");
			part.ContentDisposition.Parameters.Add("name2", "value2");
			part.Headers["Content-Type"] = "image/jpeg";
			serializer.Put(part);
			serializer.Close();

			m.Position = 0;

			Parser parser = new Parser(serializer.Boundary);
			Part[] parts = parser.Parse(m);

			Assert.AreEqual(1, parts.Length);
			Assert.AreEqual(Encoding.UTF8.GetString(part1), parts[0].Text);
			Assert.AreEqual("form-data", parts[0].ContentDisposition.Value);
			Assert.AreEqual("value1", parts[0].ContentDisposition.Parameters["name1"]);
			Assert.AreEqual("value2", parts[0].ContentDisposition.Parameters["name2"]);
			Assert.AreEqual("image/jpeg", parts[0].Headers["content-type"]);
		}

		[TestMethod]
		public void TestGenerateHeader_ContentType()
		{
			MemoryStream m = new MemoryStream();
			Serializer serializer = new Serializer(m);

			Part part = new Part(part1, 0, part1.Length);
			part.Headers["Content-Type"] = "image/jpeg";
			serializer.Put(part);
			serializer.Close();

			m.Position = 0;

			Parser parser = new Parser(serializer.Boundary);
			Part[] parts = parser.Parse(m);

			Assert.AreEqual(1, parts.Length);
			Assert.AreEqual(Encoding.UTF8.GetString(part1), parts[0].Text);
			Assert.AreEqual("image/jpeg", parts[0].Headers["content-type"]);
		}
	}
}

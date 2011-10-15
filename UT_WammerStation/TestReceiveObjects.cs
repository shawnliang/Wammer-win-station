using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wammer.MultiPart;
using Wammer.Station;

namespace UT_WammerStation
{
	[TestClass]
	public class TestReceiveObjects
	{
		byte[] file;
		byte[] filename;
		byte[] filetype;
		byte[] oid;

		Part filePart;
		Part filenamePart;
		Part filetypePart;
		Part oidPart;

		List<Part> parts = new List<Part>();

		private Part CreatePart(byte[] data)
		{
			return new Part(data, 0, data.Length, new NameValueCollection());
		}

		[TestInitialize]
		public void SetUp()
		{
			file = new byte[20];
			for (int i = 0; i < file.Length; i++)
				file[i] = (byte)i;
			filename = Encoding.UTF8.GetBytes("file1.jpg");
			filetype = Encoding.UTF8.GetBytes("100");
			oid = Encoding.UTF8.GetBytes("object_id1");

			filePart = CreatePart(file);
			filenamePart = CreatePart(filename);
			filetypePart = CreatePart(filetype);
			oidPart = CreatePart(oid);

			parts.Clear();
			parts.Add(filePart);
			parts.Add(filenamePart);
			parts.Add(filetypePart);
			parts.Add(oidPart);
		}

		[TestCleanup]
		public void TearDown()
		{
			if (Directory.Exists("resource"))
				Directory.Delete("resource", true);
		}

		[TestMethod]
		public void TestSimple()
		{
			Directory.CreateDirectory(@"resource");
			Directory.CreateDirectory(@"resource\space1");
			Directory.CreateDirectory(@"resource\space1\100");

			FileStorage storage = new FileStorage("resource");
			storage.Save("space1", FileType.ImgOriginal, "id1.jpeg", file);

			using (FileStream f = File.OpenRead(@"resource\space1\100\id1.jpeg"))
			{
				Assert.AreEqual(file.Length, f.Length);
				byte[] savedFile = new byte[file.Length];
				int size = f.Read(savedFile, 0, savedFile.Length);
				Assert.AreEqual(size, f.Length);
				for (int i = 0; i < size; i++)
					Assert.AreEqual(file[i], savedFile[i]);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(DirectoryNotFoundException))]
		public void TestResourcePathHasToBeCreatedInAdvance()
		{
			FileStorage storage = new FileStorage("resource");
			storage.Save("space1", FileType.ImgOriginal, "id1.jpeg", file);
		}

		[TestMethod]
		public void TestAsyncSave()
		{
			Directory.CreateDirectory(@"resource");
			Directory.CreateDirectory(@"resource\space1");
			Directory.CreateDirectory(@"resource\space1\100");

			FileStorage storage = new FileStorage("resource");
			IAsyncResult async = storage.BeginSave("space1", FileType.ImgOriginal, "id1.jpeg", file,
				null, null);

			storage.EndSave(async);

			using (FileStream f = File.OpenRead(@"resource\space1\100\id1.jpeg"))
			{
				Assert.AreEqual(file.Length, f.Length);
				byte[] savedFile = new byte[file.Length];
				int size = f.Read(savedFile, 0, savedFile.Length);
				Assert.AreEqual(size, f.Length);
				for (int i = 0; i < size; i++)
					Assert.AreEqual(file[i], savedFile[i]);
			}
		}
	}
}

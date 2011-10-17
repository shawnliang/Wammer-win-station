using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

using Wammer.MultiPart;
using Wammer.Station;
using Wammer.Cloud;

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
		public void TestFileStorage()
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

		[TestMethod]
		public void TestObjectReceiveHandler()
		{
			Directory.CreateDirectory(@"resource");
			Directory.CreateDirectory(@"resource\space1");
			Directory.CreateDirectory(@"resource\space1\100");

			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/test/", new ObjectUploadHandler());
				server.Start();

				HttpWebRequest requst = (HttpWebRequest)WebRequest.
											Create("http://localhost/test/");
				requst.ContentType = "multipart/form-data; boundary=AaB03x";
				requst.Method = "POST";
				using (FileStream fs = new FileStream("ObjectUpload1.txt", FileMode.Open))
				using (Stream outStream = requst.GetRequestStream())
				{
					fs.CopyTo(outStream);
				}

				HttpWebResponse response = (HttpWebResponse)requst.GetResponse();
				byte[] resData = null;

				using (BinaryReader reader = new BinaryReader(response.GetResponseStream()))
				{
					resData = reader.ReadBytes((int)response.ContentLength);
					Assert.AreEqual(response.ContentLength, resData.Length);
				}

				string responseString = Encoding.UTF8.GetString(resData);
				ObjectUploadResponse res = fastJSON.JSON.Instance.ToObject
										<ObjectUploadResponse>(responseString);

				Assert.AreEqual(200, res.http_status);
				Assert.IsNotNull(res.timestamp);
				Assert.AreEqual(0, res.app_ret_code);
				Assert.AreEqual("Success", res.app_ret_msg);
				Assert.AreEqual("object_id1", res.object_id);


				using (FileStream fs = File.OpenRead(@"resource\space1\100\object_id1.jpeg"))
				using (StreamReader ss = new StreamReader(fs))
				{
					string fileContent = ss.ReadToEnd();
					Assert.AreEqual("1234567890abcdefghij", fileContent);
				}
			}
		}

		[TestMethod]
		public void TestObjectReceiveHandler_ServerError()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/test/", new ObjectUploadHandler());
				server.Start();

				HttpWebRequest requst = (HttpWebRequest)WebRequest.
											Create("http://localhost/test/");
				requst.ContentType = "multipart/form-data; boundary=AaB03x";
				requst.Method = "POST";
				using (FileStream fs = new FileStream("ObjectUpload1.txt", FileMode.Open))
				using (Stream outStream = requst.GetRequestStream())
				{
					fs.CopyTo(outStream);
				}

				HttpWebResponse response = (HttpWebResponse)requst.GetResponse();
				byte[] resData = null;

				using (BinaryReader reader = new BinaryReader(response.GetResponseStream()))
				{
					resData = reader.ReadBytes((int)response.ContentLength);
					Assert.AreEqual(response.ContentLength, resData.Length);
				}

				string responseString = Encoding.UTF8.GetString(resData);
				ObjectUploadResponse res = fastJSON.JSON.Instance.ToObject
										<ObjectUploadResponse>(responseString);

				Assert.AreEqual(200, res.http_status);
				Assert.IsNotNull(res.timestamp);
				Assert.AreNotEqual(0, res.app_ret_code);
				Assert.AreNotEqual("Success", res.app_ret_msg);
				Assert.AreEqual("object_id1", res.object_id);
			}
		}

		[TestMethod]
		public void TestObjectReceiveHandler_ClientError()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/test/", new ObjectUploadHandler());
				server.Start();

				HttpWebRequest requst = (HttpWebRequest)WebRequest.
											Create("http://localhost/test/");
				requst.ContentType = "multipart/form-data; boundary=AaB03x";
				requst.Method = "POST";
				using (FileStream fs = new FileStream("SingeMultiPart.txt", FileMode.Open))
				using (Stream outStream = requst.GetRequestStream())
				{
					fs.CopyTo(outStream);
				}

				HttpWebResponse response = (HttpWebResponse)requst.GetResponse();
				byte[] resData = null;

				using (BinaryReader reader = new BinaryReader(response.GetResponseStream()))
				{
					resData = reader.ReadBytes((int)response.ContentLength);
					Assert.AreEqual(response.ContentLength, resData.Length);
				}

				string responseString = Encoding.UTF8.GetString(resData);
				ObjectUploadResponse res = fastJSON.JSON.Instance.ToObject
										<ObjectUploadResponse>(responseString);

				Assert.AreEqual(200, res.http_status);
				Assert.IsNotNull(res.timestamp);
				Assert.AreNotEqual(0, res.app_ret_code);
				Assert.AreNotEqual("Success", res.app_ret_msg);
				Assert.AreEqual(null, res.object_id);
			}
		}
	}
}

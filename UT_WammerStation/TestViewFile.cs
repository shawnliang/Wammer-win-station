using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station;


namespace UT_WammerStation
{
	[TestClass]
	public class TestViewFile
	{
		[TestInitialize]
		public void setUp()
		{
			if (!Directory.Exists("resource"))
				Directory.CreateDirectory("resource");
			if (!Directory.Exists("resource/space1"))
				Directory.CreateDirectory("resource/space1");
			if (!Directory.Exists("resource/space1/100"))
				Directory.CreateDirectory("resource/space1/100");

			FileStream fs = File.Open("resource/space1/100/object_id1.jpg", 
															FileMode.Create);
			using (StreamWriter w = new StreamWriter(fs))
			{
				w.Write("1234567890");
			}
		}

		[TestCleanup]
		public void tearDown()
		{
			Directory.Delete("resource", true);
		}

		[TestMethod]
		public void TestView_ByPOST()
		{
			using (HttpServer server = new HttpServer(80))
			{
				server.AddHandler("/v1/objects/view", new ViewObjectHandler("resource"));
				server.Start();

				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
											"http://localhost/v1/objects/view");
				req.ContentType = "application/x-www-form-urlencoded";
				req.Method = "POST";
				using (StreamWriter fs = new StreamWriter(req.GetRequestStream()))
				{
					fs.Write("object_id=object_id1&file_type=100");
				}

				HttpWebResponse response = (HttpWebResponse)req.GetResponse();
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Assert.AreEqual("image/jpeg", response.ContentType);

				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					string responseText = reader.ReadToEnd();
					Assert.AreEqual("1234567890", responseText);
				}
			}
		}
	}
}

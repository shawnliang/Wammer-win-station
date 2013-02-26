using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using Wammer.Station;
using Waveface.Stream.Model;

namespace UT_WammerStation.Doc
{
	/// <summary>
	/// Summary description for GetPreviewTest
	/// </summary>
	[TestClass]
	public class GetPreviewTest
	{
		string object_id;

		[TestInitialize]
		public void setup()
		{
			object_id = Guid.NewGuid().ToString();
			AttachmentCollection.Instance.Save(
				new Attachment
				{
					object_id = object_id,
					type = AttachmentType.doc,
					doc_meta = new DocProperty
					{
						preview_files = new List<string> { "file1", "file2", "file3" }
					}
				});

			createFile("file1", "1111");
			createFile("file2", "2222");
			createFile("file3", "3333");

		}

		private void createFile(string filename, string data)
		{
			using (var w = File.CreateText(filename))
			{
				w.Write(data);
			}
		}

		[TestCleanup]
		public void tearDown()
		{
			if (File.Exists("file1"))
				File.Delete("file1");
			if (File.Exists("file2"))
				File.Delete("file2");
			if (File.Exists("file3"))
				File.Delete("file3");

			AttachmentCollection.Instance.RemoveAll();
		}


		[TestMethod]
		public void GetPreview1()
		{
			var param = new System.Collections.Specialized.NameValueCollection() {
				{ "apikey", "k"},
				{ "session_token", "s"},
				{ "object_id", object_id},
				{ "target", "preview"},
				{ "page", "1"}
			};

			var imp = new Wammer.Station.AttachmentView.AttachmentViewHandlerImp();
			var result = imp.GetAttachmentStream(param);

			Assert.AreEqual("image/jpeg", result.MimeType);
			using (var r = new StreamReader(result.Stream))
			{
				Assert.AreEqual("1111", r.ReadToEnd());
			}
		}

		[TestMethod]
		public void GetPreview3()
		{
			var param = new System.Collections.Specialized.NameValueCollection() {
				{ "apikey", "k"},
				{ "session_token", "s"},
				{ "object_id", object_id},
				{ "target", "preview"},
				{ "page", "3"}
			};

			var imp = new Wammer.Station.AttachmentView.AttachmentViewHandlerImp();
			var result = imp.GetAttachmentStream(param);

			Assert.AreEqual("image/jpeg", result.MimeType);
			using (var r = new StreamReader(result.Stream))
			{
				Assert.AreEqual("3333", r.ReadToEnd());
			}
		}

		[TestMethod]
		[ExpectedException(typeof(WammerStationException))]
		public void GetPreview_OutofIndex_4()
		{
			var param = new System.Collections.Specialized.NameValueCollection() {
				{ "apikey", "k"},
				{ "session_token", "s"},
				{ "object_id", object_id},
				{ "target", "preview"},
				{ "page", "4"}
			};

			var imp = new Wammer.Station.AttachmentView.AttachmentViewHandlerImp();
			var result = imp.GetAttachmentStream(param);
		}

		[TestMethod]
		[ExpectedException(typeof(WammerStationException))]
		public void GetPreview_OutofIndex_0()
		{
			var param = new System.Collections.Specialized.NameValueCollection() {
				{ "apikey", "k"},
				{ "session_token", "s"},
				{ "object_id", object_id},
				{ "target", "preview"},
				{ "page", "0"}
			};

			var imp = new Wammer.Station.AttachmentView.AttachmentViewHandlerImp();
			var result = imp.GetAttachmentStream(param);
		}
	}
}

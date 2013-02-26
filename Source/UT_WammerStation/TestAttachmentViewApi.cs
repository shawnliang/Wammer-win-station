using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using Wammer;
using Wammer.PerfMonitor;
using Wammer.Station;
using Waveface.Stream.Model;

namespace UT_WammerStation
{

	public class FackAttViewHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			using (BinaryWriter w = new BinaryWriter(Response.OutputStream))
			{
				w.Write(Encoding.UTF8.GetBytes("123456"));
			}
			Response.Close();
		}
	}

	[TestClass]
	public class TestAttachmentViewApi
	{
		HttpServer server8080 = new HttpServer(8080);
		HttpServer server80 = new HttpServer(80);

		[TestInitialize]
		public void Setup()
		{
			CloudServer.BaseUrl = "http://127.0.0.1:8080/";

			FakeCloudRemoteHandler.SavedParams = new System.Collections.Specialized.NameValueCollection();
			server8080.AddHandler("/attachments/view/", new FakeCloudRemoteHandler());
			server8080.Start();
			server8080.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(HttpRequestMonitor.Instance.OnTaskEnqueue);

			server80.AddHandler("/objects/view/DownloadAttachment", new FackAttViewHandler());
			server80.Start();
			server80.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(HttpRequestMonitor.Instance.OnTaskEnqueue);
		}

		[TestCleanup]
		public void TearDown()
		{
			server8080.Stop();
			server80.Stop();
		}
	}
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;

using Wammer.Station;

namespace UT_WammerStation
{
	class MyHandler : IHttpHandler
	{
		byte[] response;

		public MyHandler(string response)
		{
			this.response = Encoding.ASCII.GetBytes(response);
		}

		public MyHandler(byte[] response)
		{
			this.response = response;
		}

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			response.StatusCode = 200;
			response.OutputStream.Write(this.response, 0, this.response.Length);
			response.Close();
		}

		public object Clone()
		{
			return new MyHandler(response);
		}
	}


	[TestClass]
	public class TestHttpServer
	{
		[TestMethod]
		public void TestDispatchToHandlers()
		{
			using (HttpServer server = new HttpServer(80))
			{
				string reply1 = "from handler1";
				string reply2 = "from handler2";

				server.AddHandler("/class1/action1/", new MyHandler(reply1));
				server.AddHandler("/class1/action2/", new MyHandler(reply2));
				server.Start();

				WebClient agent = new WebClient();
				string replyFromHandler1 = agent.DownloadString(
										"http://127.0.0.1:80/class1/action1/");
				string replyFromHandler2 = agent.DownloadString(
										"http://127.0.0.1:80/class1/action2/");
				Assert.AreEqual(reply1, replyFromHandler1);
				Assert.AreEqual(reply2, replyFromHandler2);
			}
		}

		[TestMethod]
		public void TestDefaultHandler()
		{
			using (HttpServer server = new HttpServer(80))
			{
				string reply1 = "from handler1";

				server.AddDefaultHandler(new MyHandler("1234567890"));
				server.AddHandler("/class1/action1/", new MyHandler(reply1));
				server.Start();

				WebClient agent = new WebClient();
				string replyFromHandler1 = agent.DownloadString(
									"http://127.0.0.1:80/class1/action1/pp/");
				Assert.AreEqual(replyFromHandler1, "1234567890");
			}
		}

		[TestMethod]
		public void TestBuiltInDefaultHandler()
		{
			using (HttpServer server = new HttpServer(80))
			{
				string reply1 = "from handler1";

				server.AddHandler("/class1/action1/", new MyHandler(reply1));
				server.Start();

				try
				{
					WebClient agent = new WebClient();
					string replyFromHandler1 = agent.DownloadString(
										"http://127.0.0.1:80/class1/action1/pp/");
				}
				catch (WebException e)
				{
					Assert.AreEqual(WebExceptionStatus.ProtocolError, e.Status);
					return;
				}
				Assert.Fail("expected exception is not thrown");
			}
		}

		[TestMethod]
		public void TestPrefixOfAnotherHandler()
		{
			using (HttpServer server = new HttpServer(80))
			{
				string reply1 = "from handler1";
				string reply2 = "from handler2";

				server.AddHandler("/class1/action1/", new MyHandler(reply1));
				server.AddHandler("/class1", new MyHandler(reply2));
				server.Start();

				WebClient agent = new WebClient();
				string replyFromHandler1 = agent.DownloadString(
										"http://127.0.0.1:80/class1/action1/");
				string replyFromHandler2 = agent.DownloadString(
										"http://127.0.0.1:80/class1/");
				Assert.AreEqual(reply1, replyFromHandler1);
				Assert.AreEqual(reply2, replyFromHandler2);
			}
		}

		[TestMethod]
		public void TestInvokingPathHasNoEndingSlash()
		{
			using (HttpServer server = new HttpServer(80))
			{
				string reply1 = "from handler1";
				string reply2 = "from handler2";

				server.AddHandler("/class1/action1/", new MyHandler(reply1));
				server.AddHandler("/class1", new MyHandler(reply2));
				server.Start();

				WebClient agent = new WebClient();
				string replyFromHandler1 = agent.DownloadString(
										"http://127.0.0.1:80/class1/action1");
				string replyFromHandler2 = agent.DownloadString(
										"http://127.0.0.1:80/class1");
				Assert.AreEqual(reply1, replyFromHandler1);
				Assert.AreEqual(reply2, replyFromHandler2);
			}
		}

		[TestMethod]
		public void TestOffline()
		{
			using (HttpServer server = new HttpServer(80))
			{
				string reply1 = "from handler1";
				server.AddHandler("/class1/action1/", new MyHandler(reply1));
				server.OfflineKey = "12345";
				server.Start();

				WebClient agent = new WebClient();
				agent.DownloadString("http://localhost/station/offline?key=12345");

				try
				{
					agent.DownloadString("http://localhost/class1/action1/");
				}
				catch (WebException e)
				{
					HttpWebResponse res = (HttpWebResponse)e.Response;
					Assert.AreEqual(HttpStatusCode.ServiceUnavailable, res.StatusCode);
					Wammer.Cloud.WammerCloudException werr = new Wammer.Cloud.WammerCloudException("", e);
					Wammer.Cloud.CloudResponse json = fastJSON.JSON.Instance.ToObject<Wammer.Cloud.CloudResponse>(werr.response);
					Assert.AreEqual((int)StationApiError.ServerOffline, json.api_ret_code);
					Assert.AreEqual((int)HttpStatusCode.ServiceUnavailable, json.status);

					return;
				}

				Assert.Fail("expected exception is not thrown");
			}
		}

		[TestMethod]
		public void TestOfflineThenOnline()
		{
			using (HttpServer server = new HttpServer(80))
			{
				string reply1 = "from handler1";
				server.AddHandler("/class1/action1/", new MyHandler(reply1));
				server.OfflineKey = "12345";
				server.Start();

				WebClient agent = new WebClient();
				agent.DownloadString("http://localhost/station/offline?key=12345");
				agent.DownloadString("http://localhost/station/online?key=12345");

				Assert.AreEqual(reply1, agent.DownloadString("http://localhost/class1/action1/"));
			}
		}

		[TestMethod]
		public void TestOfflineNeedsAKey()
		{
			using (HttpServer server = new HttpServer(80))
			{
				string reply1 = "from handler1";
				server.AddHandler("/class1/action1/", new MyHandler(reply1));
				server.OfflineKey = "12345";
				server.Start();

				WebClient agent = new WebClient();
				

				try
				{
					agent.DownloadString("http://localhost/station/offline?key=aaaaa");
				}
				catch (WebException e)
				{
					HttpWebResponse res = (HttpWebResponse)e.Response;
					Assert.AreEqual(HttpStatusCode.BadRequest, res.StatusCode);
					Wammer.Cloud.WammerCloudException werr = new Wammer.Cloud.WammerCloudException("", e);
					Wammer.Cloud.CloudResponse json = fastJSON.JSON.Instance.ToObject<Wammer.Cloud.CloudResponse>(werr.response);
					Assert.AreEqual((int)StationApiError.Error, json.api_ret_code);
					Assert.AreEqual((int)HttpStatusCode.BadRequest, json.status);
					Assert.AreEqual("Offline key error", json.api_ret_msg);
					return;
				}

				Assert.Fail("expected exception is not thrown");
			}
		}
	}
}

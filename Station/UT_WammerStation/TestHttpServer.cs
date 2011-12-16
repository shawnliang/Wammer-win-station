using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using System.Collections.Specialized;

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

	class MyHandler2 : HttpHandler
	{
		public static NameValueCollection SavedParameters; 
		public override object Clone()
		{
			return this.MemberwiseClone();
		}

		protected override void HandleRequest()
		{
			SavedParameters = this.Parameters;
			this.RespondSuccess();
		}
	}

	[TestClass]
	public class TestHttpServer
	{
		[TestMethod]
		public void TestEncoding()
		{
			using (HttpServer server = new HttpServer(80))
			{
				MyHandler2 h2 = new MyHandler2();
				server.AddHandler("/class1/action1/", h2);
				server.Start();

				WebClient agent = new WebClient();
				string replyFromHandler1 = agent.DownloadString(
										"http://127.0.0.1:80/class1/action1/?email=sh%40wave.com");


				Assert.AreEqual("sh@wave.com", MyHandler2.SavedParameters["email"]);
			}
		}
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
		public void TestRestartHttpServer()
		{
			using (HttpServer server = new HttpServer(80))
			{
				string reply1 = "from handler1";

				server.AddHandler("/class1/action1/", new MyHandler(reply1));
				server.Start();

				WebClient agent = new WebClient();
				Assert.AreEqual(reply1, agent.DownloadString("http://127.0.0.1:80/class1/action1"));


				server.Stop();

				System.Threading.Thread.Sleep(1000);
				server.Start();

				Assert.AreEqual(reply1, agent.DownloadString("http://127.0.0.1:80/class1/action1"));
			}
		}
	}
}

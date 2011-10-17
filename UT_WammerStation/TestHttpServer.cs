using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
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

		public void Handle(object state)
		{
			HttpListenerContext context = (HttpListenerContext)state;

			context.Response.StatusCode = 200;
			context.Response.OutputStream.Write(response, 0, response.Length);
			context.Response.Close();
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
	}
}

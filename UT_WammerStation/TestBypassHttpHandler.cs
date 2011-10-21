using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Net;
using Wammer.Station;
using System.Text;

namespace UT_WammerStation
{
	class MyForwardedHandler: IHttpHandler
	{
		public CookieCollection recvCookies;
		public NameValueCollection recvQueryString;
		public string recvData;
		public string recvContentType;
		public long recvContentLength;
		public string recvMethod;
		public string recvPath;

		public void Handle(object state)
		{
			HttpListenerContext ctx = (HttpListenerContext)state;

			recvCookies = ctx.Request.Cookies;
			recvQueryString = ctx.Request.QueryString;
			recvContentType = ctx.Request.ContentType;
			recvContentLength = ctx.Request.ContentLength64;
			recvMethod = ctx.Request.HttpMethod;
			recvPath = ctx.Request.Url.AbsolutePath;
			using (StreamReader r = new StreamReader(ctx.Request.InputStream))
			{
				recvData = r.ReadToEnd();
			}


			ctx.Response.StatusCode = 200;
			ctx.Response.Cookies.Add(new Cookie("aaa", "111"));
			ctx.Response.Cookies.Add(new Cookie("bbb", "222"));
			using (StreamWriter w = new StreamWriter(ctx.Response.OutputStream))
			{
				w.Write("write from target");
			}
		}
	}

	[TestClass]
	public class TestBypassHttpHandler
	{
		[TestMethod]
		public void TestByPass()
		{
			
			using (HttpServer proxyServer = new HttpServer(80))
			using (HttpServer targetServer = new HttpServer(8080))
			{
				MyForwardedHandler target = new MyForwardedHandler();
				targetServer.AddHandler("/target/", target);
				targetServer.Start();
				proxyServer.AddHandler("/", new MyHandler("21212")); // dummy
				proxyServer.AddDefaultHandler(new BypassHttpHandler("localhost", 8080));
				proxyServer.Start();
				

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
													"http://localhost:80/target/?abc=123&qaz=wsx");

				request.ContentType = "application/shawn";
				request.Method = "POST";
				request.ContentLength = 20;

				request.CookieContainer = new CookieContainer();
				request.CookieContainer.Add(new Cookie("cookie1", "val1", "", "localhost"));
				request.CookieContainer.Add(new Cookie("cookie2", "val2", "", "localhost"));

				using (StreamWriter w = new StreamWriter(request.GetRequestStream()))
				{
					w.Write("1234567890qwertyuiop");
				}

				HttpWebResponse resp = (HttpWebResponse)request.GetResponse();

				Assert.AreEqual(request.ContentType, target.recvContentType);
				Assert.AreEqual(request.ContentLength, target.recvContentLength);
				Assert.AreEqual(request.Method, target.recvMethod);
				Assert.AreEqual("val1", target.recvCookies["cookie1"].Value);
				Assert.AreEqual("val2", target.recvCookies["cookie2"].Value);
				Assert.AreEqual(2, target.recvCookies.Count);
				Assert.AreEqual("123", target.recvQueryString["abc"]);
				Assert.AreEqual("wsx", target.recvQueryString["qaz"]);
				Assert.AreEqual("/target/", target.recvPath);
				Assert.AreEqual("1234567890qwertyuiop", target.recvData);

				using (StreamReader r = new StreamReader(resp.GetResponseStream()))
				{
					Assert.AreEqual("write from target", r.ReadToEnd());
				}

				Assert.AreEqual(2, resp.Cookies.Count);
				Assert.AreEqual("111", resp.Cookies["aaa"].Value);
				Assert.AreEqual("/target/", resp.Cookies["aaa"].Path);
				Assert.AreEqual("222", resp.Cookies["bbb"].Value);
			}

		}
	}
}

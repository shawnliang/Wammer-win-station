using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Net;
using Wammer.Station;
using Wammer.Cloud;

namespace UT_WammerStation
{
	class ErrorHttpHandler:IHttpHandler
	{
		int status;
		string msg;

		public ErrorHttpHandler(int status, string msg)
		{
			this.status = status;
			this.msg = msg;
		}

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			response.StatusCode = this.status;
			using (StreamWriter w = new StreamWriter(response.OutputStream))
			{
				if (msg != null)
					w.Write(this.msg);
			}
		}
	}


	class MyForwardedHandler: IHttpHandler
	{
		public CookieCollection recvCookies;
		public NameValueCollection recvQueryString;
		public string recvData;
		public string recvContentType;
		public long recvContentLength;
		public string recvMethod;
		public string recvPath;

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			recvCookies = request.Cookies;
			recvQueryString = request.QueryString;
			recvContentType = request.ContentType;
			recvContentLength = request.ContentLength64;
			recvMethod = request.HttpMethod;
			recvPath = request.Url.AbsolutePath;
			using (StreamReader r = new StreamReader(request.InputStream))
			{
				recvData = r.ReadToEnd();
			}


			response.StatusCode = 200;
			response.Cookies.Add(new Cookie("aaa", "111"));
			response.Cookies.Add(new Cookie("bbb", "222"));
			using (StreamWriter w = new StreamWriter(response.OutputStream))
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


		[TestMethod]
		public void TestBypassException()
		{
			using (HttpServer proxyServer = new HttpServer(80))
			{
				proxyServer.AddHandler("/", new MyHandler("dummy")); // dummy
				BypassHttpHandler bypasser = new BypassHttpHandler("localhost", 8080);
				bypasser.AddExceptPrefix("/bypass/");
				proxyServer.AddDefaultHandler(bypasser);
				proxyServer.Start();

				WebClient agent = new WebClient();
				try
				{
					agent.DownloadData("http://localhost:80/bypass/");
				}
				catch (WebException e)
				{
					Assert.AreEqual(WebExceptionStatus.ProtocolError, e.Status);
					using (StreamReader r = new StreamReader(e.Response.GetResponseStream()))
					{
						string json = r.ReadToEnd();
						CloudResponse resp = fastJSON.JSON.Instance.ToObject<CloudResponse>(json);
						Assert.AreEqual(-1, resp.app_ret_code);
						Assert.AreEqual(403, resp.status);
						Assert.AreEqual("Station does not support this REST API; only Cloud does",
																				resp.app_ret_msg);
					}
					return;
				}

				Assert.Fail("Expected exception is not thrown");
			}
		}

		[TestMethod]
		public void TestBypassException_pathWithOutSlash()
		{
			using (HttpServer proxyServer = new HttpServer(80))
			{
				proxyServer.AddHandler("/", new MyHandler("dummy")); // dummy
				BypassHttpHandler bypasser = new BypassHttpHandler("localhost", 8080);
				bypasser.AddExceptPrefix("/bypass/");
				proxyServer.AddDefaultHandler(bypasser);
				proxyServer.Start();

				WebClient agent = new WebClient();
				try
				{
					agent.DownloadData("http://localhost:80/bypass");
				}
				catch (WebException e)
				{
					Assert.AreEqual(WebExceptionStatus.ProtocolError, e.Status);
					using (StreamReader r = new StreamReader(e.Response.GetResponseStream()))
					{
						string json = r.ReadToEnd();
						CloudResponse resp = fastJSON.JSON.Instance.ToObject<CloudResponse>(json);
						Assert.AreEqual(-1, resp.app_ret_code);
						Assert.AreEqual(403, resp.status);
						Assert.AreEqual("Station does not support this REST API; only Cloud does",
																				resp.app_ret_msg);
					}
					return;
				}

				Assert.Fail("Expected exception is not thrown");
			}
		}

		[TestMethod]
		public void TestBypassException_pathWithQueryString()
		{
			using (HttpServer proxyServer = new HttpServer(80))
			{
				proxyServer.AddHandler("/", new MyHandler("dummy")); // dummy
				BypassHttpHandler bypasser = new BypassHttpHandler("localhost", 8080);
				bypasser.AddExceptPrefix("/bypass/");
				proxyServer.AddDefaultHandler(bypasser);
				proxyServer.Start();

				WebClient agent = new WebClient();
				try
				{
					agent.DownloadData("http://localhost:80/bypass?abc=123");
				}
				catch (WebException e)
				{
					Assert.AreEqual(WebExceptionStatus.ProtocolError, e.Status);
					using (StreamReader r = new StreamReader(e.Response.GetResponseStream()))
					{
						string json = r.ReadToEnd();
						CloudResponse resp = fastJSON.JSON.Instance.ToObject<CloudResponse>(json);
						Assert.AreEqual(-1, resp.app_ret_code);
						Assert.AreEqual(403, resp.status);
						Assert.AreEqual("Station does not support this REST API; only Cloud does",
																				resp.app_ret_msg);
					}
					return;
				}

				Assert.Fail("Expected exception is not thrown");
			}
		}

		[TestMethod]
		public void TestBypassRemoteError()
		{
			using (HttpServer proxyServer = new HttpServer(80))
			using (HttpServer targetServer = new HttpServer(8080))
			{
				proxyServer.AddHandler("/", new MyHandler("dummy")); // dummy
				BypassHttpHandler bypasser = new BypassHttpHandler("localhost", 8080);
				proxyServer.AddDefaultHandler(bypasser);
				proxyServer.Start();

				targetServer.AddHandler("/bypass/", new ErrorHttpHandler(502, "err from target"));
				targetServer.Start();

				WebClient agent = new WebClient();
				try
				{
					agent.DownloadData("http://localhost:80/bypass/");
				}
				catch (WebException e)
				{
					Assert.AreEqual(HttpStatusCode.BadGateway, 
														((HttpWebResponse)e.Response).StatusCode);
					using (StreamReader r = new StreamReader(e.Response.GetResponseStream()))
					{
						string json = r.ReadToEnd();
						Assert.AreEqual("err from target", json);
					}
					return;
				}

				Assert.Fail("Expected exception is not thrown");
			}
		}

		[TestMethod]
		public void TestBypassRemoteError_no_response_stream()
		{
			using (HttpServer proxyServer = new HttpServer(80))
			using (HttpServer targetServer = new HttpServer(8080))
			{
				proxyServer.AddHandler("/", new MyHandler("dummy")); // dummy
				BypassHttpHandler bypasser = new BypassHttpHandler("localhost", 8080);
				proxyServer.AddDefaultHandler(bypasser);
				proxyServer.Start();

				targetServer.AddHandler("/bypass/", new ErrorHttpHandler(502, null));
				targetServer.Start();

				WebClient agent = new WebClient();
				try
				{
					agent.DownloadData("http://localhost:80/bypass/");
				}
				catch (WebException e)
				{
					Assert.AreEqual(HttpStatusCode.BadGateway,
														((HttpWebResponse)e.Response).StatusCode);
					using (StreamReader r = new StreamReader(e.Response.GetResponseStream()))
					{
						string json = r.ReadToEnd();
						Assert.AreEqual("", json);
					}
					return;
				}

				Assert.Fail("Expected exception is not thrown");
			}
		}
	}
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Wammer.Cloud;
using Wammer.Station;

namespace UT_WammerStation
{
	class ErrorHttpHandler : IHttpHandler
	{
		int status;
		string msg;

		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;

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

		public object Clone()
		{
			return new ErrorHttpHandler(status, msg);
		}

		public void SetBeginTimestamp(long beginTime)
		{
		}


		public void OnTaskEnqueue(EventArgs e)
		{
		}


		public void HandleRequest()
		{
		}
	}


	class MyForwardedHandler : IHttpHandler
	{
		public static CookieCollection recvCookies;
		public static NameValueCollection recvQueryString;
		public static string recvData;
		public static string recvContentType;
		public static long recvContentLength;
		public static string recvMethod;
		public static string recvPath;

		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;

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

		public void SetBeginTimestamp(long beginTime)
		{
		}
		public object Clone()
		{
			return new MyForwardedHandler();
		}


		public void OnTaskEnqueue(EventArgs e)
		{
		}


		public void HandleRequest()
		{
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
				proxyServer.AddDefaultHandler(new BypassHttpHandler("http://localhost:8080", "stid"));
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

				Assert.AreEqual(request.ContentType, MyForwardedHandler.recvContentType);
				Assert.AreEqual(request.ContentLength, MyForwardedHandler.recvContentLength);
				Assert.AreEqual(request.Method, MyForwardedHandler.recvMethod);
				Assert.AreEqual("val1", MyForwardedHandler.recvCookies["cookie1"].Value);
				Assert.AreEqual("val2", MyForwardedHandler.recvCookies["cookie2"].Value);
				Assert.AreEqual(2, MyForwardedHandler.recvCookies.Count);
				Assert.AreEqual("123", MyForwardedHandler.recvQueryString["abc"]);
				Assert.AreEqual("wsx", MyForwardedHandler.recvQueryString["qaz"]);
				Assert.AreEqual("/target/", MyForwardedHandler.recvPath);
				Assert.AreEqual("1234567890qwertyuiop", MyForwardedHandler.recvData);

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
				BypassHttpHandler bypasser = new BypassHttpHandler("http://localhost:8080", "stid");
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
						Assert.AreEqual(-1, resp.api_ret_code);
						Assert.AreEqual(403, resp.status);
						Assert.AreEqual("Station does not support this REST API; only Cloud does",
																				resp.api_ret_message);
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
				BypassHttpHandler bypasser = new BypassHttpHandler("http://localhost:8080", "stid");
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
						Assert.AreEqual(-1, resp.api_ret_code);
						Assert.AreEqual(403, resp.status);
						Assert.AreEqual("Station does not support this REST API; only Cloud does",
																				resp.api_ret_message);
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
				BypassHttpHandler bypasser = new BypassHttpHandler("http://localhost:8080", "stid");
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
						Assert.AreEqual(-1, resp.api_ret_code);
						Assert.AreEqual(403, resp.status);
						Assert.AreEqual("Station does not support this REST API; only Cloud does",
																				resp.api_ret_message);
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
				BypassHttpHandler bypasser = new BypassHttpHandler("http://localhost:8080", "stid");
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
				BypassHttpHandler bypasser = new BypassHttpHandler("http://localhost:8080", "stid");
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

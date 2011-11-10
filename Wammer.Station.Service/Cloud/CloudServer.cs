using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Wammer.Station;
using System.Web;
using System.IO;

namespace Wammer.Cloud
{
	public class CloudServer
	{
		private const string DEF_HOST_NAME = "develop.waveface.com";
		private const int DEF_PORT = 8080;
		private const string DEF_API_KEY = "0ffd0a63-65ef-512b-94c7-ab3b33117363";

		private static CookieContainer cookies = new CookieContainer();

		private static string hostname = null;
		private static int port = 0;
		private static string apiKey = null;

		public const string DEF_BASE_PATH = "v2";

		public const string PARAM_API_KEY = "apikey";
		public const string PARAM_EMAIL = "email";
		public const string PARAM_PASSWORD = "password";
		public const string PARAM_SESSION_TOKEN = "session_token";
		public const string PARAM_STATION_ID = "station_id";

		public static string SessionToken { get; set; }

		/// <summary>
		/// Gets wammer cloud base url
		/// </summary>
		public static string BaseUrl
		{
			get
			{
				return string.Format("http://{0}:{1}/", HostName, Port);
			}
		}

		/// <summary>
		/// Gets or sets wammer cloud host name
		/// </summary>
		public static string HostName
		{
			get
			{
				if (hostname != null)
					return hostname;

				return (string)StationRegistry.GetValue("cloudHostName", DEF_HOST_NAME);
			}
			set { hostname = value; }
		}

		/// <summary>
		/// Gets or sets wammer cloud port number
		/// </summary>
		public static int Port
		{
			get
			{
				if (port != 0)
					return port;

				return (int)StationRegistry.GetValue("cloudPort", DEF_PORT);
			}
			set { port = value; }
		}

		/// <summary>
		/// Gets or set api key that will be sent to cloud
		/// </summary>
		public static string APIKey
		{
			get
			{
				if (apiKey != null)
					return apiKey;

				return (string)StationRegistry.GetValue("cloudAPIKey", DEF_API_KEY);
			}
			set { apiKey = value; }
		}

		/// <summary>
		/// Requests Wammer cloud via http post
		/// </summary>
		/// <typeparam name="T">response type</typeparam>
		/// <param name="agent">web client agent</param>
		/// <param name="path">partial path of cloud url, http://host:port/base/partial_path</param>
		/// <param name="parms">request parameter names and values.
		///	They will be URLEncoded and transformed to name1=val1&amp;name2=val2...</param>
		/// <returns>Response value</returns>
		public static T requestPath<T>(WebClient agent, string path, Dictionary<object, object> parms)
		{
			string url = string.Format("http://{0}:{1}/{2}/{3}",
				CloudServer.HostName,
				CloudServer.Port,
				CloudServer.DEF_BASE_PATH,
				path);

			return request<T>(agent, url, parms);
		}

		public static T request<T>(WebClient agent, string url, Dictionary<object, object> param)
		{
			if (param.Count == 0)
				return request<T>(agent, url, "");

			StringBuilder buf = new StringBuilder();
			foreach (KeyValuePair<object, object> pair in param)
			{
				buf.Append(HttpUtility.UrlEncode(pair.Key.ToString()));
				buf.Append("=");
				buf.Append(HttpUtility.UrlEncode(pair.Value.ToString()));
				buf.Append("&");
			}

			// remove last &
			buf.Remove(buf.Length - 1, 1);

			return request<T>(agent, url, buf.ToString());
		}

		private static T request<T>(WebClient agent, string url, string postData)
		{
			string response = "";
			T resObj;

			try
			{
				agent.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
				response = agent.UploadString(url, "POST", postData);
				resObj = fastJSON.JSON.Instance.ToObject<T>(response);
			}
			catch (WebException e)
			{
				string resText = GetErrResponseText(e);
				throw new WammerCloudException("Wammer cloud error", postData, resText, e);
			}
			catch (Exception e)
			{
				throw new WammerCloudException("Wammer cloud error", postData, response, e);
			}

			if (resObj is CloudResponse)
			{
				CloudResponse cres = resObj as CloudResponse;
				if (cres.status != 200)
					throw new WammerCloudException("Wammer cloud error", postData, response, 
						cres.api_ret_code);
			}


			return resObj;
		}

		private static string GetErrResponseText(WebException e)
		{
			try
			{
				using (BinaryReader r = new BinaryReader(e.Response.GetResponseStream()))
				{
					byte[] res = r.ReadBytes((int)e.Response.ContentLength);
					return Encoding.UTF8.GetString(res);
				}
			}
			catch
			{
				// don't care if error response is unavailable
				return null;
			}
		}
	}
}

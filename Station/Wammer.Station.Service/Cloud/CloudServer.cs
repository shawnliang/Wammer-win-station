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
		private const string DEF_API_KEY = "0ffd0a63-65ef-512b-94c7-ab3b33117363";
		public const string DEF_BASE_PATH = "v2";
		public const string DEF_BASE_URL = "http://develop.waveface.com:8080/v2/";

		private static string apiKey = null;
		private static string baseUrl = null;

		public const string PARAM_API_KEY = "apikey";
		public const string PARAM_EMAIL = "email";
		public const string PARAM_PASSWORD = "password";
		public const string PARAM_SESSION_TOKEN = "session_token";
		public const string PARAM_STATION_ID = "station_id";
		public const string PARAM_LOCATION = "location";

		public static string SessionToken { get; set; }

		/// <summary>
		/// Gets or sets wammer cloud base url
		/// </summary>
		public static string BaseUrl
		{
			get
			{
				if (baseUrl == null)
					baseUrl = (string)StationRegistry.GetValue("cloudBaseURL", DEF_BASE_URL);

				return baseUrl;
			}

			set
			{
				baseUrl = value;
			}
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
			string url = CloudServer.BaseUrl + path;

			return request<T>(agent, url, parms);
		}

		public static T request<T>(WebClient agent, string url, Dictionary<object, object> param, bool isGet)
		{
			if (param.Count == 0)
				return request<T>(agent, url, "", isGet);

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

			return request<T>(agent, url, buf.ToString(), isGet);
		}

		private static T request<T>(WebClient agent, string url, string parameters, bool isGet)
		{
			string response = "";
			T resObj;

			try
			{
				agent.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
				byte[] rawResponse = null;
				if (isGet)
				{
					if (string.IsNullOrEmpty(parameters))
					{
						rawResponse = agent.DownloadData(url);
					}
					else
					{
						rawResponse = agent.DownloadData(url + "?" + parameters);
					}
				}
				else
				{
					rawResponse = agent.UploadData(url, "POST", Encoding.UTF8.GetBytes(parameters));
				}
				response = Encoding.UTF8.GetString(rawResponse);
				resObj = fastJSON.JSON.Instance.ToObject<T>(response);
			}
			catch (WebException e)
			{
				throw new WammerCloudException("Wammer cloud error", parameters, e);
			}
			catch (Exception e)
			{
				throw new WammerCloudException("Wammer cloud error", parameters, response, e);
			}

			if (resObj is CloudResponse)
			{
				CloudResponse cres = resObj as CloudResponse;
				if (cres.status != 200 || cres.api_ret_code != 0)
					throw new WammerCloudException("Wammer cloud error", parameters, response,
						cres.api_ret_code);
			}


			return resObj;
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
				byte[] rawResponse = agent.UploadData(url, "POST", Encoding.UTF8.GetBytes(postData));
				response = Encoding.UTF8.GetString(rawResponse);
				resObj = fastJSON.JSON.Instance.ToObject<T>(response);
			}
			catch (WebException e)
			{
				throw new WammerCloudException("Wammer cloud error", postData, e);
			}
			catch (Exception e)
			{
				throw new WammerCloudException("Wammer cloud error", postData, response, e);
			}

			if (resObj is CloudResponse)
			{
				CloudResponse cres = resObj as CloudResponse;
				if (cres.status != 200 || cres.api_ret_code != 0)
					throw new WammerCloudException("Wammer cloud error", postData, response,
						cres.api_ret_code);
			}


			return resObj;
		}
	}
}

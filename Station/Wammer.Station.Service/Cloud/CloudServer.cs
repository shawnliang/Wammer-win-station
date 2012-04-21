using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using Wammer.Station;
using System.ComponentModel;
using Wammer.PerfMonitor;

namespace Wammer.Cloud
{
	public class CloudServer
	{
		private const string DEF_API_KEY = "0ffd0a63-65ef-512b-94c7-ab3b33117363";
		public const string DEF_BASE_PATH = "v2";
		public const string DEF_BASE_URL = "https://develop.waveface.com/v2/"; //https://api.waveface.com/v2/

		private static string apiKey = null;
		private static string baseUrl = null;

		public const string PARAM_DETAIL = "detail";
		public const string PARAM_API_KEY = "apikey";
		public const string PARAM_EMAIL = "email";
		public const string PARAM_PASSWORD = "password";
		public const string PARAM_SESSION_TOKEN = "session_token";
		public const string PARAM_STATION_ID = "station_id";
		public const string PARAM_LOCATION = "location";
		public const string PARAM_USER_ID = "user_id";
		public const string PARAM_LIMIT = "limit";
		public const string PARAM_FILTER_ENTITY = "filter_entity";
		public const string PARAM_POST_ID_LIST = "post_id_list";
		public const string PARAM_GROUP_ID = "group_id";
		public const string PARAM_CONTENT = "content";
		public const string PARAM_OBJECT_ID = "object_id";
		public const string PARAM_TYPE = "type";
		public const string PARAM_ATTACHMENT_ID_ARRAY = "attachment_id_array";
		public const string PARAM_PREVIEW = "preview";
		public const string PARAM_IMAGE_META = "image_meta";
		public const string PARAM_POST_ID = "post_id";
		public const string PARAM_LAST_UPDATE_TIME = "last_update_time";
		public const string PARAM_COVER_ATTACH = "cover_attach";
		public const string PARAM_FAVORITE = "favorite";
		public const string PARAM_TIMESTAMP = "timestamp";

		public static string SessionToken { get; set; }

		private static bool isOffline = false;
		
		
		

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

		public static void requestDownload(WebClient agent, string path, Dictionary<object, object> parameters,
			string filepath)
		{
			StringBuilder buf = new StringBuilder();
			try
			{
				foreach (KeyValuePair<object, object> pair in parameters)
				{
					buf.Append(HttpUtility.UrlEncode(pair.Key.ToString()));
					buf.Append("=");
					buf.Append(HttpUtility.UrlEncode(pair.Value.ToString()));
					buf.Append("&");
				}

				// remove last &
				buf.Remove(buf.Length - 1, 1);				

				var stream = agent.OpenRead(new Uri(CloudServer.baseUrl + path + "?" + buf.ToString()));
				stream.WriteTo(filepath, 1024, (sender, e) =>
				{
				    PerfCounter.GetCounter(PerfCounter.DWSTREAM_RATE, false).IncrementBy(long.Parse(e.UserState.ToString()));
				});
				stream.Close();
			}
			catch (WebException e)
			{
				throw new WammerCloudException("Wammer cloud error", e);
			}
			catch (Exception e)
			{
				throw new WammerCloudException("Wammer cloud error", e);
			}
		}

		public static void requestAsyncDownload(WebClient agent, string path, Dictionary<object, object> parameters, 
			string filepath, AsyncCompletedEventHandler handler, object evtargs)
		{
			StringBuilder buf = new StringBuilder();
			foreach (KeyValuePair<object, object> pair in parameters)
			{
				buf.Append(HttpUtility.UrlEncode(pair.Key.ToString()));
				buf.Append("=");
				buf.Append(HttpUtility.UrlEncode(pair.Value.ToString()));
				buf.Append("&");
			}

			// remove last &
			buf.Remove(buf.Length - 1, 1);

			agent.DownloadFileCompleted += handler;
			agent.DownloadFileAsync(new Uri(CloudServer.baseUrl + path + "?" + buf.ToString()), filepath, evtargs);
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
		public static string requestPath(WebClient agent, string path, Dictionary<object, object> parms, bool checkOffline = true)
		{
			if (checkOffline)
			{
				if (isOffline)
				{
					throw new WammerCloudException("Station is in offline mode", new WebException("Station is in offline mode", WebExceptionStatus.ConnectFailure));
				}
			}

			string url = CloudServer.BaseUrl + path;

			try
			{
				string res = request(agent, url, parms);
				isOffline = false;
				return res;
			}
			catch (WammerCloudException e)
			{
				isOffline = IsNetworkError(e);
				throw e;
			}
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
		public static T requestPath<T>(WebClient agent, string path, Dictionary<object, object> parms, bool checkOffline = true)
		{
			if (checkOffline)
			{
				if (isOffline)
				{
					throw new WammerCloudException("Station is in offline mode", new WebException("Station is in offline mode", WebExceptionStatus.ConnectFailure));
				}
			}

			try
			{
				T res = ConvertFromJson<T>(requestPath(agent, path, parms, false));
				isOffline = false;
				return res;
			}
			catch (WammerCloudException e)
			{
				isOffline = IsNetworkError(e);
				throw e;
			}
		}

		public static T request<T>(WebClient agent, string url, Dictionary<object, object> param, bool isGet, bool checkOffline = true)
		{
			if (checkOffline)
			{
				if (isOffline)
				{
					throw new WammerCloudException("Station is in offline mode", new WebException("Station is in offline mode", WebExceptionStatus.ConnectFailure));
				}
			}

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

			try
			{
				T res = request<T>(agent, url, buf.ToString(), isGet);
				isOffline = false;
				return res;
			}
			catch (WammerCloudException e)
			{
				isOffline = IsNetworkError(e);
				throw e;
			}
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
				throw new WammerCloudException("Wammer cloud error", e);
			}
			catch (Exception e)
			{
				throw new WammerCloudException("Wammer cloud error", response, e);
			}

			CloudResponse cres = resObj as CloudResponse;
			if (cres != null)
			{
				if (cres.status != 200 || cres.api_ret_code != 0)
					throw new WammerCloudException("Wammer cloud error", response,
						cres.api_ret_code);
			}


			return resObj;
		}

		public static string request(WebClient agent, string url, Dictionary<object, object> param)
		{
			if (param.Count == 0)
				return request(agent, url);

			StringBuilder buf = new StringBuilder();
			foreach (KeyValuePair<object, object> pair in param)
			{
				buf.Append(HttpUtility.UrlEncode(pair.Key.ToString()));
				buf.Append("=");
				buf.Append(HttpUtility.UrlEncode(Convert.ToString(pair.Value)));
				buf.Append("&");
			}

			// remove last &
			buf.Remove(buf.Length - 1, 1);

			return request(agent, url, buf.ToString());
		}

		public static T request<T>(WebClient agent, string url, Dictionary<object, object> param, bool checkOffline = true)
		{
			if (checkOffline)
			{
				if (isOffline)
				{
					throw new WammerCloudException("Station is in offline mode", new WebException("Station is in offline mode", WebExceptionStatus.ConnectFailure));
				}
			}

			try
			{
				T res = ConvertFromJson<T>(request(agent, url, param));
				isOffline = false;
				return res;
			}
			catch (WammerCloudException e)
			{
				isOffline = IsNetworkError(e);
				throw e;
			}
		}

		private static string request(WebClient agent, string url, string postData = "")
		{
			try
			{
				agent.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
				byte[] rawResponse = agent.UploadData(url, "POST", Encoding.UTF8.GetBytes(postData));
				return Encoding.UTF8.GetString(rawResponse);
			}
			catch (WebException e)
			{
				throw new WammerCloudException("Wammer cloud error", e);
			}
		}

		private static T request<T>(WebClient agent, string url, string postData)
		{
			string response = string.Empty;

			response = request(agent, url, postData);

			return ConvertFromJson<T>(response);
		}

		public static T ConvertFromJson<T>(string json)
		{
			T resObj;
			try
			{
				resObj = fastJSON.JSON.Instance.ToObject<T>(json);
			}
			catch (Exception e)
			{
				throw new WammerCloudException("Wammer cloud error", json, e);
			}

			CloudResponse cres = resObj as CloudResponse;
			if (cres != null)
			{
				if (cres.status != 200 || cres.api_ret_code != 0)
					throw new WammerCloudException("Wammer cloud error", json,
						cres.api_ret_code);
			}

			return resObj;
		}

		public static bool IsNetworkError(WammerCloudException e)
		{
			return (e.InnerException != null && e.InnerException is WebException && e.HttpError != WebExceptionStatus.ProtocolError);
		}

		public static bool IsSessionError(WammerCloudException e)
		{
			WebException webex = (WebException)e.InnerException;
			if (webex != null)
			{
				HttpWebResponse response = (HttpWebResponse)webex.Response;
				if (response != null)
				{
					if (response.StatusCode == HttpStatusCode.Unauthorized)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}

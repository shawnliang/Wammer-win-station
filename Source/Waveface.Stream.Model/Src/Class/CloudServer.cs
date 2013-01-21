using fastJSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;

namespace Waveface.Stream.Model
{
	public class CloudServer
	{
		public const string DEF_BASE_PATH = "v3";
		public const string DEF_BASE_URL = "https://develop.waveface.com/v3/";

		public const string PARAM_DETAIL = "detail";
		public const string PARAM_API_KEY = "apikey";
		public const string PARAM_EMAIL = "email";
		public const string PARAM_PASSWORD = "password";
		public const string PARAM_DEVICE_ID = "device_id";
		public const string PARAM_DEVICE_NAME = "device_name";
		public const string PARAM_SESSION_TOKEN = "session_token";
		public const string PARAM_STATION_ID = "station_id";
		public const string PARAM_LOCATION = "location";
		public const string PARAM_USER_ID = "user_id";
		public const string PARAM_USER_FOLDER = "user_folder";
		public const string PARAM_LIMIT = "limit";
		public const string PARAM_FILTER_ENTITY = "filter_entity";
		public const string PARAM_POST_ID_LIST = "post_id_list";
		public const string PARAM_GROUP_ID = "group_id";
		public const string PARAM_CONTENT = "content";
		public const string PARAM_OBJECT_ID = "object_id";
		public const string PARAM_OBJECT_ID_LIST = "object_id_list";
		public const string PARAM_OBJECT_IDS = "object_ids";
		public const string PARAM_TYPE = "type";
		public const string PARAM_ATTACHMENT_ID_ARRAY = "attachment_id_array";
		public const string PARAM_PREVIEW = "preview";
		public const string PARAM_IMAGE_META = "image_meta";
		public const string PARAM_POST_ID = "post_id";
		public const string PARAM_LAST_UPDATE_TIME = "last_update_time";
		public const string PARAM_TIMESTAMP = "timestamp";
		public const string PARAM_COVER_ATTACH = "cover_attach";
		public const string PARAM_FAVORITE = "favorite";
		public const string PARAM_UPDATE_TIME = "update_time";
		public const string PARAM_IMPORT = "import";
		public const string PARAM_REMOVE_ALL_DATA = "remove_resource";
		public const string PARAM_COUNT = "count";
		public const string PARAM_TARGET = "target";
		public const string PARAM_DATUM = "datum";
		public const string PARAM_SNS = "sns";
		public const string PARAM_PURGE_ALL = "purge_all";
		public const string PARAM_METADATA = "metadata";
		public const string PARAM_COMPONENT_OPTIONS = "component_options";
		public const string PARAM_NAME = "name";
		public const string PARAM_COLLECTION_ID = "collection_id";
		public const string PARAM_MODIFY_TIME = "modify_time";
		public const string PARAM_COLLECTION_ID_LIST = "collection_id_list";
		public static Dictionary<string, string> CodeName = new Dictionary<string, string>
		                                                    	{
		                                                    		{"0ffd0a63-65ef-512b-94c7-ab3b33117363", "Station"},
		                                                    		{"e96546fa-3ed5-540a-9ef2-1f8ce1dc60f2", "Android"},
		                                                    		{"ca5c3c5c-287d-5805-93c1-a6c2cbf9977c", "iPhone"},
		                                                    		{"ba15e628-44e6-51bc-8146-0611fdfa130b", "iPad"},
		                                                    		{"a23f9491-ba70-5075-b625-b8fb5d9ecd90", "Windows"},
		                                                    		{"fdda2704-dcd5-5b0c-a676-8a46813068d3", "Windows Phone"},
		                                                    		{"c0870e40-1416-5d88-9014-bc089832ebd8", "Android Tablet"},
		                                                    		{"74ab96e4-06b3-5307-bf05-21ed5b0a2e11", "Automation"}
		                                                    	};

		private static string apiKey;
		private static string baseUrl;

		private static bool isOffline;
		public static string SessionToken { get; set; }


		//private static readonly ILog logger = LogManager.GetLogger("CloudServer");

		/// <summary>
		/// Gets or sets wammer cloud base url
		/// </summary>
		public static string BaseUrl
		{
			get { return baseUrl ?? (baseUrl = (string)StationRegistry.GetValue("cloudBaseURL", DEF_BASE_URL)); }
			set { baseUrl = value; }
		}

		public static CloudType Type
		{
			get
			{
				if (BaseUrl.Contains("develop.waveface.com"))
					return CloudType.Development;
				else if (BaseUrl.Contains("staging.waveface.com"))
					return CloudType.Staging;
				else
					return CloudType.Production;
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

				return (string)StationRegistry.GetValue("cloudAPIKey", GetDefaultAPIKey());
			}
			set { apiKey = value; }
		}

		private static String GetDefaultAPIKey()
		{
			//0ffd0a63-65ef-512b-94c7-ab3b33117363

			var buffer = new byte[36];
			buffer[0] = 0x30;
			buffer[1] = 0x66;
			buffer[2] = 0x66;
			buffer[3] = 0x64;
			buffer[4] = 0x30;
			buffer[5] = 0x61;
			buffer[6] = 0x36;
			buffer[7] = 0x33;
			buffer[8] = 0x2d;
			buffer[9] = 0x36;
			buffer[10] = 0x35;
			buffer[11] = 0x65;
			buffer[12] = 0x66;
			buffer[13] = 0x2d;
			buffer[14] = 0x35;
			buffer[15] = 0x31;
			buffer[16] = 0x32;
			buffer[17] = 0x62;
			buffer[18] = 0x2d;
			buffer[19] = 0x39;
			buffer[20] = 0x34;
			buffer[21] = 0x63;
			buffer[22] = 0x37;
			buffer[23] = 0x2d;
			buffer[24] = 0x61;
			buffer[25] = 0x62;
			buffer[26] = 0x33;
			buffer[27] = 0x62;
			buffer[28] = 0x33;
			buffer[29] = 0x33;
			buffer[30] = 0x31;
			buffer[31] = 0x31;
			buffer[32] = 0x37;
			buffer[33] = 0x33;
			buffer[34] = 0x36;
			buffer[35] = 0x33;

			return Encoding.UTF8.GetString(buffer);
		}

		public static bool VersionNotCompatible { get; set; }

		public static void requestDownload(string path, Dictionary<object, object> parameters,
										   string filepath)
		{
			var buf = new StringBuilder();

			long beginTime = Environment.TickCount;
			try
			{
				foreach (var pair in parameters)
				{
					if (PARAM_SESSION_TOKEN.Equals(pair.Key) && string.Empty.Equals(pair.Value))
						throw new WammerCloudException("session token is null", WebExceptionStatus.ProtocolError, (int)GeneralApiError.SessionNotExist);

					buf.Append(HttpUtility.UrlEncode(pair.Key.ToString()));
					buf.Append("=");
					buf.Append(HttpUtility.UrlEncode(pair.Value.ToString()));
					buf.Append("&");
				}

				// remove last &
				buf.Remove(buf.Length - 1, 1);
				using (var agent = new DefaultWebClient())
				{
					//logger.DebugFormat("DownloadFile({0}, {1}, true,...)", baseUrl + path + "?" + buf, filepath);
					addVersionToHttpHeader(agent);
					agent.DownloadFile(baseUrl + path + "?" + buf, filepath, true, (sender, e) => PerfCounter.GetCounter(PerfCounter.DWSTREAM_RATE).IncrementBy(
											long.Parse(e.UserState.ToString())));
				}
			}
			catch (WebException e)
			{
				throw GetWammerCloudException(beginTime, e);
			}
			catch (Exception e)
			{
				long elapsed_ms = Environment.TickCount - beginTime;
				throw new WammerCloudException("Wammer cloud error (elapsed ms = " + elapsed_ms + ")", e);
			}
		}

		private static void addVersionToHttpHeader(WebClient agent)
		{
			agent.Headers.Add("Waveface-Stream", "WIN Station/" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
		}

		/// <summary>
		/// Requests Wammer cloud via http post
		/// </summary>
		/// <param name="path">partial path of cloud url, http://host:port/base/partial_path</param>
		/// <param name="parms">request parameter names and values.
		/// They will be URLEncoded and transformed to name1=val1&amp;name2=val2...</param>
		/// <param name="checkOffline">if set to <c>true</c> [check offline].</param>
		/// <param name="autoRedirectRequest">if set to <c>true</c> [auto redirect request].</param>
		/// <returns>Response value</returns>
		public static string requestPath(string path, Dictionary<object, object> parms,
										 bool checkOffline = true, Boolean autoRedirectRequest = true)
		{
			return requestPath(BaseUrl, path, parms, checkOffline, autoRedirectRequest);
		}

		/// <summary>
		/// Requests the path.
		/// </summary>
		/// <param name="baseUrl">The base URL.</param>
		/// <param name="path">The path.</param>
		/// <param name="parms">The parms.</param>
		/// <param name="checkOffline">if set to <c>true</c> [check offline].</param>
		/// <param name="autoRedirectRequest">if set to <c>true</c> [auto redirect request].</param>
		/// <param name="timeout">The timeout.</param>
		/// <returns></returns>
		public static string requestPath(string baseUrl, string path, Dictionary<object, object> parms,
										 bool checkOffline = true, Boolean autoRedirectRequest = true, int timeout = -1)
		{
			if (checkOffline)
			{
				if (isOffline)
				{
					throw new WammerCloudException("Station is in offline mode",
												   new WebException("Station is in offline mode", WebExceptionStatus.ConnectFailure));
				}
			}

			string url = baseUrl + path;

			try
			{
				string res = request(url, parms, autoRedirectRequest, timeout);
				isOffline = false;
				return res;
			}
			catch (WammerCloudException e)
			{
				isOffline = IsNetworkError(e);
				throw;
			}
		}

		/// <summary>
		/// Requests Wammer cloud via http post
		/// </summary>
		/// <typeparam name="T">response type</typeparam>
		/// <param name="path">partial path of cloud url, http://host:port/base/partial_path</param>
		/// <param name="parms">request parameter names and values.
		/// They will be URLEncoded and transformed to name1=val1&amp;name2=val2...</param>
		/// <param name="checkOffline">if set to <c>true</c> [check offline].</param>
		/// <param name="autoRedirectRequest">if set to <c>true</c> [auto redirect request].</param>
		/// <returns>Response value</returns>
		public static T requestPath<T>(string path, Dictionary<object, object> parms,
									   bool checkOffline = true, Boolean autoRedirectRequest = true)
		{
			if (checkOffline)
			{
				if (isOffline)
				{
					throw new WammerCloudException("Station is in offline mode",
												   new WebException("Station is in offline mode", WebExceptionStatus.ConnectFailure));
				}
			}

			try
			{
				var res = ConvertFromJson<T>(requestPath(path, parms, false, autoRedirectRequest));
				isOffline = false;
				return res;
			}
			catch (WammerCloudException e)
			{
				isOffline = IsNetworkError(e);
				throw;
			}
		}

		public static T request<T>(string url, Dictionary<object, object> param, bool isGet, bool checkOffline = true, int timeout = -1)
		{
			if (checkOffline)
			{
				if (isOffline)
				{
					throw new WammerCloudException("Station is in offline mode",
												   new WebException("Station is in offline mode", WebExceptionStatus.ConnectFailure));
				}
			}

			if (param.Count == 0)
				return request<T>(url, "", isGet, timeout);

			var buf = new StringBuilder();
			foreach (var pair in param)
			{
				if (PARAM_SESSION_TOKEN.Equals(pair.Key) && string.Empty.Equals(pair.Value))
					throw new WammerCloudException("session token is null", WebExceptionStatus.ProtocolError, (int)GeneralApiError.SessionNotExist);

				buf.Append(HttpUtility.UrlEncode(pair.Key.ToString()));
				buf.Append("=");
				buf.Append(HttpUtility.UrlEncode(pair.Value.ToString()));
				buf.Append("&");
			}

			// remove last &
			buf.Remove(buf.Length - 1, 1);

			try
			{
				var res = request<T>(url, buf.ToString(), isGet, timeout);
				isOffline = false;
				return res;
			}
			catch (WammerCloudException e)
			{
				isOffline = IsNetworkError(e);
				throw;
			}
		}

		private static T request<T>(string url, string parameters, bool isGet, int timeout = -1)
		{
			string response = "";
			T resObj;

			long beginTime = Environment.TickCount;

			try
			{
				byte[] rawResponse;
				using (var agent = new DefaultWebClient())
				{
					agent.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
					addVersionToHttpHeader(agent);

					if (timeout > 0)
					{
						agent.Timeout = timeout;
						agent.ReadWriteTimeout = timeout;
					}

					if (isGet)
					{
						rawResponse = string.IsNullOrEmpty(parameters)
										? agent.DownloadData(url)
										: agent.DownloadData(url + "?" + parameters);
					}
					else
					{
						rawResponse = agent.UploadData(url, "POST", Encoding.UTF8.GetBytes(parameters));
					}
				}

				Debug.Assert(rawResponse != null, "rawResponse != null");
				response = Encoding.UTF8.GetString(rawResponse);
				resObj = JSON.Instance.ToObject<T>(response);
			}
			catch (WebException e)
			{
				throw GetWammerCloudException(beginTime, e);
			}
			catch (Exception e)
			{
				long elapsed_ms = Environment.TickCount - beginTime;
				throw new WammerCloudException("Wammer cloud error (elapsed ms = " + elapsed_ms + ")", e);
			}

			var cres = resObj as CloudResponse;
			if (cres != null)
			{
				if (cres.status != 200 || cres.api_ret_code != 0)
					throw new WammerCloudException("Wammer cloud error", response,
												   cres.api_ret_code);
			}


			return resObj;
		}

		public static string request(string url, Dictionary<object, object> param, Boolean autoRedirectRequest = true, int timeout = -1)
		{
			if (param.Count == 0)
				return request(url);

			var buf = new StringBuilder();
			foreach (var pair in param)
			{
				if (PARAM_SESSION_TOKEN.Equals(pair.Key) && string.Empty.Equals(pair.Value))
					throw new WammerCloudException("session token is null", WebExceptionStatus.ProtocolError, (int)GeneralApiError.SessionNotExist);

				buf.Append(HttpUtility.UrlEncode(pair.Key.ToString()));
				buf.Append("=");
				buf.Append(HttpUtility.UrlEncode(Convert.ToString(pair.Value)));
				buf.Append("&");
			}

			// remove last &
			buf.Remove(buf.Length - 1, 1);

			return request(url, buf.ToString(), autoRedirectRequest, timeout);
		}

		public static T request<T>(string url, Dictionary<object, object> param, bool checkOffline = true)
		{
			if (checkOffline)
			{
				if (isOffline)
				{
					throw new WammerCloudException("Station is in offline mode",
												   new WebException("Station is in offline mode", WebExceptionStatus.ConnectFailure));
				}
			}

			try
			{
				var res = ConvertFromJson<T>(request(url, param));
				isOffline = false;
				return res;
			}
			catch (WammerCloudException e)
			{
				isOffline = IsNetworkError(e);
				throw;
			}
		}

		private static string request(string url, string postData = "", Boolean autoRedirectRequest = true, int timeout = -1)
		{
			long beginTime = Environment.TickCount;

			try
			{
				byte[] rawResponse;
				using (var agent = new DefaultWebClient())
				{
					agent.AllowAutoRedirect = autoRedirectRequest;
					if (timeout > 0)
					{
						agent.Timeout = timeout;
						agent.ReadWriteTimeout = timeout;
					}

					agent.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
					addVersionToHttpHeader(agent);

					rawResponse = agent.UploadData(url, "POST", Encoding.UTF8.GetBytes(postData));
				}
				return Encoding.UTF8.GetString(rawResponse);
			}
			catch (WebException e)
			{
				throw GetWammerCloudException(beginTime, e);
			}
		}

		private static WammerCloudException GetWammerCloudException(long beginTime, WebException e)
		{
			long elapsed_ms = Environment.TickCount - beginTime;
			var ex = new WammerCloudException("Wammer cloud error (elapsed ms = " + elapsed_ms + ")", e);

			if (ex.WammerError == (int)GeneralApiError.NotSupportClient)
				CloudServer.VersionNotCompatible = true;

			return ex;
		}

		public static T ConvertFromJson<T>(string json)
		{
			T resObj;
			try
			{
				resObj = JSON.Instance.ToObject<T>(json);
			}
			catch (Exception e)
			{
				throw new WammerCloudException("Wammer cloud error", json, e);
			}

			var cres = resObj as CloudResponse;
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
			return (e.InnerException != null && e.InnerException is WebException &&
					e.HttpError != WebExceptionStatus.ProtocolError);
		}

		public static bool IsSessionError(WammerCloudException e)
		{
			if (e.WammerError != 0)
			{
				return (e.WammerError == (int)GeneralApiError.SessionNotExist);
			}
			var webex = (WebException)e.InnerException;
			if (webex != null)
			{
				var response = (HttpWebResponse)webex.Response;
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
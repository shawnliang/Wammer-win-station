using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using Waveface.Stream.Model;
using Newtonsoft.Json;
using System.Threading;

namespace Waveface.Stream.Core
{
	/// <summary>
	/// 
	/// </summary>
	[Obfuscation]
	public static class StationAPI
	{
		#region Const
		public static string API_KEY = "a23f9491-ba70-5075-b625-b8fb5d9ecd90";
		private const string STATION_MGMT_URLBASE = "http://127.0.0.1:9989/v3";
		private const string STATION_FUNC_URLBASE = "http://127.0.0.1:9981/v3";
		#endregion


		#region Private Static Method
		/// <summary>
		/// Toes the query string.
		/// </summary>
		/// <param name="nvc">The NVC.</param>
		/// <returns></returns>
		private static string ToQueryString(NameValueCollection nvc)
		{
			DebugInfo.ShowMethod();

			return string.Join("&", Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
		}

		public static string Post(string uri, NameValueCollection parameters, int timeout = 0, int readWriteTimeout = 0)
		{
			DebugInfo.ShowMethod();

			var queryString = ToQueryString(parameters);
			var data = Encoding.Default.GetBytes(queryString);
			var request = WebRequest.Create(uri) as HttpWebRequest;

			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = data.Length;

			if (timeout > 0)
				request.Timeout = timeout;

			if (readWriteTimeout > 0)
				request.ReadWriteTimeout = readWriteTimeout;

			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(data, 0, data.Length);
			}

			var response = request.GetResponse();
			// Get the stream containing content returned by the server.
			var dataStream = response.GetResponseStream();
			// Open the stream using a StreamReader for easy access.
			using (var sr = new StreamReader(dataStream))
			{
				var responseMsg = sr.ReadToEnd();
				return responseMsg;
			}
		}
		#endregion


		#region Public Static Method
		public static void MoveFolder(string user_id, string folder_path, string session_token)
		{
			DebugInfo.ShowMethod();

			var url = STATION_MGMT_URLBASE + @"/station/moveFolder";

			var parameters = new NameValueCollection()
				{
					{"apikey", API_KEY},
					{"session_token", session_token},
					{"user_id", user_id},
					{"user_folder", folder_path}
				};

			Post(url, parameters, 30 * 60 * 1000, 30 * 60 * 1000);
		}

		public static string SuspendSync()
		{
			DebugInfo.ShowMethod();

			var uri = STATION_MGMT_URLBASE + @"/station/suspendSync";

			return Post(uri, new NameValueCollection(){
						{ "apikey", API_KEY }
					});
		}

		public static string ResumeSync()
		{
			DebugInfo.ShowMethod();

			var uri = STATION_MGMT_URLBASE + @"/station/resumeSync";

			return Post(uri, new NameValueCollection(){
						{ "apikey", API_KEY }
					});
		}

		public static ImportResponse ImportPhoto(string sessionToken, string groupID, IEnumerable<string> paths)
		{
			DebugInfo.ShowMethod();

			var url = STATION_MGMT_URLBASE + @"/station/Import";

			var pathObj = new ImportMsg { files = paths.ToList() };
			var pathStr = JsonConvert.SerializeObject(pathObj);

			var parameters = new NameValueCollection() 
				{
					{"apikey", API_KEY},
					{"session_token", sessionToken},
					{"group_id", groupID},
					{"paths", pathStr}
				};

			var response = Post(url, parameters);
			return JsonConvert.DeserializeObject<ImportResponse>(response);
		}

		public static string ImportDoc(string sessionToken, IEnumerable<string> paths)
		{
			DebugInfo.ShowMethod();

			var url = STATION_MGMT_URLBASE + @"/station/ImportDoc";

			var parameters = new NameValueCollection() 
				{
					{"apikey", API_KEY},
					{"session_token", sessionToken},
					{"paths", string.Join(",", paths.ToArray())}
				};

			return Post(url, parameters);
		}

		public static string SNSDisconnect(string sessionToken, string sns)
		{
			DebugInfo.ShowMethod();

			var url = STATION_FUNC_URLBASE + @"/users/SNSDisconnect";

			var parameters = new NameValueCollection() 
				{
					{"apikey", API_KEY},
					{"session_token", sessionToken},
                    {"sns", sns},
                    {"purge_all", "no"}
				};

			return Post(url, parameters);
		}

		public static string UpdateUser(string sessionToken, string userID, Boolean subscribed)
		{
			DebugInfo.ShowMethod();

			var url = STATION_FUNC_URLBASE + @"/users/update";

			var parameters = new NameValueCollection() 
				{
					{"apikey", API_KEY},
					{"session_token", sessionToken},
                    {"user_id", userID},
                    {"subscribed", (subscribed ? "yes" : "no")}
				};

			return Post(url, parameters);
		}

		public static string UpdateUser(string sessionToken, string userID, string email = null, string nickName = null, string avatarUrl = null)
		{
			DebugInfo.ShowMethod();

			var url = STATION_FUNC_URLBASE + @"/users/update";

			var parameters = new NameValueCollection() 
				{
					{"apikey", API_KEY},
					{"session_token", sessionToken},
                    {"user_id", userID}
				};

			if (!string.IsNullOrEmpty(email))
				parameters.Add("email", email);

			if (!string.IsNullOrEmpty(nickName))
				parameters.Add("nickname", nickName);

			if (!string.IsNullOrEmpty(avatarUrl))
				parameters.Add("avatar_url", avatarUrl);

			return Post(url, parameters);
		}

		public static string GetUser(string sessionToken, string userID, int timeout = 0, int readWriteTimeout = 0)
		{
			DebugInfo.ShowMethod();

			var url = STATION_FUNC_URLBASE + @"/users/get";

			var parameters = new NameValueCollection() 
				{
					{"apikey", API_KEY},
					{"session_token", sessionToken},
                    {"user_id", userID}
				};

			return Post(url, parameters, timeout, readWriteTimeout);
		}


		public static string AddUser(string email, string password, string deviceId, string deviceName, string userFolder)
		{
			DebugInfo.ShowMethod();

			var uri = STATION_MGMT_URLBASE + @"/station/drivers/add";

			return Post(uri, new NameValueCollection(){
						{ "email", email},
						{ "password", password},
						{ "device_id", deviceId},
						{ "device_name", deviceName},
						{ "user_folder", userFolder}
					});
		}

		public static string AddUser(string userID, string sessionToken, string userFolder)
		{
			DebugInfo.ShowMethod();

			var uri = STATION_MGMT_URLBASE + @"/station/drivers/add";

			return Post(uri, new NameValueCollection(){
						{ "user_id", userID},
						{ "session_token", sessionToken},
						{ "user_folder", userFolder}
					});
		}

		public static string RemoveUser(string userId, Boolean removeResource = true)
		{
			DebugInfo.ShowMethod();

			var uri = STATION_MGMT_URLBASE + @"/station/drivers/remove";

			return Post(uri, new NameValueCollection(){
                        { "apikey", API_KEY},
						{ "user_id", userId},
						{ "remove_resource", removeResource.ToString()}
					});
		}

		public static string Login(string email, string password,
									   string deviceId, string deviceName)
		{
			DebugInfo.ShowMethod();

			var uri = STATION_FUNC_URLBASE + @"/auth/login";

			return Post(uri, new NameValueCollection(){
						{ "apikey", API_KEY},
						{ "email", email},
						{ "password", password},
						{ "device_id", deviceId},
						{ "device_name", deviceName}
					});
		}

		public static string Login(string userID, string sessionToken)
		{
			DebugInfo.ShowMethod();

			var uri = STATION_FUNC_URLBASE + @"/auth/login";

			return Post(uri, new NameValueCollection(){
						{ "apikey", API_KEY},
						{ "user_id", userID},
						{ "session_token", sessionToken}
					});
		}

		public static void AddMonitorFile(string file, string user_id, string sesion_token)
		{
			var uri = STATION_MGMT_URLBASE + @"/station/monitor/add";

			var parameters = new NameValueCollection()
				{
					{"apikey", API_KEY},
					{"session_token", sesion_token},
					{"user_id", user_id},
					{"file", file}
				};

			Post(uri, parameters);
		}

		public static string Logout(string sessionToken)
		{
			DebugInfo.ShowMethod();

			var uri = STATION_FUNC_URLBASE + @"/auth/logout";

			return Post(uri, new NameValueCollection(){
						{ "apikey", API_KEY},
						{ "session_token", sessionToken}
					});
		}

		public static string DeleteAccount(string sessionToken)
		{
			DebugInfo.ShowMethod();

			var uri = STATION_FUNC_URLBASE + @"/users/deleteWithEmail";

			return Post(uri, new NameValueCollection(){
						{ "session_token", sessionToken}
					});
		}

		public static string CreateCollection(string sessionToken, string name, IEnumerable<string> attachmentIDs, string id = null, string coverAttachID = null, bool? isManualCreated = null, DateTime? timeStamp = null)
		{
			DebugInfo.ShowMethod();

			var uri = STATION_FUNC_URLBASE + @"/collections/create";

			if (timeStamp == null)
				timeStamp = DateTime.Now;

			var id_list = "[" + String.Join(",", attachmentIDs.Select((x) => "\"" + x + "\"").ToArray()) + "]";

			var parameters = new NameValueCollection(){
						{ "apikey", API_KEY},
						{ "session_token", sessionToken},
						{ "name", name},
						{ "object_id_list", id_list},
						{ "create_time", timeStamp.Value.ToUTCISO8601ShortString()}
					};

			if (!string.IsNullOrEmpty(id))
				parameters.Add("collection_id", id);

			if (!string.IsNullOrEmpty(coverAttachID))
				parameters.Add("cover", coverAttachID);

			if (isManualCreated.HasValue)
				parameters.Add("manual", isManualCreated.Value.ToString().ToLower());

			return Post(uri, parameters);
		}


		public static string HideCollection(string sessionToken, string id, DateTime? timeStamp = null)
		{
			DebugInfo.ShowMethod();

			var uri = STATION_FUNC_URLBASE + @"/collections/hide";

			if (timeStamp == null)
				timeStamp = DateTime.Now;

			var parameters = new NameValueCollection(){
						{ "apikey", API_KEY},
						{ "session_token", sessionToken},
						{ "collection_id", id},
						{ "modify_time", timeStamp.Value.ToUTCISO8601ShortString()}
					};

			return Post(uri, parameters);
		}


		public static string UnHideCollection(string sessionToken, string id, DateTime? timeStamp = null)
		{
			DebugInfo.ShowMethod();

			var uri = STATION_FUNC_URLBASE + @"/collections/unhide";

			if (timeStamp == null)
				timeStamp = DateTime.Now;

			var parameters = new NameValueCollection(){
						{ "apikey", API_KEY},
						{ "session_token", sessionToken},
						{ "collection_id", id},
						{ "modify_time", timeStamp.Value.ToUTCISO8601ShortString()}
					};

			return Post(uri, parameters);
		}

		public static string UpdateCollection(string sessionToken, string id, string name = null, IEnumerable<string> attachmentIDs = null, string coverAttachID = null, bool? hidden = null, DateTime? timeStamp = null)
		{
			DebugInfo.ShowMethod();

			var uri = STATION_FUNC_URLBASE + @"/collections/update";

			if (timeStamp == null)
				timeStamp = DateTime.Now;

			var parameters = new NameValueCollection(){
						{ "apikey", API_KEY},
						{ "session_token", sessionToken},
						{ "collection_id", id},
						{ "modify_time", timeStamp.Value.ToUTCISO8601ShortString()}
					};

			if (!string.IsNullOrEmpty(name))
				parameters.Add("name", name);

			if (attachmentIDs != null && attachmentIDs.Any())
			{
				var id_list = "[" + String.Join(",", attachmentIDs.Select((x) => "\"" + x + "\"").ToArray()) + "]";
				parameters.Add("object_id_list", id_list);
			}

			if (!string.IsNullOrEmpty(coverAttachID))
				parameters.Add("cover", coverAttachID);

			if (hidden.HasValue)
				parameters.Add("hidden", hidden.Value.ToString());

			return Post(uri, parameters);
		}

		#endregion
	}
}

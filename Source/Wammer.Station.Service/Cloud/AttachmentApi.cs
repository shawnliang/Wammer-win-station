using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Wammer.Model;
using Wammer.Station;
using Wammer.Utility;
using Waveface.Stream.Model;


namespace Wammer.Cloud
{
	public class AttachmentApi
	{
		public const string TMPQUEUE = "tmpqueue";
		#region Location enum

		public enum Location
		{
			Cloud = 0,
			Station = 1,
			Dropbox = 2
		}

		#endregion

		private static readonly ILog logger = LogManager.GetLogger("AttachmentApi");

		public AttachmentApi(Driver driver)
		{
			userToken = driver.session_token;
		}

		private string userToken { get; set; }


		public void AttachmentView(ResourceDownloadEventArgs evtargs, string stationId)
		{
			Debug.Assert(!String.IsNullOrEmpty(evtargs.attachment.object_id));
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_OBJECT_ID, evtargs.attachment.object_id},
			                 		{CloudServer.PARAM_SESSION_TOKEN, userToken},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
			                 		{CloudServer.PARAM_STATION_ID, stationId}
			                 	};

			if (evtargs.attachment.type.Equals("image") && !evtargs.IsOriginalAttachment())
			{
				parameters.Add(CloudServer.PARAM_IMAGE_META, evtargs.imagemeta.ToString().ToLower());
			}

			CloudServer.requestDownload("attachments/view", parameters, evtargs.filepath);
		}

		public static AttachmentRedirectInfo GetImageMetadata(string objectId, string session_token, string apikey,
														   ImageMeta meta, string station_id)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{"object_id", objectId},
			                 		{"session_token", session_token},
			                 		{"station_id", station_id},
			                 		{"apikey", apikey},
			                 		{"return_meta", "true"}
			                 	};

			if (meta != ImageMeta.Origin && meta != ImageMeta.None)
				parameters.Add("image_meta", meta.ToString().ToLower());

			return CloudServer.requestPath<AttachmentRedirectInfo>("attachments/view", parameters, true, false);
		}

		public static DownloadResult DownloadObject(string url, AttachmentInfo metaData = null)
		{
			using (var agent = new DefaultWebClient())
			{
				var data = agent.DownloadData(url);
				var contentType = agent.ResponseHeaders["Content-type"];

				return new DownloadResult(data, metaData, contentType);
			}
		}

		public static DownloadResult DownloadObject(string url, string save_path, AttachmentInfo metaData = null)
		{
			using (var agent = new DefaultWebClient())
			{
				agent.DownloadFile(url, save_path);
				var contentType = agent.ResponseHeaders["Content-type"];

				return new DownloadResult(null, metaData, contentType);
			}
		}

		public static AttachmentInfo GetInfo(string object_id, string session_token)
		{
			if (object_id == null || session_token == null)
				throw new ArgumentNullException();

			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_OBJECT_ID, object_id},
			                 		{CloudServer.PARAM_SESSION_TOKEN, session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			return CloudServer.requestPath<AttachmentInfo>("attachments/get", parameters, false);
		}

		public static void UploadMetadata(string session_token, string apikey, string metadata)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_METADATA, metadata},
			                 		{CloudServer.PARAM_SESSION_TOKEN, session_token},
			                 		{CloudServer.PARAM_API_KEY, apikey}
			                 	};

			CloudServer.requestPath<CloudResponse>("attachments/upload_metadata", parameters, false);
		}

		public static ObjectUploadResponse Upload(Stream dataStream, string groupId,
												  string objectId, string fileName, string contentType,
												  ImageMeta meta, AttachmentType type, string apiKey,
												  string token, int bufferSize = 1024,
												  Action<object, ProgressChangedEventArgs> progressChangedCallBack = null,
												  string post_id = null, string file_path = null, exif exif = null, DateTime? import_time = null, int? timezone = null, DateTime? file_create_time = null, DocProperty doc_meta = null)
		{
			try
			{
				if (token == null)
					throw new WammerCloudException("session token is null", WebExceptionStatus.ProtocolError, (int)GeneralApiError.SessionNotExist);

				Dictionary<string, object> pars = GetAdditionalParams(groupId, objectId, meta, type, apiKey, token, post_id, file_path, exif, import_time, timezone, file_create_time, doc_meta);
				HttpWebResponse _webResponse = Waveface.MultipartFormDataPostHelper.MultipartFormDataPost(
					CloudServer.BaseUrl + "attachments/upload",
					"Mozilla 4.0+",
					pars,
					fileName,
					contentType,
					dataStream, bufferSize, progressChangedCallBack);

				Debug.Assert(_webResponse != null, "_webResponse != null");
				using (var reader = new StreamReader(_webResponse.GetResponseStream()))
				{
					return fastJSON.JSON.Instance.ToObject<ObjectUploadResponse>(reader.ReadToEnd());
				}
			}
			catch (WebException e)
			{
				throw new WammerCloudException("Wammer cloud error", e);
			}
		}

		private static Dictionary<string, object> GetAdditionalParams(string groupId, string objectId, ImageMeta meta,
			AttachmentType type, string apiKey, string token, string post_id = null, string file_path = null, exif exif = null, DateTime? import_time = null, int? timezone = null,
			DateTime? file_create_time = null, DocProperty doc_meta = null)
		{
			var pars = new Dictionary<string, object>();

			pars["type"] = type.ToString();

			if (meta != ImageMeta.None)
				pars["image_meta"] = meta.ToString().ToLower();

			pars["session_token"] = token;
			pars["apikey"] = apiKey;

			if (objectId != null)
				pars["object_id"] = objectId;

			pars["group_id"] = groupId;

			if (!string.IsNullOrEmpty(post_id))
				pars["post_id"] = post_id;

			if (!string.IsNullOrEmpty(file_path))
				pars["file_path"] = file_path;

			if (import_time.HasValue)
				pars["import_time"] = import_time.Value.ToUTCISO8601ShortString();

			if (exif != null)
				pars["exif"] = JsonConvert.SerializeObject(exif, Formatting.Indented);

			if (timezone.HasValue)
				pars["timezone"] = timezone.Value;

			if (file_create_time.HasValue && file_create_time.Value > DateTime.MinValue)
				pars["file_create_time"] = file_create_time.Value.ToUTCISO8601ShortString();

			if (doc_meta != null)
				pars["doc_meta"] = doc_meta.ToFastJSON();

			return pars;
		}

		public static void updateDocMetadata(string session, string apikey, string object_id, DateTime accessTime)
		{
			var meta = new
			{
				object_id = object_id,
				type = "doc",
				access_time = accessTime.ToUTCISO8601ShortString()
			};

			var parameters = new Dictionary<object, object>
			{
				{CloudServer.PARAM_METADATA, JsonConvert.SerializeObject(meta)},
				{CloudServer.PARAM_SESSION_TOKEN, session},
				{CloudServer.PARAM_API_KEY, apikey}
			};

			CloudServer.requestPath<CloudResponse>("attachments/update_metadata", parameters, false);
		}


		public static AttachmentSearchResult Search(string session, string apikey, DateTime since, DateTime until, int count = 100, int start = 0)
		{
			var parameters = new Dictionary<object, object>
			{
				{"modify_time_since", since.ToUTCISO8601ShortString()},
				{"modify_time_until", until.ToUTCISO8601ShortString()},
				{"start", start},
				{"count", count},
				{"views", "all"},
				{CloudServer.PARAM_SESSION_TOKEN, session},
				{CloudServer.PARAM_API_KEY, apikey}
			};

			return CloudServer.requestPath<AttachmentSearchResult>("attachments/search", parameters, false);
		}

		public static void DownloadWebThumb(string session_token, string apikey, string object_id, long webthumb_id, string save_path)
		{
			var url = string.Format("{0}attachments/view/?object_id={1}&target=preview&id={2}&session_token={3}&apikey={4}",
				CloudServer.BaseUrl,
				object_id,
				webthumb_id,
				System.Web.HttpUtility.UrlEncode(session_token),
				apikey);

			using (var agent = new DefaultWebClient())
			{
				agent.DownloadFile(url, save_path);
			}
		}
	}


	public class AttachmentSearchResult
	{
		public int total_count { get; set; }
		public int results_count { get; set; }
		public string next_page { get; set; }
		public List<AttachmentInfo> results { get; set; }
	}

	public class AttachmentRedirectInfo : AttachmentInfo
	{
		public string redirect_to { get; set; }
	}


	public class DownloadResult
	{
		public DownloadResult(byte[] Image, AttachmentInfo metadata, string contentType)
		{
			this.Image = Image;
			Metadata = metadata;
			ContentType = contentType ?? "application/octet-stream";
		}

		public byte[] Image { get; private set; }
		public AttachmentInfo Metadata { get; private set; }
		public string ContentType { get; private set; }
	}

	public class AttachmentQueueResponse
	{
		public int total_results { get; set; }
		public int counts { get; set; }
		public List<string> objects { get; set; }
	}
}

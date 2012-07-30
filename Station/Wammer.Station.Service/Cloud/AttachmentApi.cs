using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using log4net;
using Wammer.Model;
using Wammer.Station;
using Wammer.Station.JSONClass;
using Wammer.Utility;
using System.Diagnostics;

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

		public void AttachmentSetLoc(int loc, string object_id, string file_path)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{"loc", loc},
			                 		{"object_id", object_id},
			                 		{"file_path", file_path},
			                 		{CloudServer.PARAM_SESSION_TOKEN, userToken},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			CloudServer.requestPath<CloudResponse>("attachments/setloc", parameters);
		}

		public void AttachmentUnsetLoc(int loc, string object_id)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{"loc", loc},
			                 		{"object_id", object_id},
			                 		{CloudServer.PARAM_SESSION_TOKEN, userToken},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.PARAM_API_KEY}
			                 	};

			CloudServer.requestPath<CloudResponse>("attachments/unsetloc", parameters);
		}

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

			if (evtargs.imagemeta != ImageMeta.Origin)
			{
				parameters.Add(CloudServer.PARAM_IMAGE_META, evtargs.imagemeta.ToString().ToLower());
			}

			CloudServer.requestDownload("attachments/view", parameters, evtargs.filepath);
		}

		public static AttachmentView GetImageMetadata(string objectId, string session_token, string apikey,
														   ImageMeta meta, string station_id)
		{
			Debug.WriteLine("session_token: " + session_token);
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

			return CloudServer.requestPath<AttachmentView>("attachments/view", parameters, true, false);
		}

		public static void SaveImageFromMetaData(AttachmentView metadata,string file,ref string contentType,
															   Action<object, ProgressChangedEventArgs>
																progressChangedCallBack)
		{
			using (var agent = new DefaultWebClient())
			{
				string tempFile = Guid.NewGuid().ToString();
				agent.DownloadFile(metadata.redirect_to, tempFile, true, progressChangedCallBack);
				contentType = agent.ResponseHeaders["Content-type"];
				File.Move(tempFile, file);
			}
		}

		public static DownloadResult DownloadImageWithMetadata(string objectId, string session_token, string apikey,
		                                                       ImageMeta meta, string station_id)
		{
			return DownloadImageWithMetadata(objectId, session_token, apikey, meta, station_id, null);
		}


		public static DownloadResult DownloadImageWithMetadata(string objectId, string session_token, string apikey,
															   ImageMeta meta, string station_id,
															   Action<object, ProgressChangedEventArgs>
																progressChangedCallBack)
		{
			var metadata = GetImageMetadata(objectId, session_token, apikey, meta, station_id);

			logger.Info("Attachement redirect to: " + metadata.redirect_to);
			using (var agent = new DefaultWebClient())
			{
				using (var to = new MemoryStream())
				using (var from = agent.OpenRead(metadata.redirect_to))
				{
					from.WriteTo(to, 1024, progressChangedCallBack);
					return new DownloadResult(to.ToArray(), metadata, agent.ResponseHeaders["Content-type"]);
				}
			}
		}

		public static void SetSync(string object_id, string session_token)
		{
			if (object_id == null || session_token == null)
				throw new ArgumentNullException();

			var parameters = new Dictionary<object, object>
			{
				{CloudServer.PARAM_OBJECT_IDS, "[\"" + object_id + "\"]"},
				{CloudServer.PARAM_SESSION_TOKEN, session_token},
				{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			};

			CloudServer.requestPath<CloudResponse>("attachments/set_sync", parameters);
			logger.Info("attachments/set_sync: " + object_id);
		}

		public static void SetSync(ICollection<string> object_ids, string session_token)
		{
			if (session_token == null || object_ids == null || object_ids.Count == 0)
				throw new ArgumentNullException();

			var buffer = new StringBuilder();
			buffer.Append("[");
			foreach (var object_id in object_ids)
				buffer.Append("\"").Append(object_id).Append("\",");

			buffer.Remove(buffer.Length - 1, 1); // remove the trailing comma
			buffer.Append("]");
			string objIdArray = buffer.ToString();

			var parameters = new Dictionary<object, object>
			{
				{CloudServer.PARAM_OBJECT_IDS, objIdArray},
				{CloudServer.PARAM_SESSION_TOKEN, session_token},
				{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			};

			CloudServer.requestPath<CloudResponse>("attachments/set_sync", parameters);
			logger.Info("attachments/set_sync: " + objIdArray);
		}

		public static AttachmentQueueResponse GetQueue(string session, int count)
		{
			using (DefaultWebClient agent = new DefaultWebClient())
			{
				Dictionary<object, object> parameters = new Dictionary<object, object>
				{
					{ CloudServer.PARAM_API_KEY, CloudServer.APIKey},
					{ CloudServer.PARAM_SESSION_TOKEN, session},
					{ CloudServer.PARAM_TARGET, TMPQUEUE},
					{ CloudServer.PARAM_COUNT, count}
				};
				
				return CloudServer.requestPath<AttachmentQueueResponse>("attachments/get_queue", parameters, false);
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
	}

	public class DownloadResult
	{
		public DownloadResult(byte[] Image, AttachmentView metadata, string contentType)
		{
			this.Image = Image;
			Metadata = metadata;
			ContentType = contentType ?? "application/octet-stream";
		}

		public byte[] Image { get; private set; }
		public AttachmentView Metadata { get; private set; }
		public string ContentType { get; private set; }
	}
	public class AttachmentQueueResponse
	{
		public int total_results { get; set; }
		public int counts { get; set; }
		public List<string> objects { get; set; }
	}
}
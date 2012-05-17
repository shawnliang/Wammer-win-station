using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using Wammer.Model;
using Wammer.Station;
using Wammer.Station.JSONClass;
using Wammer.Utility;
using System.Text;

namespace Wammer.Cloud
{
	public class AttachmentApi
	{
		#region Location enum

		public enum Location
		{
			Cloud = 0,
			Station = 1,
			Dropbox = 2
		}

		#endregion

		public AttachmentApi(Driver driver)
		{
			userToken = driver.session_token;
		}

		private string userToken { get; set; }

		public void AttachmentSetLoc(WebClient agent, int loc, string object_id, string file_path)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{"loc", loc},
			                 		{"object_id", object_id},
			                 		{"file_path", file_path},
			                 		{CloudServer.PARAM_SESSION_TOKEN, userToken},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			CloudServer.requestPath<CloudResponse>(agent, "attachments/setloc", parameters);
		}

		public void AttachmentUnsetLoc(WebClient agent, int loc, string object_id)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{"loc", loc},
			                 		{"object_id", object_id},
			                 		{CloudServer.PARAM_SESSION_TOKEN, userToken},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.PARAM_API_KEY}
			                 	};

			CloudServer.requestPath<CloudResponse>(agent, "attachments/unsetloc", parameters);
		}

		public AttachmentGetResponse AttachmentGet(WebClient agent, string object_id)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_OBJECT_ID, object_id},
			                 		{CloudServer.PARAM_SESSION_TOKEN, userToken},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			return CloudServer.requestPath<AttachmentGetResponse>(agent, "attachments/get", parameters);
		}

		public void AttachmentView(WebClient agent, ResourceDownloadEventArgs evtargs, string stationId)
		{
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

			CloudServer.requestDownload(agent, "attachments/view", parameters, evtargs.filepath);
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
			using (WebClient agent = new NoRedirectWebClient())
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

				var metadata =
					CloudServer.requestPath<AttachmentView>(agent, "attachments/view", parameters);

				using (var redirectableAgent = new WebClient())
				{
					using (var to = new MemoryStream())
					using (var from = redirectableAgent.OpenRead(metadata.redirect_to))
					{
						from.WriteTo(to, 1024, progressChangedCallBack);
						return new DownloadResult(to.ToArray(), metadata, redirectableAgent.ResponseHeaders["Content-type"]);
					}
				}
			}
		}

		public static void SetSync(WebClient agent, string object_id, string session_token)
		{
			if (agent == null || object_id == null || session_token == null)
				throw new ArgumentNullException();

			var parameters = new Dictionary<object, object>
			{
				{CloudServer.PARAM_OBJECT_IDS, "[\"" + object_id + "\"]"},
				{CloudServer.PARAM_SESSION_TOKEN, session_token},
				{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			};

			CloudServer.requestPath<CloudResponse>(agent, "attachments/set_sync", parameters);
		}

		public static void SetSync(WebClient agent, ICollection<string> object_ids, string session_token)
		{
			if (agent == null || session_token == null || object_ids == null || object_ids.Count == 0)
				throw new ArgumentNullException();

			StringBuilder buffer = new StringBuilder();
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

			CloudServer.requestPath<CloudResponse>(agent, "attachments/set_sync", parameters);
		}

		public static AttachmentInfo GetInfo(WebClient agent, string object_id, string session_token)
		{
			if (agent == null || object_id == null || session_token == null)
				throw new ArgumentNullException();

			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_OBJECT_ID, object_id},
			                 		{CloudServer.PARAM_SESSION_TOKEN, session_token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			return CloudServer.requestPath<AttachmentInfo>(agent, "attachments/get", parameters, false);
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
}
using System.Collections.Generic;
using System.Net;
using System.ComponentModel;
using MongoDB.Driver.Builders;
using Wammer.Model;
using Wammer.Station;
using Wammer.Utility;
using System;
using System.IO;


namespace Wammer.Cloud
{
	public class AttachmentApi
	{
		private string userToken { get; set; }

		public enum Location
		{
			Cloud = 0,
			Station = 1,
			Dropbox = 2
		}

		public AttachmentApi(Driver driver)
		{
			this.userToken = driver.session_token;
		}

		public void AttachmentSetLoc(WebClient agent, int loc, string object_id, string file_path)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ "loc", loc },
				{ "object_id", object_id },
				{ "file_path", file_path },
				{ CloudServer.PARAM_SESSION_TOKEN, this.userToken},
				{ CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			};

			CloudServer.requestPath<CloudResponse>(agent, "attachments/setloc", parameters);
		}

		public void AttachmentUnsetLoc(WebClient agent, int loc, string object_id)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ "loc", loc },
				{ "object_id", object_id },
				{ CloudServer.PARAM_SESSION_TOKEN, this.userToken },
				{ CloudServer.PARAM_API_KEY, CloudServer.PARAM_API_KEY }
			};

			CloudServer.requestPath<CloudResponse>(agent, "attachments/unsetloc", parameters);
		}

		public AttachmentGetResponse AttachmentGet(WebClient agent, string object_id)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
		    {
		        {CloudServer.PARAM_OBJECT_ID, object_id},
		        {CloudServer.PARAM_SESSION_TOKEN, this.userToken},
		        {CloudServer.PARAM_API_KEY, CloudServer.APIKey}
		    };

			return CloudServer.requestPath<AttachmentGetResponse>(agent, "attachments/get", parameters);
		}

		public void AttachmentView(WebClient agent, ResourceDownloadEventArgs evtargs, string stationId)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object> 
			{ 
				{CloudServer.PARAM_OBJECT_ID, evtargs.attachment.object_id},
				{CloudServer.PARAM_SESSION_TOKEN, this.userToken},
				{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
				{CloudServer.PARAM_STATION_ID, stationId}
			};

			if (evtargs.imagemeta != ImageMeta.Origin)
			{
				parameters.Add(CloudServer.PARAM_IMAGE_META, evtargs.imagemeta.ToString().ToLower());
			}

			CloudServer.requestDownload(agent, "attachments/view", parameters, evtargs.filepath);
		}

		public static DownloadResult DownloadImageWithMetadata(string objectId, string session_token, string apikey, ImageMeta meta, string station_id)
		{
			return DownloadImageWithMetadata(objectId, session_token, apikey, meta, station_id, null);
		}

		public static DownloadResult DownloadImageWithMetadata(string objectId, string session_token, string apikey, ImageMeta meta, string station_id,
			Action<object, System.ComponentModel.ProgressChangedEventArgs> progressChangedCallBack)
		{
			using (WebClient agent = new NoRedirectWebClient())
			{
				Dictionary<object, object> parameters = new Dictionary<object, object>
				{
					{ "object_id", objectId },
					{ "session_token", session_token },
					{ "station_id", station_id },
					{ "apikey", apikey },
					{ "return_meta", "true" }
				};

				if (meta != ImageMeta.Origin && meta != ImageMeta.None)
					parameters.Add("image_meta", meta.ToString().ToLower());

				Wammer.Station.JSONClass.AttachmentView metadata =
					CloudServer.requestPath<Wammer.Station.JSONClass.AttachmentView>(agent, "attachments/view", parameters);

				using (MemoryStream to = new MemoryStream())
				using (Stream from = agent.OpenRead(metadata.redirect_to))
				{
					from.WriteTo(to, 1024, progressChangedCallBack);
					return new DownloadResult(to.ToArray(), metadata, agent.ResponseHeaders["Content-type"]);
				}
			}
		}

		public static void SetSync(WebClient agent, string object_id, string session_token)
		{
			if (agent == null || object_id == null || session_token == null)
				throw new ArgumentNullException();

			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ CloudServer.PARAM_OBJECT_IDS, "[\"" + object_id + "\"]" },
				{ CloudServer.PARAM_SESSION_TOKEN, session_token},
				{ CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			};

			CloudServer.requestPath<CloudResponse>(agent, "attachments/set_sync", parameters);
		}

		public static AttachmentInfo GetInfo(WebClient agent, string object_id, string session_token)
		{
			if (agent == null || object_id == null || session_token == null)
				throw new ArgumentNullException();

			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ CloudServer.PARAM_OBJECT_ID, object_id },
				{ CloudServer.PARAM_SESSION_TOKEN, session_token},
				{ CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			};

			return CloudServer.requestPath<AttachmentInfo>(agent, "attachments/get", parameters, true);
		}
	}

	public class DownloadResult
	{
		public byte[] Image { get; private set; }
		public Wammer.Station.JSONClass.AttachmentView Metadata { get; private set; }
		public string ContentType { get; private set; }

		public DownloadResult(byte[] Image, Wammer.Station.JSONClass.AttachmentView metadata, string contentType)
		{
			this.Image = Image;
			this.Metadata = metadata;
			this.ContentType = contentType;

			if (this.ContentType == null)
				this.ContentType = "application/octet-stream";
		}
	}
}

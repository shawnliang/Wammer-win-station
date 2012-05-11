using System;
using System.IO;
using System.Net;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;
using System.ComponentModel;
using log4net;

namespace Wammer.Station
{
	public class AttachmentViewHandler : HttpHandler
	{
		private readonly string station_id;

		/// <summary>
		/// File download is started.
		/// </summary>
		public event EventHandler FileDownloadStarted;

		/// <summary>
		/// File download is in progress.
		/// </summary>
		public event ProgressChangedEventHandler FileDownloadInProgress;

		/// <summary>
		/// File download is finished. Result could be either successful or unsuccessful.
		/// </summary>
		public event EventHandler FileDownloadFinished;

		public AttachmentViewHandler(string stationId)
			: base()
		{
			this.station_id = stationId;
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}

		public override void HandleRequest()
		{
			ImageMeta imageMeta = ImageMeta.None;

			try
			{
				string objectId = Parameters["object_id"];
				if (objectId == null)
					throw new ArgumentException("missing required param: object_id");


				if (Parameters["image_meta"] == null)
					imageMeta = ImageMeta.Origin;
				else
					imageMeta = (ImageMeta)Enum.Parse(typeof(ImageMeta),
																	Parameters["image_meta"], true);

				// "target" parameter is used to request cover image or slide page.
				// In this version station has no such resources so station always forward this
				// request to cloud.
				if (Parameters["target"] != null)
				{
					TunnelToCloud(station_id, imageMeta);
					return;
				}

				string namePart = objectId;
				string metaStr = imageMeta.GetCustomAttribute<DescriptionAttribute>().Description;

				if (imageMeta != ImageMeta.Origin)
				{
					namePart += "_" + metaStr;
				}

				Attachment doc = null;

				doc = AttachmentCollection.Instance.FindOne(imageMeta == ImageMeta.Origin ? Query.And(Query.EQ("_id", objectId), Query.Exists("saved_file_name", true)) : Query.And(Query.EQ("_id", objectId), Query.Exists("image_meta." + metaStr, true)));

				if (doc == null)
				{
					TunnelToCloud(station_id, imageMeta);
					return;
				}

				Driver driver = DriverCollection.Instance.FindDriverByGroupId(doc.group_id);
				if (driver == null)
					throw new WammerStationException("Cannot find user with group_id: " + doc.group_id, (int)StationLocalApiError.InvalidDriver);

				var storage = new FileStorage(driver);
				FileStream fs = storage.LoadByNameWithNoSuffix(namePart);
				Response.StatusCode = 200;
				Response.ContentLength64 = fs.Length;
				Response.ContentType = doc.mime_type;

				if (doc.type == AttachmentType.image && imageMeta != ImageMeta.Origin)
					Response.ContentType = doc.image_meta.GetThumbnailInfo(imageMeta).mime_type;

				Wammer.Utility.StreamHelper.BeginCopy(fs, Response.OutputStream, CopyComplete,
					new CopyState(fs, Response, objectId));

			}
			catch (ArgumentException e)
			{
				this.LogWarnMsg("Bad request: " + e.Message);
				HttpHelper.RespondFailure(Response, e, (int)HttpStatusCode.BadRequest);
			}
			catch (FileNotFoundException)
			{
				TunnelToCloud(station_id, imageMeta);
			}
		}


		protected void TunnelToCloud(string station_id, ImageMeta meta)
		{
			if (string.IsNullOrEmpty(station_id))
				throw new ArgumentException("param cannot be null or empty. If you really need it blank, change the code.");

			this.LogDebugMsg("Forward to cloud");

			try
			{
				OnFileDownloadStarted();

				DownloadResult downloadResult = AttachmentApi.DownloadImageWithMetadata(
					Parameters["object_id"], Parameters["session_token"], Parameters["apikey"], meta, station_id, (sender, e) =>
					{
						OnFileDownloadInProgress(e);
					});

				Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", downloadResult.Metadata.creator_id));

				if (driver == null)
					throw new WammerStationException("driver does not exist: " + downloadResult.Metadata.creator_id, (int)StationLocalApiError.InvalidDriver);

				if (meta == ImageMeta.Origin && !driver.isPrimaryStation)
					throw new WammerStationException("Access to original attachment from secondary station is not allowed.", (int)StationLocalApiError.AccessDenied);

				var storage = new FileStorage(driver);
				var fileName = GetSavedFile(Parameters["object_id"], downloadResult.Metadata.redirect_to, meta);
				storage.SaveFile(fileName, new ArraySegment<byte>(downloadResult.Image));

				SetAttachementToDB(meta, downloadResult, fileName);

				Response.ContentType = downloadResult.ContentType;

				var m = new MemoryStream(downloadResult.Image);

				Wammer.Utility.StreamHelper.BeginCopy(m, Response.OutputStream, CopyComplete,
					new CopyState(m, Response, Parameters["object_id"]));

			}
			catch (WebException e)
			{
				throw new WammerCloudException("AttachmentViewHandler cannot download object: " + Parameters["object_id"] + " image meta: " + meta, e);
			}
			catch (WammerCloudException e)
			{
				throw new WammerCloudException("AttachmentViewHandler cannot download object: " + Parameters["object_id"] + " image meta: " + meta, e);
			}
			finally
			{
				OnFileDownloadFinished();
			}
		}

		private void SetAttachementToDB(ImageMeta meta, DownloadResult downloadResult, string fileName)
		{
			if (meta == ImageMeta.Origin)
			{
				AttachmentCollection.Instance.Update(Query.EQ("_id", Parameters["object_id"]), Update
					.Set("file_name", downloadResult.Metadata.file_name)
					.Set("mime_type", downloadResult.ContentType)
					.Set("url", "/v2/attachments/view/?object_id=" + Parameters["object_id"])
					.Set("file_size", downloadResult.Image.Length)
					.Set("modify_time", DateTime.UtcNow)
					.Set("image_meta.width", downloadResult.Metadata.image_meta.width)
					.Set("image_meta.height", downloadResult.Metadata.image_meta.height)
					.Set("md5", downloadResult.Metadata.md5)
					.Set("type", (int)(AttachmentType)Enum.Parse(typeof(AttachmentType), downloadResult.Metadata.type, true))
					.Set("group_id", downloadResult.Metadata.group_id)
					.Set("saved_file_name", fileName)
					.Set("is_body_upstreamed", true), UpdateFlags.Upsert);
			}
			else
			{
				var metaStr = meta.GetCustomAttribute<DescriptionAttribute>().Description;
				AttachmentCollection.Instance.Update(Query.EQ("_id", Parameters["object_id"]), Update
					.Set("group_id", downloadResult.Metadata.group_id)
					.Set("file_name", downloadResult.Metadata.file_name)
					.Set("is_thumb_upstreamed", true)
					.Set("type", (int)(AttachmentType)Enum.Parse(typeof(AttachmentType), downloadResult.Metadata.type, true))
					.Set("image_meta." + metaStr, new ThumbnailInfo()
					{
						mime_type = downloadResult.ContentType,
						modify_time = DateTime.UtcNow,
						url = "/v2/attachments/view/?object_id=" + Parameters["object_id"] + "&image_meta=" + metaStr,
						file_size = downloadResult.Image.Length,
						width = downloadResult.Metadata.image_meta.GetThumbnail(meta).width,
						height = downloadResult.Metadata.image_meta.GetThumbnail(meta).height,
						saved_file_name = fileName
					}.ToBsonDocument()), UpdateFlags.Upsert);
			}
		}


		private static string GetSavedFile(string objectID, string uri, ImageMeta meta)
		{
			string fileName = objectID;			

			if (meta != ImageMeta.Origin && meta != ImageMeta.None)
			{
				var metaStr = meta.GetCustomAttribute<DescriptionAttribute>().Description;
				fileName += "_" + metaStr;
			}

			if (uri.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
				uri = new Uri(uri).AbsolutePath;

			string extension = Path.GetExtension(uri);

			if (meta == ImageMeta.Small || meta == ImageMeta.Medium || meta == ImageMeta.Large || meta == ImageMeta.Square)
				fileName += ".dat";
			else if (!string.IsNullOrEmpty(extension))
				fileName += extension;

			return fileName;
		}



		private void CopyComplete(IAsyncResult ar)
		{
			var state = (CopyState)ar.AsyncState;

			try
			{
				Wammer.Utility.StreamHelper.EndCopy(ar);
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Error responding attachment/view : " + state.attachmentId, e);
				HttpHelper.RespondFailure(state.response, new CloudResponse(400, -1, e.Message));
			}
			finally
			{
				try
				{
					state.source.Close();
					state.response.Close();
				}
				catch (Exception e)
				{
					this.LogWarnMsg("error closing source and response", e);
				}
			}
		}

		private void OnFileDownloadStarted()
		{
			EventHandler handler = FileDownloadStarted;

			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		private void OnFileDownloadFinished()
		{
			EventHandler handler = FileDownloadFinished;

			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		private void OnFileDownloadInProgress(ProgressChangedEventArgs evt)
		{
			ProgressChangedEventHandler handler = FileDownloadInProgress;

			if (handler != null)
				handler(this, evt);
		}
	}

	class CopyState
	{
		public Stream source { get; private set; }
		public HttpListenerResponse response { get; private set; }
		public string attachmentId { get; private set; }

		public CopyState(Stream src, HttpListenerResponse res, string attachmentId)
		{
			source = src;
			response = res;
			this.attachmentId = attachmentId;
		}
	}
}

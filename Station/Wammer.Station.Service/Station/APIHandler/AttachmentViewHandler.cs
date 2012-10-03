using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI, "/attachments/view/")]
	public class AttachmentViewHandler : HttpHandler
	{
		private readonly string station_id;
		private volatile int allowForwardToCloud = 1;
		private readonly AttachmentView.AttachmentViewHandlerImp impl = new AttachmentView.AttachmentViewHandlerImp();
		private AttachmentUpload.AttachmentUploadStorage storage = new AttachmentUpload.AttachmentUploadStorage(new AttachmentUpload.AttachmentUploadStorageDB());


		public AttachmentViewHandler(string stationId)
		{
			station_id = stationId;
		}

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

		public override object Clone()
		{
			return MemberwiseClone();
		}

		public override void HandleRequest()
		{
			try
			{
				var result = impl.GetAttachmentStream(Parameters);


				Response.StatusCode = 200;
				Response.ContentLength64 = result.Stream.Length;
				Response.ContentType = result.MimeType;

				StreamHelper.BeginCopy(result.Stream, Response.OutputStream, CopyComplete,
									   new CopyState(result.Stream, Response, Parameters["object_id"]));
			}
			catch(FileNotFoundException e)
			{
				this.LogDebugMsg("Attachment is not found; Bypass to cloud: " + e.Message);
				
				var meta = Parameters["image_meta"];

				if (string.IsNullOrEmpty(meta))
					TunnelToCloud(station_id, ImageMeta.Origin);
				else
					TunnelToCloud(station_id, (ImageMeta)Enum.Parse(typeof(ImageMeta), meta, true));
			}
		}

		public void OnSyncResumed(object sender, EventArgs args)
		{
			System.Threading.Interlocked.Exchange(ref allowForwardToCloud, 1);
		}

		public void OnSyncSuspended(object sender, EventArgs args)
		{
			System.Threading.Interlocked.Exchange(ref allowForwardToCloud, 0);
		}

		protected void TunnelToCloud(string station_id, ImageMeta meta)
		{
			if (string.IsNullOrEmpty(station_id))
				throw new ArgumentException("param cannot be null or empty. If you really need it blank, change the code.");

			if (0 == allowForwardToCloud)
				throw new WammerStationException("Object not found and forward to cloud is not allowed", (int)StationLocalApiError.NotFound);

			this.LogDebugMsg("Forward to cloud");

			try
			{
				OnFileDownloadStarted();

				var metaData = AttachmentApi.GetImageMetadata(
					Parameters["object_id"], Parameters["session_token"], Parameters["apikey"], meta, station_id);

				var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", metaData.creator_id));

				if (driver == null)
					throw new WammerStationException("driver does not exist: " + metaData.creator_id,
					                                 (int) StationLocalApiError.InvalidDriver);

				if (meta == ImageMeta.Origin && !driver.isPrimaryStation)
					throw new WammerStationException("Access to original attachment from secondary station is not allowed.",
					                                 (int) StationLocalApiError.AccessDenied);


				var downloadResult = AttachmentApi.DownloadObject(metaData.redirect_to, metaData);

				var saveResult = SaveAttachmentToDisk(meta, metaData, downloadResult);

				this.LogDebugMsg("Save attachement file to " + saveResult.RelativePath);

				SetAttachementToDB(meta, downloadResult, saveResult.RelativePath);

				if (meta == ImageMeta.Origin || meta == ImageMeta.None)
					TaskQueue.Enqueue(new NotifyCloudOfBodySyncedTask(Parameters["object_id"], driver.session_token), TaskPriority.Low, true);

				Response.ContentType = downloadResult.ContentType;

				var m = new MemoryStream(downloadResult.Image);

				StreamHelper.BeginCopy(m, Response.OutputStream, CopyComplete,
				                       new CopyState(m, Response, Parameters["object_id"]));
			}
			catch (WebException e)
			{
				throw new WammerCloudException(
					"AttachmentViewHandler cannot download object: " + Parameters["object_id"] + " image meta: " + meta, e);
			}
			catch (WammerCloudException e)
			{
				throw new WammerCloudException(
					"AttachmentViewHandler cannot download object: " + Parameters["object_id"] + " image meta: " + meta, e.response, e.InnerException);
			}
			finally
			{
				OnFileDownloadFinished();
			}
		}

		private AttachmentUpload.AttachmentSaveResult SaveAttachmentToDisk(ImageMeta meta, JSONClass.AttachmentView metaData, DownloadResult result)
		{
			var param = new AttachmentUpload.UploadData
			{
				object_id = Parameters["object_id"],
				group_id = metaData.group_id,
				file_name = metaData.file_name,
				imageMeta = meta,
				raw_data = new ArraySegment<byte>(result.Image),
				file_create_time = string.IsNullOrEmpty(metaData.file_create_time) ? (DateTime?)null : TimeHelper.ParseCloudTimeString(metaData.file_create_time)
			};

			AttachmentUpload.AttachmentSaveResult saveResult;
			if (meta == ImageMeta.Origin)
			{
				string takenTime = extractTakenTimeFromImageExif(result);
				saveResult = storage.Save(param, takenTime);
			}
			else
			{
				saveResult = storage.Save(param, null);
			}
			return saveResult;
		}

		private static string extractTakenTimeFromImageExif(DownloadResult result)
		{
			try
			{
				var exif = ExifLibrary.ExifFile.Read(result.Image);
				string takenTime = null;
				if (exif.Properties.ContainsKey(ExifLibrary.ExifTag.DateTimeOriginal))
					takenTime = ((DateTime)(exif.Properties[ExifLibrary.ExifTag.DateTimeOriginal].Value)).ToCloudTimeString();
				return takenTime;
			}
			catch
			{
				return null;
			}
		}

		private void SetAttachementToDB(ImageMeta meta, DownloadResult downloadResult, string fileName)
		{
			this.LogDebugMsg("Save attachement data to db");
			if (meta == ImageMeta.Origin)
			{
				AttachmentCollection.Instance.Update(Query.EQ("_id", Parameters["object_id"]), Update
				                                                                               	.Set("file_name",
				                                                                               	     downloadResult.Metadata.
				                                                                               	     	file_name)
				                                                                               	.Set("mime_type",
				                                                                               	     downloadResult.ContentType)
				                                                                               	.Set("url",
				                                                                               	     "/v2/attachments/view/?object_id=" +
				                                                                               	     Parameters["object_id"])
				                                                                               	.Set("file_size",
				                                                                               	     downloadResult.Image.Length)
				                                                                               	.Set("modify_time", DateTime.UtcNow)
				                                                                               	.Set("image_meta.width",
				                                                                               	     downloadResult.Metadata.
				                                                                               	     	image_meta.width)
				                                                                               	.Set("image_meta.height",
				                                                                               	     downloadResult.Metadata.
				                                                                               	     	image_meta.height)
				                                                                               	.Set("md5",
				                                                                               	     downloadResult.Metadata.md5)
				                                                                               	.Set("type",
				                                                                               	     (int)
				                                                                               	     (AttachmentType)
				                                                                               	     Enum.Parse(
				                                                                               	     	typeof (AttachmentType),
				                                                                               	     	downloadResult.Metadata.type,
				                                                                               	     	true))
				                                                                               	.Set("group_id",
				                                                                               	     downloadResult.Metadata.
				                                                                               	     	group_id)
				                                                                               	.Set("saved_file_name", fileName),
				                                     UpdateFlags.Upsert);
			}
			else
			{
				string metaStr = meta.GetCustomAttribute<DescriptionAttribute>().Description;
				AttachmentCollection.Instance.Update(Query.EQ("_id", Parameters["object_id"]), Update
				                                                                               	.Set("group_id",
				                                                                               	     downloadResult.Metadata.
				                                                                               	     	group_id)
				                                                                               	.Set("file_name",
				                                                                               	     downloadResult.Metadata.
				                                                                               	     	file_name)
				                                                                               	.Set("type",
				                                                                               	     (int)
				                                                                               	     (AttachmentType)
				                                                                               	     Enum.Parse(
				                                                                               	     	typeof (AttachmentType),
				                                                                               	     	downloadResult.Metadata.type,
				                                                                               	     	true))
				                                                                               	.Set("image_meta." + metaStr,
				                                                                               	     new ThumbnailInfo
				                                                                               	     	{
				                                                                               	     		mime_type =
				                                                                               	     			downloadResult.ContentType,
				                                                                               	     		modify_time = DateTime.UtcNow,
				                                                                               	     		url =
				                                                                               	     			"/v2/attachments/view/?object_id=" +
				                                                                               	     			Parameters["object_id"] +
				                                                                               	     			"&image_meta=" + metaStr,
				                                                                               	     		file_size =
				                                                                               	     			downloadResult.Image.Length,
				                                                                               	     		width =
				                                                                               	     			downloadResult.Metadata.
				                                                                               	     			image_meta.GetThumbnail(meta)
				                                                                               	     			.width,
				                                                                               	     		height =
				                                                                               	     			downloadResult.Metadata.
				                                                                               	     			image_meta.GetThumbnail(meta)
				                                                                               	     			.height,
				                                                                               	     		saved_file_name = fileName
				                                                                               	     	}.ToBsonDocument()),
				                                     UpdateFlags.Upsert);
			}
		}


		private static string GetSavedFile(string objectID, string uri, ImageMeta meta)
		{
			var fileName = objectID;

			if (meta != ImageMeta.Origin && meta != ImageMeta.None)
			{
				var metaStr = meta.GetCustomAttribute<DescriptionAttribute>().Description;
				fileName += "_" + metaStr;
			}

			if (uri.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
				uri = new Uri(uri).AbsolutePath;

			var extension = Path.GetExtension(uri);

			if (meta == ImageMeta.Small || meta == ImageMeta.Medium || meta == ImageMeta.Large || meta == ImageMeta.Square)
				fileName += ".dat";
			else if (!string.IsNullOrEmpty(extension))
				fileName += extension;

			return fileName;
		}


		private void CopyComplete(IAsyncResult ar)
		{
			var state = (CopyState) ar.AsyncState;

			try
			{
				StreamHelper.EndCopy(ar);
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

	internal class CopyState
	{
		public CopyState(Stream src, HttpListenerResponse res, string attachmentId)
		{
			source = src;
			response = res;
			this.attachmentId = attachmentId;
		}

		public Stream source { get; private set; }
		public HttpListenerResponse response { get; private set; }
		public string attachmentId { get; private set; }
	}
}
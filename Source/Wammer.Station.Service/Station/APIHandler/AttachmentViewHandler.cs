﻿using MongoDB.Driver.Builders;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station.Timeline;
using Wammer.Utility;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI, "/attachments/view/")]
	public class AttachmentViewHandler : HttpHandler
	{
		private readonly string station_id;
		private volatile int allowForwardToCloud = 1;
		private readonly AttachmentView.AttachmentViewHandlerImp impl = new AttachmentView.AttachmentViewHandlerImp();

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
			catch (FileNotFoundException e)
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
													 (int)StationLocalApiError.InvalidDriver);

				if (meta == ImageMeta.Origin && !driver.isPrimaryStation)
					throw new WammerStationException("Access to original attachment from secondary station is not allowed.",
													 (int)StationLocalApiError.AccessDenied);


				var downloadResult = AttachmentApi.DownloadObject(metaData.redirect_to, metaData);

				var saveResult = ResourceDownloadTask.SaveAttachmentToDisk(meta, metaData, downloadResult.Image);

				this.LogDebugMsg("Attachement is saved to " + saveResult.RelativePath);

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

		private void SetAttachementToDB(ImageMeta meta, DownloadResult downloadResult, string fileName)
		{

			var attachmentAttributes = downloadResult.Metadata;
			var mimeType = downloadResult.ContentType;
			var length = downloadResult.Image.Length;

			ResourceDownloadTask.SaveToAttachmentDB(meta, fileName, attachmentAttributes, mimeType, length);
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
			var state = (CopyState)ar.AsyncState;

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
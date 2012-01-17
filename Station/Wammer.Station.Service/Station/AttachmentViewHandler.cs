using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Web;

using Wammer.Cloud;
using Wammer.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	public class AttachmentViewHandler: HttpHandler
	{
		private static log4net.ILog logger = log4net.LogManager.GetLogger("AttachView");
		private string AdditionalParam;

		public AttachmentViewHandler(string stationId)
			:base()
		{
			this.AdditionalParam = "station_id=" + stationId;
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}

		protected override void HandleRequest()
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
					throw new FileNotFoundException();

				string namePart = objectId;
				string metaStr = "";
				if (imageMeta != ImageMeta.Origin)
				{
					metaStr = imageMeta.ToString().ToLower();
					namePart += "_" + metaStr;
				}
				
				Attachment doc = AttachmentCollection.Instance.FindOne(Query.EQ("_id", objectId));
				if (doc == null)
					throw new FileNotFoundException();

				Driver driver = DriverCollection.Instance.FindOne(Query.ElemMatch("groups", Query.EQ("group_id", doc.group_id)));
				FileStorage storage = new FileStorage(driver);
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
				logger.Warn("Bad request: " + e.Message);
				HttpHelper.RespondFailure(Response, e, (int)HttpStatusCode.BadRequest);
			}
			catch (FileNotFoundException)
			{
				TunnelToCloud(AdditionalParam);
			}
		}

		private static void CopyComplete(IAsyncResult ar)
		{
			CopyState state = (CopyState)ar.AsyncState;

			try
			{
				Wammer.Utility.StreamHelper.EndCopy(ar);	
			}
			catch (Exception e)
			{
				logger.Warn("Error responding attachment/view : " + state.attachmentId, e);
				HttpHelper.RespondFailure(state.response, new CloudResponse(400, -1, e.Message));
			}
			finally
			{
				try{
					state.source.Close();
					state.response.Close();
				}
				catch(Exception e)
				{
					logger.Warn("error closing source and response", e);
				}
			}
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

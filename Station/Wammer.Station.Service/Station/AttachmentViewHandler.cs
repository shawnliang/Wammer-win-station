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

		public AttachmentViewHandler()
			:base()
		{
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
				using (FileStream fs = storage.LoadByNameWithNoSuffix(namePart))
				{
					Response.StatusCode = 200;
					Response.ContentLength64 = fs.Length;
					Response.ContentType = doc.mime_type;

					if (doc.type == AttachmentType.image && imageMeta != ImageMeta.Origin)
						Response.ContentType = doc.image_meta.GetThumbnailInfo(imageMeta).mime_type;

					Wammer.Utility.StreamHelper.Copy(fs, Response.OutputStream);
					fs.Close();
					Response.OutputStream.Close();
				}
			}
			catch (ArgumentException e)
			{
				logger.Warn("Bad request: " + e.Message);
				HttpHelper.RespondFailure(Response, e, (int)HttpStatusCode.BadRequest);
			}
			catch (FileNotFoundException)
			{
				if (imageMeta == ImageMeta.Large ||
					imageMeta == ImageMeta.Medium ||
					imageMeta == ImageMeta.Small ||
					imageMeta == ImageMeta.Square ||
					Parameters["target"] != null)
				{
					TunnelToCloud();
				}
				else
				{
					logger.Warn("No such resource: " + Parameters["object_id"]);
					HttpHelper.RespondFailure(Response,
						new CloudResponse((int)HttpStatusCode.NotFound, -1, "No such resource"));
				}
			}
		}
	}
}

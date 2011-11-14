using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Web;

using Wammer.Cloud;
using MongoDB.Driver;
//using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	public class AttachmentViewHandler: HttpHandler
	{
		private readonly MongoServer mongodb;
		private readonly MongoCollection<Attachment> attachments;
		private readonly AtomicDictionary<string, FileStorage> groupFolders;

		public AttachmentViewHandler(MongoServer mongodb, 
			AtomicDictionary<string, FileStorage> groupFolders)
			:base()
		{
			this.groupFolders = groupFolders;
			this.mongodb = mongodb;
			this.attachments = mongodb.GetDatabase("wammer").
														GetCollection<Attachment>("attachments");
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}

		protected override void HandleRequest()
		{
			try
			{
				string objectId = Parameters["object_id"];
				if (objectId == null)
					throw new ArgumentException("missing required param: object_id");

				ImageMeta imageMeta;

				if (Parameters["image_meta"] == null)
					imageMeta = ImageMeta.Origin;
				else
					imageMeta = (ImageMeta)Enum.Parse(typeof(ImageMeta),
																	Parameters["image_meta"], true);

				// Assuming client only request (cover) image for doc attachments and
				// station always don't have (cover) image
				if (Parameters["target"] != null && Parameters["target"].Equals("image"))
					throw new FileNotFoundException();

				string namePart = objectId;
				string metaStr = "";
				if (imageMeta != ImageMeta.Origin)
				{
					metaStr = imageMeta.ToString().ToLower();
					namePart += "_" + metaStr;
				}
				
				Attachment doc = attachments.FindOne(Query.EQ("_id", objectId));
				if (doc == null)
					throw new FileNotFoundException();

				using (FileStream fs = groupFolders[doc.group_id].LoadByNameWithNoSuffix(namePart))
				{
					Response.StatusCode = 200;

					if (doc.type == AttachmentType.image)
					{
						if (imageMeta == ImageMeta.Origin)
							Response.ContentType = doc.mime_type;
						else
							Response.ContentType = doc.image_meta.GetThumbnailInfo(imageMeta).mime_type;
					}

					Wammer.Utility.StreamHelper.Copy(fs, Response.OutputStream);
					fs.Close();
					Response.OutputStream.Close();
				}
			}
			catch (ArgumentException e)
			{
				HttpHelper.RespondFailure(Response, e, (int)HttpStatusCode.BadRequest);
			}
			catch (FileNotFoundException e)
			{
				TunnelToCloud();
			}
		}
	}
}

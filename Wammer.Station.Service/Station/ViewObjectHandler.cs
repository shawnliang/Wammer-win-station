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
	public class ViewObjectHandler: HttpHandler
	{
		private readonly MongoServer mongodb;
		private readonly MongoCollection<Attachment> attachments;
		private readonly AtomicDictionary<string, FileStorage> groupFolders;

		public ViewObjectHandler(MongoServer mongodb, 
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


				string namePart = objectId;
				string metaStr = "";
				if (imageMeta != ImageMeta.Origin)
				{
					metaStr = imageMeta.ToString().ToLower();
					namePart += "_" + metaStr;
				}
				
				Attachment doc = attachments.FindOne(Query.EQ("_id", objectId));
				if (doc == null)
					throw new FileNotFoundException("attachment " + objectId + " is not found");

				using (FileStream fs = groupFolders[doc.group_id].LoadByNameWithNoSuffix(namePart))
				{
					Response.StatusCode = 200;
					
					if (imageMeta == ImageMeta.Origin)
						Response.ContentType = doc.mime_type;
					else
						Response.ContentType = doc.image_meta.GetThumbnailInfo(imageMeta).mime_type;

					Wammer.Utility.StreamHelper.Copy(fs, Response.OutputStream);
					Response.OutputStream.Close();
				}
			}
			catch (ArgumentException e)
			{
				HttpHelper.RespondFailure(Response, e, (int)HttpStatusCode.BadRequest);
			}
			catch (FileNotFoundException e)
			{
				HttpHelper.RespondFailure(Response, e, (int)HttpStatusCode.NotFound);
			}
		}
	}
}

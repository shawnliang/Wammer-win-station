using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Web;

using Wammer.Cloud;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Wammer.Station
{
	public class ViewObjectHandler: HttpHandler
	{
		private readonly FileStorage fileStorage;
		private readonly MongoServer mongodb;
		private readonly MongoCollection<BsonDocument> attachments;

		public ViewObjectHandler(FileStorage fileStorage, MongoServer mongodb)
			:base()
		{
			this.fileStorage = fileStorage;
			this.mongodb = mongodb;
			this.attachments = mongodb.GetDatabase("wammer").
														GetCollection<BsonDocument>("attachments");
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


				using (FileStream fs = fileStorage.LoadByNameWithNoSuffix(namePart))
				{
					Response.StatusCode = 200;
					BsonDocument doc = attachments.FindOne(new QueryDocument("object_id", objectId));

					if (imageMeta == ImageMeta.Origin)
						Response.ContentType = doc["mime_type"].AsString;
					else
						Response.ContentType = doc["image_meta"].AsBsonDocument[metaStr]
							.AsBsonDocument["mime_type"].AsString;


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

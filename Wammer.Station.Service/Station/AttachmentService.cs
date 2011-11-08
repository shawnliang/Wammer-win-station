using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
using System.Net;

using Wammer.Cloud;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	[ServiceContract]
	public interface IAttatchmentService
	{
		[OperationContract]
		[WebGet(UriTemplate = "get?object_id={objId}&session_token={token}&apikey={apiKey}")]
		Stream GetAttachmentInfo(string objId, string token, string apiKey);
	}

	[ServiceBehavior(
		InstanceContextMode=InstanceContextMode.Single,
		ConcurrencyMode=ConcurrencyMode.Multiple)]
	public class AttachmentService: IAttatchmentService
	{
		private readonly MongoServer mongodb;
		private readonly MongoCollection<Attachment> attachments;

		public AttachmentService(MongoServer mongo)
		{
			this.mongodb = mongo;

			if (!mongo.GetDatabase("wammer").CollectionExists("attachments"))
				mongo.GetDatabase("wammer").CreateCollection("attachments");

			this.attachments = mongo.GetDatabase("wammer").
														GetCollection<Attachment>("attachments");
		}

		public Stream GetAttachmentInfo(string object_id, string session_token, string apikey)
		{
			try
			{
				if (object_id == null)
					return GenerateErrStream(WebOperationContext.Current,
											HttpStatusCode.BadRequest, "missing parameter: object_id");

				Attachment doc = attachments.FindOne(Query.EQ("_id", object_id));
				if (doc == null)
					return GenerateErrStream(WebOperationContext.Current,
										HttpStatusCode.NotFound, "object not found: " + object_id);

				MemoryStream s = new MemoryStream();
				StreamWriter w = new StreamWriter(s);
				string jsn = doc.ToJson();

				w.Write(fastJSON.JSON.Instance.ToJSON(doc, false, false, false, false));
				w.Flush();

				WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
				s.Position = 0;
				return s;
			}
			catch (Exception e)
			{
				return GenerateErrStream(WebOperationContext.Current,
					HttpStatusCode.InternalServerError, e.Message);
			}
		}

		private static MemoryStream GenerateErrStream(WebOperationContext webContext, HttpStatusCode status, string errMsg)
		{
			try
			{
				webContext.OutgoingResponse.ContentType = "application/json";
				webContext.OutgoingResponse.StatusCode = status;
				CloudResponse res = new CloudResponse(
								(int)status, -1, errMsg);
				MemoryStream m = new MemoryStream();
				StreamWriter w1 = new StreamWriter(m);
				w1.Write(fastJSON.JSON.Instance.ToJSON(res));
				w1.Flush();
				m.Position = 0;
				return m;
			}
			catch (Exception e)
			{
				webContext.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
				return null;
			}
		}
	}
}

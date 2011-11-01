using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;

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
		private readonly MongoCollection<BsonDocument> attachments;

		public AttachmentService(MongoServer mongo)
		{
			this.mongodb = mongo;

			if (!mongo.GetDatabase("wammer").CollectionExists("attachments"))
				mongo.GetDatabase("wammer").CreateCollection("attachments");

			this.attachments = mongo.GetDatabase("wammer").
														GetCollection<BsonDocument>("attachments");
		}

		public Stream GetAttachmentInfo(string object_id, string session_token, string apikey)
		{
			BsonDocument doc = attachments.FindOne(Query.EQ("object_id", object_id));
			doc.Remove("_id");

			MemoryStream s = new MemoryStream();
			StreamWriter w = new StreamWriter(s);

			w.Write(doc.ToJson());
			w.Flush();

			WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
			s.Position = 0;
			return s;
		}
	}
}

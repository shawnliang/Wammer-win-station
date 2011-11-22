using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Wammer.Model
{
	class CloudStorage
	{
		[BsonIgnore]
		public static MongoCollection<CloudStorage> collection = Database.wammer.GetCollection<CloudStorage>("cloudstorage");

		[BsonId]
		public string Id { get; set; }
		public string Type { get; set; }
		public string Folder { get; set; }
		public long Quota { get; set; }
		public long Used { get; set; }
	}
}

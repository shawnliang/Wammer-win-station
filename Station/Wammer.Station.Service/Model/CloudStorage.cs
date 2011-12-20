using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Wammer.Model
{
	[BsonIgnoreExtraElements]
	public class CloudStorage
	{
		[BsonId]
		public string Id { get; set; }
		[BsonIgnoreIfNull]
		public string Type { get; set; }
		[BsonIgnoreIfNull]
		public string Folder { get; set; }
		[BsonIgnoreIfNull]
		public long Quota { get; set; }
		[BsonIgnoreIfNull]
		public string UserAccount { get; set; }
	}

	public class CloudStorageCollection : Collection<CloudStorage>
	{
		private static CloudStorageCollection instance;

		static CloudStorageCollection()
		{
			instance = new CloudStorageCollection();
		}

		private CloudStorageCollection()
			: base("cloudstorage")
		{
		}

		public static CloudStorageCollection Instance
		{
			get { return instance; }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System.IO;

namespace Waveface.Stream.Model
{
	[BsonIgnoreExtraElements]
	public class ImportTaskStaus
	{
		[BsonId(IdGenerator=typeof(GuidGenerator))]
		public Guid Id { get; set; }

		public bool IsComplete { get; set; }

		[BsonIgnoreIfNull]
		public string UserId { get; set; }

		[BsonIgnoreIfNull]
		public List<ObjectIdAndPath> FailedFiles { get; set; }

		public int SuccessCount { get; set; }
		public int TotalFiles { get; set; }

		public bool Hidden { get; set; }

		public List<string> Sources { get; set; }
		public string Error { get; set; }
	}

	[BsonIgnoreExtraElements]
	public class ObjectIdAndPath
	{
		[BsonIgnoreIfNull]
		public string object_id { get; set; }
		[BsonIgnoreIfNull]
		public string file_path { get; set; }
		[BsonIgnoreIfNull]
		public string Error { get; set; }

		private DateTime? createTime;

		public DateTime CreateTime
		{
			get
			{
				if (createTime.HasValue)
					return createTime.Value;
				else
				{
					var t = File.GetCreationTime(file_path);
					createTime = new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Kind);
					return createTime.Value;
				}
			}
		}
	}
}

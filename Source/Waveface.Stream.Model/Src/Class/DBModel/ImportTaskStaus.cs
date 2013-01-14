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
		[BsonId]
		public Guid Id { get; set; }

		[BsonIgnoreIfNull]
		public string UserId { get; set; }

		[BsonIgnoreIfNull]
		public List<ObjectIdAndPath> CopyFailed { get; set; }
		
		[BsonIgnoreIfNull]
		public List<string> Sources { get; set; }

		[BsonIgnoreIfNull]
		public string Error { get; set; }

		public DateTime Time { get; set; }

		public bool Hidden { get; set; }

		public bool IsStarted { get; set; }
		public bool IsCopyComplete { get; set; }
		
		public int Total { get; set; }
		public int Skipped { get; set; }
		public int Indexed { get; set; }
		public int Copied { get; set; }
		public int Thumbnailed { get; set; }
		public int UploadedCount { get; set; }
		public int UploadedSize { get; set; }

		public int UploadCount { get; set; }
		public int UploadSize { get; set; }

		public int GetSkippedCount()
		{
			return Total - Indexed - (CopyFailed == null ? 0 : CopyFailed.Count);
		}

		public bool IsPending()
		{
			return TotalFiles == 0 && !IsComplete;
		}

		public ImportTaskStaus()
		{
			CopyFailed = new List<ObjectIdAndPath>();
			Sources = new List<string>();
		}
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


		private DateTime? _createTime;

		[BsonIgnore]
		public DateTime CreateTime
		{
			get
			{
				if (_createTime.HasValue)
					return _createTime.Value;
				else
				{
					_createTime = File.GetCreationTime(file_path).TrimToSec();
					return _createTime.Value;
				}
			}
		}
	}
}

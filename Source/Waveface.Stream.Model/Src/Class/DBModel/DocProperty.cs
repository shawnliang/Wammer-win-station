using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Waveface.Stream.Model
{
	[BsonIgnoreExtraElements]
	public class DocProperty
	{
		[XmlIgnore]
		[BsonIgnoreIfDefault]
		[BsonIgnoreIfNull]
		public List<string> preview_files { get; set; }

		[BsonIgnoreIfDefault]
		[BsonIgnoreIfNull]
		public List<DateTime> access_time { get; set; }

		[BsonIgnoreIfDefault]
		[BsonIgnoreIfNull]
		public DateTime modify_time { get; set; }

		[BsonIgnoreIfNull]
		public string title { get; set; }

		[BsonIgnoreIfNull]
		public string author { get; set; }

		public int preview_pages { get; set; }

		[BsonIgnore]
		public string file_name { get; set; }
	}
}

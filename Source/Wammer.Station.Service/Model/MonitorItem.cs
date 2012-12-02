using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Model
{
	[BsonIgnoreExtraElements]
	public class MonitorItem
	{
		[BsonId]
		public string id { get; set; }
		[BsonIgnoreIfNull]
		public string path { get; set; }
		[BsonIgnoreIfNull]
		public string user_id {get;set;}
		[BsonIgnoreIfNull]
		public DateTime last_modify_time { get; set; }

		public MonitorItem()
		{
		}

		public MonitorItem(string path, string user_id)
		{
			this.path = path;
			this.user_id = user_id;
			this.id = path + "/" + user_id;
		}
	}
}

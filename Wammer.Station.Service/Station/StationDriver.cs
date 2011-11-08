using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Wammer.Cloud;
using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Station
{

	public class StationDriver
	{
		[BsonId]
		public string _Id { get; set; }
		public string email { get; set; }
		public string folder { get; set; }
		public string user_id { get; set; }
		public List<UserGroup> groups { get; set; }

		public StationDriver()
		{
			groups = new List<UserGroup>();
		}
	}
}

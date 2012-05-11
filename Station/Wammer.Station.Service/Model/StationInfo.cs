using System;

using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Model
{
	[BsonIgnoreExtraElements]
	public class StationInfo
	{	
		[BsonId]
		public string Id { get; set; }
		[BsonIgnoreIfNull]
		public string SessionToken { get; set; }
		[BsonIgnoreIfNull]
		public DateTime LastLogOn { get; set; }
		[BsonIgnoreIfNull]
		public string Location { get; set; }
	}

	public class StationCollection : Collection<StationInfo>
	{
		private static readonly StationCollection instance;

		static StationCollection()
		{
			instance = new StationCollection();
		}

		private StationCollection()
			:base("station")
		{
		}

		public static StationCollection Instance
		{
			get { return instance; }
		}
	}
}

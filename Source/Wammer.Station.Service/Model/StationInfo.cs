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
		#region Var
		private static StationCollection _instance; 
		#endregion

		#region Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static StationCollection Instance
		{
			get { return _instance ?? (_instance = new StationCollection()); }
		}
		#endregion


		private StationCollection()
			: base("station")
		{
		}
	}
}
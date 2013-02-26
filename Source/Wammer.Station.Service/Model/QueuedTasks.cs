using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Model
{
	[BsonIgnoreExtraElements]
	public class QueuedTask
	{
		[BsonId]
		public string id { get; set; }

		[BsonIgnoreIfNull]
		public string queue { get; set; }

		[BsonIgnoreIfNull]
		public Wammer.Station.ITask Data { get; set; }
	}

	public class QueuedTaskCollection : Collection<QueuedTask>
	{
		#region Var
		private static QueuedTaskCollection _instance;
		#endregion

		#region Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static QueuedTaskCollection Instance
		{
			get { return _instance ?? (_instance = new QueuedTaskCollection()); }
		}
		#endregion

		private QueuedTaskCollection()
			: base("queued_tasks")
		{
		}
	}
}
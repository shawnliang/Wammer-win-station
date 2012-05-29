using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Model
{
	[BsonIgnoreExtraElements]
	public class QueuedTask
	{
		[BsonId]
		public Guid id { get; set; }

		[BsonIgnoreIfNull]
		public string queue { get; set; }

		[BsonIgnoreIfNull]
		public byte[] bytes { get; set; }

		[BsonIgnore]
		public object Data
		{
			get
			{
				if (bytes == null)
					return null;

				var f = new BinaryFormatter();
				using (var m = new MemoryStream(bytes))
				{
					return f.Deserialize(m);
				}
			}

			set
			{
				using (var m = new MemoryStream())
				{
					var f = new BinaryFormatter();
					f.Serialize(m, value);
					bytes = m.ToArray();
				}
			}
		}
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
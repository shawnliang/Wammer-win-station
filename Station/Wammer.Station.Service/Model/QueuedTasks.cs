using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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

	public class QueuedTaskCollection: Collection<QueuedTask>
	{
		private static readonly QueuedTaskCollection instance;

		static QueuedTaskCollection()
		{
			instance = new QueuedTaskCollection();
		}

		private QueuedTaskCollection()
			:base("queued_tasks")
		{
		}

		public static QueuedTaskCollection Instance
		{
			get { return instance; }
		}
	}
}

using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Model
{
	public enum ServiceState
	{
		Online,
		Offline
	}

	[BsonIgnoreExtraElements]
	public class Service
	{
		[BsonId]
		public string Id { get; set; }

		[BsonIgnoreIfNull]
		public ServiceState State { get; set; }
	}


	public class ServiceCollection : Collection<Service>
	{
		private static readonly ServiceCollection instance = new ServiceCollection();

		private ServiceCollection()
			: base("service")
		{
		}

		public static ServiceCollection Instance
		{
			get { return instance; }
		}
	}
}
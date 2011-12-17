using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

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
		private static ServiceCollection instance;

		private ServiceCollection()
			:base("service")
		{
		}

		static ServiceCollection()
		{
			instance = new ServiceCollection();
		}

		public static ServiceCollection Instance
		{
			get { return instance; }
		}
	}
}

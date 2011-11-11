using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Wammer.Station
{
	public class StationDBDoc
	{
		[BsonId]
		public string Id { get; set; }
		public string SessionToken { get; set; }
	}

	public class StationInfo
	{
		public static IPAddress IPv4Address { get; private set; }
		public static string BaseURL { get; private set; }
		private static StationDBDoc station;
		private static MongoCollection<StationDBDoc> dbDocs;

		public static void Init(MongoServer mongodb)
		{
			dbDocs = InitDbDocs(mongodb);
			station = Load();
		}

		static StationInfo()
		{
			IPv4Address = GetIPAddressesV4()[0];
			IPv4Address = GetIPAddressesV4()[0];
			BaseURL = string.Format("http://{0}:9981/{1}/", IPv4Address,
									Cloud.CloudServer.DEF_BASE_PATH);
		}

		private static IPAddress[] GetIPAddressesV4()
		{
			IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
			List<IPAddress> ret = new List<IPAddress>();

			foreach (IPAddress ip in ips)
			{
				if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
					!IPAddress.IsLoopback(ip))
					ret.Add(ip);
			}

			return ret.ToArray();
		}

		public static string Id
		{
			get { return station.Id; }
			set	{ station.Id = value; }
		}

		public static string SessionToken
		{
			get
			{
				lock (station)
				{
					return station.SessionToken;
				}
			}

			set
			{
				lock (station)
				{
					station.SessionToken = value;
				}
			}
		}

		public static void Save()
		{
			lock (station)
			{
				dbDocs.Save(station);
			}
		}

		private static StationDBDoc Load()
		{
			StationDBDoc doc = dbDocs.FindOne();
			if (doc == null)
				return new StationDBDoc { Id = Guid.NewGuid().ToString() };
			else
				return doc;
		}

		private static MongoCollection<StationDBDoc> InitDbDocs(MongoServer mongodb)
		{
			MongoDatabase db = mongodb.GetDatabase("wammer");
			if (!db.CollectionExists("station"))
				db.CreateCollection("station");

			return db.GetCollection<StationDBDoc>("station");
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Wammer.Cloud;
using Wammer.Station;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Model
{
	class Drivers
	{
		public static MongoCollection<Drivers> collection = Database.wammer.GetCollection<Drivers>("drivers");

		[BsonId]
		public string user_id { get; set; }
		public string email { get; set; }
		public string folder { get; set; }
		public List<UserGroup> groups { get; set; }

		public Drivers()
		{
			groups = new List<UserGroup>();
		}

		public static void RequestToAdd(string url, string email, string password, string folder)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{"email", email},
				{"password", password},
				{"folder", folder}
			};

			CloudResponse res = CloudServer.request<CloudResponse>(
				new WebClient(), url, parameters);

			if (res.api_ret_code != 0)
				throw new WammerCloudException(
					"Unable to add user", WebExceptionStatus.Success, res.api_ret_code);
		}
	}
}

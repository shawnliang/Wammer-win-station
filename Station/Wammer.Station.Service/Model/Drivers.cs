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
	public class Drivers
	{
		[BsonIgnore]
		public static MongoCollection<Drivers> collection = Database.wammer.GetCollection<Drivers>("drivers");

		[BsonId]
		public string user_id { get; set; }
		public string email { get; set; }
		public string folder { get; set; }
		public List<UserGroup> groups { get; set; }
		public string stssion_token { get; set; }

		public Drivers()
		{
			groups = new List<UserGroup>();
		}

		public static void RequestToAdd(string url, string email, string password)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{"email", email},
				{"password", password},
			};

			try
			{
				CloudResponse res = CloudServer.request<CloudResponse>(
					new WebClient(), "http://localhost:9981/v2/station/drivers/add", parameters);
			}
			catch (WebException e)
			{
				HttpWebResponse resp = (HttpWebResponse)e.Response;
				//resp.
				//switch (res.api_ret_code)
				//{
				//    case (int)StationApiError.DriverExist:

				//        break;
				//    case (int)StationApiError.AuthFailed:

				//        break;
				//    case (int)StationApiError.AlreadyHasStaion:
				//        HandleAlreadyHasStaion(ex.response);
				//        break;
				//    default:
				//        MessageBox.Show("Unknown error :" + ex.ToString());
				//        break;
				//}
			}

			//if (res.api_ret_code != 0)
			//    throw new WammerCloudException(
			//        "Unable to add user", WebExceptionStatus.Success, res.api_ret_code);
		}
	}
}

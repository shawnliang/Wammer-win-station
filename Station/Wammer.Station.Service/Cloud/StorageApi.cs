using System.Collections.Generic;
using System.Net;
using MongoDB.Driver.Builders;
using Wammer.Model;

namespace Wammer.Cloud
{
	public class StorageApi
	{
		public StorageApi(string user_id)
		{
			Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", user_id));
			userToken = driver.session_token;
		}

		private string userToken { get; set; }

		public StorageAuthResponse StorageAuthorize(WebClient agent, string type)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{"type", type},
			                 		{CloudServer.PARAM_SESSION_TOKEN, userToken},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			var res = CloudServer.requestPath<StorageAuthResponse>(agent, "storages/authorize", parameters);
			return res;
		}

		public StorageLinkResponse StorageLink(WebClient agent, string type)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{"type", type},
			                 		{CloudServer.PARAM_SESSION_TOKEN, userToken},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			var res = CloudServer.requestPath<StorageLinkResponse>(agent, "storages/link", parameters);
			return res;
		}

		public StorageCheckResponse StorageCheck(WebClient agent, string type)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{"type", type},
			                 		{CloudServer.PARAM_SESSION_TOKEN, userToken},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			var res = CloudServer.requestPath<StorageCheckResponse>(agent, "storages/check", parameters);
			return res;
		}

		public void StorageUnlink(WebClient agent, string type)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{"type", type},
			                 		{CloudServer.PARAM_SESSION_TOKEN, userToken},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			CloudServer.requestPath<StorageResponse>(agent, "storages/unlink", parameters);
		}
	}

	public class CloudStorageType
	{
		public const string DROPBOX = "dropbox";
	};
}
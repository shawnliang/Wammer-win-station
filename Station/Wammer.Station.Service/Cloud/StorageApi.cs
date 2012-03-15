using System.Collections.Generic;
using System.Net;
using MongoDB.Driver.Builders;
using Wammer.Model;

namespace Wammer.Cloud
{
	public class StorageApi
	{
		private string userToken { get; set; }

		public StorageApi(string user_id)
		{
			Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", user_id));
			this.userToken = driver.session_token;
		}

		public StorageAuthResponse StorageAuthorize(WebClient agent, string type)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ "type", type },
				{ CloudServer.PARAM_SESSION_TOKEN, this.userToken },
				{ CloudServer.PARAM_API_KEY, CloudServer.APIKey }
			};

			StorageAuthResponse res = CloudServer.requestPath<StorageAuthResponse>(agent, "storages/authorize", parameters);
			return res;
		}

		public StorageLinkResponse StorageLink(WebClient agent, string type)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ "type", type },
				{ CloudServer.PARAM_SESSION_TOKEN, this.userToken },
				{ CloudServer.PARAM_API_KEY, CloudServer.APIKey }
			};

			StorageLinkResponse res = CloudServer.requestPath<StorageLinkResponse>(agent, "storages/link", parameters);
			return res;
		}

		public StorageCheckResponse StorageCheck(WebClient agent, string type)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ "type", type },
				{ CloudServer.PARAM_SESSION_TOKEN, this.userToken },
				{ CloudServer.PARAM_API_KEY, CloudServer.APIKey }
			};

			StorageCheckResponse res = CloudServer.requestPath<StorageCheckResponse>(agent, "storages/check", parameters);
			return res;
		}

		public void StorageUnlink(WebClient agent, string type)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ "type", type },
				{ CloudServer.PARAM_SESSION_TOKEN, this.userToken },
				{ CloudServer.PARAM_API_KEY, CloudServer.APIKey }
			};

			CloudServer.requestPath<StorageResponse>(agent, "storages/unlink", parameters);
		}
	}

	public class CloudStorageType
	{
		public const string DROPBOX = "dropbox";
	};
}

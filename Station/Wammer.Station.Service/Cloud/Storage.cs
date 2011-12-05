using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Wammer.Model;
using Wammer.Utility;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Wammer.Cloud
{
	public class Storage
	{
		private string userToken { get; set; }

		public Storage(string user_id)
		{
			Drivers driver = Drivers.collection.FindOne(Query.EQ("_id", user_id));
			this.userToken = driver.session_token;
		}

		public StorageAuthResponse StorageAuthorize(WebClient agent, Dictionary<object, object> param)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>(param);
			parameters.Add(CloudServer.PARAM_SESSION_TOKEN, this.userToken);
			parameters.Add(CloudServer.PARAM_API_KEY, CloudServer.APIKey);

			StorageAuthResponse res = CloudServer.requestPath<StorageAuthResponse>(agent, "storages/authorize", parameters);
			return res;
		}

		public StorageLinkResponse StorageLink(WebClient agent, Dictionary<object, object> param)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>(param);
			parameters.Add(CloudServer.PARAM_SESSION_TOKEN, this.userToken);
			parameters.Add(CloudServer.PARAM_API_KEY, CloudServer.APIKey);

			StorageLinkResponse res = CloudServer.requestPath<StorageLinkResponse>(agent, "storages/link", parameters);
			return res;
		}

		public StorageCheckResponse StorageCheck(WebClient agent, Dictionary<object, object> param)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>(param);
			parameters.Add(CloudServer.PARAM_SESSION_TOKEN, this.userToken);
			parameters.Add(CloudServer.PARAM_API_KEY, CloudServer.APIKey);

			StorageCheckResponse res = CloudServer.requestPath<StorageCheckResponse>(agent, "storages/check", parameters);
			return res;
		}

		public void StorageUnlink(WebClient agent, Dictionary<object, object> param)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>(param);
			parameters.Add(CloudServer.PARAM_SESSION_TOKEN, this.userToken);
			parameters.Add(CloudServer.PARAM_API_KEY, CloudServer.APIKey);

			CloudServer.requestPath<StorageResponse>(agent, "storages/unlink", parameters);
		}
	}
}

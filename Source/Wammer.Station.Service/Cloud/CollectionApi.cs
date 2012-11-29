using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Utility;

namespace Wammer.Cloud
{
	public static class CollectionApi
	{
		public static void CreateCollection(string session, string apikey, string name, IEnumerable<string> attIds, string collectionId = null, DateTime? create_time = null)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			
			if (attIds == null)
				throw new ArgumentNullException("attIds");

			var id_list = "[" + String.Join(",", attIds.Select((x) => "\"" + x + "\"").ToArray()) + "]";

			Dictionary<object, object> parameters = new Dictionary<object, object>{
				{ CloudServer.PARAM_API_KEY, apikey },
				{ CloudServer.PARAM_SESSION_TOKEN, session },
				{ CloudServer.PARAM_NAME, name },
				{ CloudServer.PARAM_OBJECT_ID_LIST, id_list },
			};

			if (!string.IsNullOrEmpty(collectionId))
				parameters.Add("collection_id", collectionId);
			
			if (create_time.HasValue)
				parameters.Add("create_time", create_time.Value.ToCloudTimeString());

			CloudServer.requestPath<CloudResponse>("collections/create", parameters);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Utility;

namespace Wammer.Cloud
{
	public static class CollectionApi
	{
		/// <summary>
		/// Creates the collection.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="apikey">The apikey.</param>
		/// <param name="name">The name.</param>
		/// <param name="attIds">The att ids.</param>
		/// <param name="collectionId">The collection id.</param>
		/// <param name="create_time">The create_time.</param>
		/// <exception cref="System.ArgumentNullException">name</exception>
		public static void CreateCollection(string session, string apikey, string name, IEnumerable<string> attIds, string collectionId = null, string cover = null, bool? manual = null, DateTime? create_time = null)
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

			if (!string.IsNullOrEmpty(cover))
				parameters.Add("cover", cover);

			if (manual.HasValue)
				parameters.Add("manual", manual.Value.ToString());

			if (create_time.HasValue)
				parameters.Add("create_time", create_time.Value.ToCloudTimeString());

			CloudServer.requestPath<CloudResponse>("collections/create", parameters);
		}

		/// <summary>
		/// Updates the collection.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="apikey">The apikey.</param>
		/// <param name="collectionID">The collection ID.</param>
		/// <param name="attIds">The att ids.</param>
		/// <param name="modifyTime">The modify time.</param>
		/// <param name="name">The name.</param>
		/// <exception cref="System.ArgumentNullException">name</exception>
		public static void UpdateCollection(string session, string apikey, string collectionID, IEnumerable<string> attIds, DateTime? modifyTime = null, string name = null)
		{
			if (string.IsNullOrEmpty(collectionID))
				throw new ArgumentNullException("collectionID");

			if (attIds == null)
				throw new ArgumentNullException("attIds");

			var id_list = "[" + String.Join(",", attIds.Select((x) => "\"" + x + "\"").ToArray()) + "]";

			Dictionary<object, object> parameters = new Dictionary<object, object>{
				{ CloudServer.PARAM_API_KEY, apikey },
				{ CloudServer.PARAM_SESSION_TOKEN, session },
				{ CloudServer.PARAM_COLLECTION_ID, collectionID },
				{ CloudServer.PARAM_OBJECT_ID_LIST, id_list },
			};

			if (modifyTime.HasValue)
				parameters.Add(CloudServer.PARAM_MODIFY_TIME, modifyTime.Value.ToCloudTimeString());


			if (!string.IsNullOrEmpty(name))
				parameters.Add(CloudServer.PARAM_NAME, name);

			CloudServer.requestPath<CloudResponse>("collections/update", parameters);
		}

		/// <summary>
		/// Hides the collection.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="apikey">The apikey.</param>
		/// <param name="collectionID">The collection ID.</param>
		/// <param name="modifyTime">The modify time.</param>
		/// <exception cref="System.ArgumentNullException">collectionID</exception>
		public static void HideCollection(string session, string apikey, string collectionID, DateTime? modifyTime = null)
		{
			if (string.IsNullOrEmpty(collectionID))
				throw new ArgumentNullException("collectionID");

			if (!modifyTime.HasValue)
				throw new ArgumentNullException("modifyTime");

			Dictionary<object, object> parameters = new Dictionary<object, object>{
				{ CloudServer.PARAM_API_KEY, apikey },
				{ CloudServer.PARAM_SESSION_TOKEN, session },
				{ CloudServer.PARAM_COLLECTION_ID, collectionID },
				{ CloudServer.PARAM_MODIFY_TIME, modifyTime.Value.ToCloudTimeString() },
			};

			CloudServer.requestPath<CloudResponse>("collections/hide", parameters);
		}

		/// <summary>
		/// Uns the hide collection.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="apikey">The apikey.</param>
		/// <param name="collectionID">The collection ID.</param>
		/// <param name="modifyTime">The modify time.</param>
		/// <exception cref="System.ArgumentNullException">collectionID</exception>
		public static void UnHideCollection(string session, string apikey, string collectionID, DateTime? modifyTime = null)
		{
			if (string.IsNullOrEmpty(collectionID))
				throw new ArgumentNullException("collectionID");

			if (!modifyTime.HasValue)
				throw new ArgumentNullException("modifyTime");

			Dictionary<object, object> parameters = new Dictionary<object, object>{
				{ CloudServer.PARAM_API_KEY, apikey },
				{ CloudServer.PARAM_SESSION_TOKEN, session },
				{ CloudServer.PARAM_COLLECTION_ID, collectionID },
				{ CloudServer.PARAM_MODIFY_TIME, modifyTime.Value.ToCloudTimeString() },
			};

			CloudServer.requestPath<CloudResponse>("collections/unhide", parameters);
		}

		/// <summary>
		/// Gets all collections.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="apikey">The apikey.</param>
		/// <returns></returns>
		public static CollectionsResponse GetAllCollections(string session, string apikey)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>{
				{ CloudServer.PARAM_API_KEY, apikey },
				{ CloudServer.PARAM_SESSION_TOKEN, session }
			};

			return CloudServer.requestPath<CollectionsResponse>("collections/getAll", parameters);
		}

		/// <summary>
		/// Gets all collections.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="apikey">The apikey.</param>
		/// <returns></returns>
		public static CollectionsResponse GetAllCollections(string session, string apikey, IEnumerable<string> collectionIDs)
		{
			var id_list = "[" + String.Join(",", collectionIDs.Select((x) => "\"" + x + "\"").ToArray()) + "]";

			Dictionary<object, object> parameters = new Dictionary<object, object>{
				{ CloudServer.PARAM_API_KEY, apikey },
				{ CloudServer.PARAM_SESSION_TOKEN, session },
				{CloudServer.PARAM_COLLECTION_ID_LIST, id_list}
			};

			return CloudServer.requestPath<CollectionsResponse>("collections/getByIds", parameters);
		}
	}
}

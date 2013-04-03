using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Waveface.Stream.Model;

namespace Waveface.Stream.Core
{
	[Obfuscation]
	public class DeleteAttachmentsCommand : WebSocketCommandBase
	{
		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "deleteAttachments"; }
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Executes the specified parameters.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public override Dictionary<string, Object> Execute(WebSocketCommandData data, Dictionary<string, Object> systemArgs = null)
		{
			var parameters = data.Parameters;

			var sessionToken = parameters.ContainsKey("session_token") ? parameters["session_token"].ToString() : string.Empty;

			if (sessionToken.Length == 0)
				return null;

			var loginedUser = LoginedSessionCollection.Instance.FindOneById(sessionToken);

			if (loginedUser == null)
				return null;

			var userID = loginedUser.user.user_id;
			var groupID = loginedUser.groups.First().group_id;

			var sinceDate = parameters.ContainsKey("since_date") ? DateTime.Parse(parameters["since_date"].ToString()) : default(DateTime?);
			var untilDate = parameters.ContainsKey("until_date") ? DateTime.Parse(parameters["until_date"].ToString()) : default(DateTime?);

			var attachmentQueryParam = Query.EQ("group_id", groupID);

			if (sinceDate != null)
				attachmentQueryParam = Query.And(attachmentQueryParam, Query.GTE("event_time", sinceDate));

			if (untilDate != null)
				attachmentQueryParam = Query.And(attachmentQueryParam, Query.LT("event_time", untilDate));

			var attachmentIDs = new HashSet<string>();
			if (parameters.ContainsKey("post_id_array"))
			{
				var postIDs = (parameters["post_id_array"] as JArray).Values();

				attachmentIDs.UnionWith(from postID in postIDs
										let post = PostDBDataCollection.Instance.FindOneById(postID.ToString())
										where post != null
										from attachmentID in post.AttachmentIDs
										select attachmentID);
			}

			if (parameters.ContainsKey("collection_id_array"))
			{
				var collectionIDs = (parameters["collection_id_array"] as JArray).Values();

				attachmentIDs.UnionWith(from collectionID in collectionIDs
										let collection = CollectionCollection.Instance.FindOneById(collectionID.ToString())
										where collection != null
										from attachmentID in collection.attachment_id_array
										select attachmentID);
			}

			if (parameters.ContainsKey("attachment_id_array"))
			{
				attachmentIDs.UnionWith(from attachmentID in (parameters["attachment_id_array"] as JArray).Values()
										select attachmentID.ToString());
			}

			if (attachmentIDs.Any())
				attachmentQueryParam = Query.And(attachmentQueryParam, Query.In("_id", new BsonArray(attachmentIDs)));

			var type = parameters.ContainsKey("type") ? int.Parse(parameters["type"].ToString()) : 0;

			if (type != 0)
			{
				var queryTypes = new List<int>();

				if ((type & 1) == 1)
				{
					queryTypes.Add((int)AttachmentType.image);
				}

				if ((type & 8) == 8)
				{
					queryTypes.Add((int)AttachmentType.doc);
				}

				if ((type & 16) == 16)
				{
					queryTypes.Add((int)AttachmentType.webthumb);
				}

				attachmentQueryParam = Query.And(attachmentQueryParam, Query.In("type", new BsonArray(queryTypes)));
			}

			var attachments = AttachmentCollection.Instance.Find(attachmentQueryParam);

			StationAPI.DeleteAttachments(sessionToken, attachments.Select(attachment => attachment.object_id).ToArray());

			AttachmentCollection.Instance.Remove(attachmentQueryParam);

			return null;
		}
		#endregion
	}
}

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
	public class GetAttachmentsCommand : WebSocketCommandBase
	{
		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "getAttachments"; }
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
			var pageNo = parameters.ContainsKey("page_no") ? int.Parse(parameters["page_no"].ToString()) : 1;
			var pageSize = parameters.ContainsKey("page_size") ? int.Parse(parameters["page_size"].ToString()) : 10;
			var skipCount = (pageNo == 1) ? 0 : (pageNo - 1) * pageSize;

			var attachmentQueryParam = Query.EQ("group_id", groupID);

			if (sinceDate != null)
				attachmentQueryParam = Query.And(attachmentQueryParam, Query.GTE("event_time", sinceDate));

			if (untilDate != null)
				attachmentQueryParam = Query.And(attachmentQueryParam, Query.LT("event_time", untilDate));

			var coverAttachmentIDs = new HashSet<string>();
			var attachmentIDs = new HashSet<string>();
			if (parameters.ContainsKey("post_id_array"))
			{
				var postIDs = (parameters["post_id_array"] as JArray).Values();
				coverAttachmentIDs.UnionWith(from postID in postIDs
											 let post = PostCollection.Instance.FindOneById(postID.ToString())
											 where post != null
											 select post.cover_attach);

				attachmentIDs.UnionWith(from postID in postIDs
										let post = PostCollection.Instance.FindOneById(postID.ToString())
										where post != null
										from attachmentID in post.attachment_id_array
										select attachmentID);
			}

			if (parameters.ContainsKey("collection_id_array"))
			{
				var collectionIDs = (parameters["collection_id_array"] as JArray).Values();
				coverAttachmentIDs.UnionWith(from collectionID in collectionIDs
											 let collection = CollectionCollection.Instance.FindOneById(collectionID.ToString())
											 where collection != null
											 select collection.cover);

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

				attachmentQueryParam = Query.And(attachmentQueryParam, Query.In("type", new BsonArray(queryTypes)));
			}

			var attachments = AttachmentCollection.Instance.Find(attachmentQueryParam)
				.SetSortOrder(SortBy.Descending("event_time")).AsEnumerable();

			var totalCount = attachments.Count();
			var pageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);

			var coverAttachments = attachments.Where((attachment) => coverAttachmentIDs.Contains(attachment.object_id));
			var normalAttachments = attachments.Except(coverAttachments, new AttachmentCompare());

			attachments = coverAttachments.Union(normalAttachments)
				.Skip(skipCount)
				.Take(pageSize);

			var dataSize = parameters.ContainsKey("data_size") ? int.Parse(parameters["data_size"].ToString()) : 1;

			Object attachmentDatas = null;
			if (dataSize == 2)
			{
				attachmentDatas = Mapper.Map<IEnumerable<Attachment>, IEnumerable<LargeSizeAttachmentData>>(attachments);
			}
			else
			{
				attachmentDatas = Mapper.Map<IEnumerable<Attachment>, IEnumerable<MediumSizeAttachmentData>>(attachments);
			}


			return new Dictionary<string, Object>() 
			{
				{"attachments", attachmentDatas},
				{"page_no", pageNo},
				{"page_size", pageSize},
				{"page_count", pageCount},
			    {"total_count", totalCount}
			};
		}
		#endregion
	}
}

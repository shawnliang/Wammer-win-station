using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Waveface.Stream.Model;

namespace Waveface.Stream.Core
{
	public class GetCollectionsCommand : WebSocketCommandBase
	{
		#region Const
		const string OBJECT_GROUP_ID = @"ObjectID";
		const string OBJCCT_MATCH_PATTERN = @"object_id=(?<" + OBJECT_GROUP_ID + @">[^&]+)";
		#endregion


		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "getCollections"; }
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

			var sinceDate = parameters.ContainsKey("since_date") ? DateTime.Parse(parameters["since_date"].ToString()) : default(DateTime?);
			var untilDate = parameters.ContainsKey("until_date") ? DateTime.Parse(parameters["until_date"].ToString()) : default(DateTime?);
			var pageNo = parameters.ContainsKey("page_no") ? int.Parse(parameters["page_no"].ToString()) : 1;
			var pageSize = parameters.ContainsKey("page_size") ? int.Parse(parameters["page_size"].ToString()) : 10;
			var skipCount = (pageNo == 1) ? 0 : (pageNo - 1) * pageSize;
			var queryParam = Query.And(Query.EQ("creator_id", userID), Query.EQ("hidden", false));

			if (parameters.ContainsKey("collection_id_array"))
			{
				var collectionIDs = from collectionID in (parameters["collection_id_array"] as JArray).Values()
									select collectionID.ToString();
				queryParam = Query.And(queryParam, Query.In("_id", new BsonArray(collectionIDs)));
			}

			if (sinceDate != null)
				queryParam = Query.And(queryParam, Query.GTE("modify_time", sinceDate.Value.ToUTCISO8601ShortString()));

			if (untilDate != null)
				queryParam = Query.And(queryParam, Query.LT("modify_time", untilDate.Value.ToUTCISO8601ShortString()));


			var filteredCollections = CollectionCollection.Instance.Find(queryParam)
				.SetSkip(skipCount)
				.SetLimit(pageSize)
				.SetSortOrder(SortBy.Descending("modify_time"));


			var totalCount = filteredCollections.Count();
			var pageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);


			var dataSize = parameters.ContainsKey("data_size") ? int.Parse(parameters["data_size"].ToString()) : 1;

			Object collectionDatas;
			if (dataSize == 0)
			{
				collectionDatas = Mapper.Map<IEnumerable<Collection>, IEnumerable<SmallSizeCollcetionData>>(filteredCollections);
			}
			else
			{
				collectionDatas = Mapper.Map<IEnumerable<Collection>, IEnumerable<MediumSizeCollcetionData>>(filteredCollections).ToArray();

				var summaryAttachmentLimit = parameters.ContainsKey("sumary_limit") ? int.Parse(parameters["sumary_limit"].ToString()) : 1;

				var idx = 0;
				foreach (var collectionData in (collectionDatas as IEnumerable<MediumSizeCollcetionData>))
				{
					if (summaryAttachmentLimit > 0)
					{
						var attachmentIDs = collectionData.AttachmentIDs;
						var summaryAttachmentDatas = new List<MediumSizeAttachmentData>(summaryAttachmentLimit);

						var coverAttachmentID = collectionData.CoverAttachmentID;
						var coverAttachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", coverAttachmentID));

						if (coverAttachment != null)
						{
							var coverAttachmentData = Mapper.Map<Attachment, MediumSizeAttachmentData>(coverAttachment);
							summaryAttachmentDatas.Add(coverAttachmentData);
						}

						foreach (var attachmentID in attachmentIDs)
						{
							if (summaryAttachmentDatas.Count >= summaryAttachmentLimit)
								break;

							if (attachmentID == coverAttachmentID)
								continue;

							var attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachmentID));

							if (attachment == null)
								continue;

							var attachmentData = Mapper.Map<Attachment, MediumSizeAttachmentData>(attachment);
							summaryAttachmentDatas.Add(attachmentData);
						}

						if (summaryAttachmentDatas.Count > 0)
							collectionData.SummaryAttachments = summaryAttachmentDatas;

						++idx;
					}
				}
			}

			return new Dictionary<string, Object>() 
			{
				{"collections", collectionDatas},
				{"page_no", pageNo},
				{"page_size", pageSize},
				{"page_count", pageCount},
			    {"total_count", totalCount}
			};
		}
		#endregion
	}
}

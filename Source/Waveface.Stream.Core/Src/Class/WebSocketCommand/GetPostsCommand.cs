using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Waveface.Stream.Model;

namespace Waveface.Stream.Core
{
	public class GetPostsCommand : WebSocketCommandBase
	{
		#region Const
		const string OBJECT_GROUP_ID = @"ObjectID";
		const string OBJCCT_MATCH_PATTERN = @"object_id=(?<" + OBJECT_GROUP_ID + @">[^&]+)";
		#endregion


		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public override string Name
		{
			get { return "getPosts"; }
		}
		#endregion



		#region Private Method
		private string GetStationAttachmentUrl(string url)
		{
			var loginedUser = LoginedSessionCollection.Instance.FindOne();
			return string.Format(@"http://127.0.0.1:9981{2}&apikey={0}&session_token={1}",
				StationAPI.API_KEY,
				HttpUtility.UrlEncode(loginedUser.session_token),
				url);
		}

		private string GetStationAttachmentUrl(string attachmentID, ImageMeta imageMeta)
		{
			var loginedUser = LoginedSessionCollection.Instance.FindOne();
			return string.Format(@"http://127.0.0.1:9981/v3/attachments/view/?apikey={0}&session_token={1}&object_id={2}&image_meta={3}",
				StationAPI.API_KEY,
				HttpUtility.UrlEncode(loginedUser.session_token),
				attachmentID,
				imageMeta);
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
			var queryParam = Query.And(Query.EQ("creator_id", userID), Query.NE("visibility", false));

			if (parameters.ContainsKey("post_id_array"))
			{
				var postIDs = from postID in (parameters["post_id_array"] as JArray).Values()
							  select postID.ToString();
				queryParam = Query.And(queryParam, Query.In("_id", new BsonArray(postIDs)));
			}


			var type = parameters.ContainsKey("type") ? (PostType)int.Parse(parameters["type"].ToString()) : PostType.All;

			if (type != PostType.All)
				queryParam = Query.And(queryParam, Query.EQ("type", type));


			if (sinceDate != null)
				queryParam = Query.And(queryParam, Query.GTE("event_since_time", sinceDate.Value.ToUTCISO8601ShortString()));

			if (untilDate != null)
				queryParam = Query.And(queryParam, Query.LT("event_since_time", untilDate.Value.ToUTCISO8601ShortString()));


			var filteredPosts = PostDBDataCollection.Instance.Find(queryParam)
				.SetSkip(skipCount)
				.SetLimit(pageSize)
				.SetSortOrder(SortBy.Descending("event_since_time"));


			var totalCount = filteredPosts.Count();
			var pageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);


			var dataSize = parameters.ContainsKey("data_size") ? int.Parse(parameters["data_size"].ToString()) : 1;

			Object postDatas;
			if (dataSize == 0)
			{
				postDatas = Mapper.Map<IEnumerable<PostDBData>, IEnumerable<SmallSizePostData>>(filteredPosts);
			}
			else
			{
				postDatas = Mapper.Map<IEnumerable<PostDBData>, IEnumerable<MediumSizePostData>>(filteredPosts).ToArray();

				var summaryAttachmentLimit = parameters.ContainsKey("sumary_limit") ? int.Parse(parameters["sumary_limit"].ToString()) : 1;

				var idx = 0;
				foreach (var postData in (postDatas as IEnumerable<MediumSizePostData>))
				{
					if (summaryAttachmentLimit > 0)
					{
						var attachmentIDs = postData.AttachmentIDs;
						var summaryAttachmentDatas = new List<MediumSizeAttachmentData>(summaryAttachmentLimit);

						var coverAttachmentID = postData.CoverAttachmentID;

						if (coverAttachmentID != null)
						{
							var coverAttachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", coverAttachmentID));

							if (coverAttachment != null)
							{
								var coverAttachmentData = Mapper.Map<Attachment, MediumSizeAttachmentData>(coverAttachment);
								summaryAttachmentDatas.Add(coverAttachmentData);
							}
							else
							{
								summaryAttachmentDatas.Add(new MediumSizeAttachmentData()
								{
									Url = GetStationAttachmentUrl(coverAttachmentID, ImageMeta.Origin),
									MetaData = new MediumSizeMetaData()
									{
										SmallPreviews = new ThumbnailData[]{
											new ThumbnailData() 
											{
												Url = GetStationAttachmentUrl(coverAttachmentID, ImageMeta.Small)
											}
										}
									}
								});
							}
						}

						foreach (var attachmentID in attachmentIDs)
						{
							if (summaryAttachmentDatas.Count >= summaryAttachmentLimit)
								break;

							if (attachmentID == coverAttachmentID)
								continue;

							var attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachmentID));

							if (attachment != null)
							{
								var attachmentData = Mapper.Map<Attachment, MediumSizeAttachmentData>(attachment);
								summaryAttachmentDatas.Add(attachmentData);
							}
							else
							{
								summaryAttachmentDatas.Add(new MediumSizeAttachmentData()
								{
									Url = GetStationAttachmentUrl(attachmentID, ImageMeta.Origin),
									MetaData = new MediumSizeMetaData()
									{
										SmallPreviews = new ThumbnailData[]{
											new ThumbnailData() 
											{
												Url = GetStationAttachmentUrl(coverAttachmentID, ImageMeta.Small)
											}
										}
									}
								});
							}
						}

						if (summaryAttachmentDatas.Count > 0)
							postData.SummaryAttachments = summaryAttachmentDatas;

						++idx;
					}
				}
			}

			return new Dictionary<string, Object>() 
			{
				{"posts", postDatas},
				{"page_no", pageNo},
				{"page_size", pageSize},
				{"page_count", pageCount},
			    {"total_count", totalCount}
			};
		}
		#endregion
	}
}

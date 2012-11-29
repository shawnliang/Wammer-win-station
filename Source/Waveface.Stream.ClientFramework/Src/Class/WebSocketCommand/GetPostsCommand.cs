using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using System.Windows.Forms;
using Waveface.Stream.Model;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using MongoDB.Bson;
using AutoMapper;
using System.Text.RegularExpressions;

namespace Waveface.Stream.ClientFramework
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
		/// <value>The name.</value>
		public override string Name
		{
			get { return "getPosts"; }
		}
		#endregion


        #region Public Method
        /// <summary>
        /// Executes the specified parameters.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override Dictionary<string, Object> Execute(WebSocketCommandData data)
		{
            var parameters = data.Parameters;

            var sessionToken = StreamClient.Instance.LoginedUser.SessionToken;
            var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

			if (loginedSession == null)
				return null;

			var userID = loginedSession.user.user_id;

            var sinceDate = parameters.ContainsKey("since_date") ? DateTime.Parse(parameters["since_date"].ToString()) : default(DateTime?);
            var untilDate = parameters.ContainsKey("until_date") ? DateTime.Parse(parameters["until_date"].ToString()) : default(DateTime?);
            var pageNo = parameters.ContainsKey("page_no") ? int.Parse(parameters["page_no"].ToString()) : 1;
            var pageSize = parameters.ContainsKey("page_size") ? int.Parse(parameters["page_size"].ToString()) : 10;
			var skipCount = (pageNo == 1) ? 0 : (pageNo - 1) * pageSize;
			var queryParam = Query.And(Query.EQ("creator_id", userID), Query.EQ("hidden", "false"));

            if (parameters.ContainsKey("post_id_array"))
            {
                var postIDs = from postID in (parameters["post_id_array"] as JArray).Values()
                              select postID.ToString();
                queryParam = Query.And(queryParam, Query.In("_id", new BsonArray(postIDs)));
            }


            var type = parameters.ContainsKey("type") ? int.Parse(parameters["type"].ToString()) : 0;

            if ((type & 1) == 1)
            {
                queryParam = Query.And(queryParam, Query.EQ("type", "text"));
            }

            if ((type & 2) == 2)
            {
                queryParam = Query.And(queryParam, Query.EQ("type", "image"));
            }

            if ((type & 4) == 4)
            {
                queryParam = Query.And(queryParam, Query.EQ("type", "link"));
            }

            queryParam = Query.And(queryParam, Query.EQ("code_name", "StreamEvent"));
         

            if (sinceDate != null)
                queryParam = Query.And(queryParam, Query.GTE("event_time", sinceDate.Value.ToUTCISO8601ShortString()));

            if (untilDate != null)
                queryParam = Query.And(queryParam, Query.LT("event_time", untilDate.Value.ToUTCISO8601ShortString()));


			var filteredPosts = PostCollection.Instance.Find(queryParam)
				.SetSkip(skipCount)
				.SetLimit(pageSize)
                .SetSortOrder(SortBy.Descending("event_time"));


			var totalCount = filteredPosts.Count();
            var pageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);


            var dataSize = parameters.ContainsKey("data_size") ? int.Parse(parameters["data_size"].ToString()) : 1;

            Object postDatas;
            if (dataSize == 0)
            {
                postDatas = Mapper.Map<IEnumerable<PostInfo>, IEnumerable<SmallSizePostData>>(filteredPosts);
            }
            else
            {
                postDatas = Mapper.Map<IEnumerable<PostInfo>, IEnumerable<MediumSizePostData>>(filteredPosts).ToArray();

                var summaryAttachmentLimit = parameters.ContainsKey("sumary_limit") ? int.Parse(parameters["sumary_limit"].ToString()) : 1;

                var idx = 0;
                foreach (var postData in (postDatas as IEnumerable<MediumSizePostData>))
                {
                    if (summaryAttachmentLimit > 0)
                    {
                        var attachmentIDs = postData.AttachmentIDs;
                        var summaryAttachmentDatas = new List<MediumSizeAttachmentData>(summaryAttachmentLimit);

                        var coverAttachmentID = postData.CoverAttachmentID;
                        var coverAttachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", coverAttachmentID));
                        var coverAttachmentData = Mapper.Map<Attachment, MediumSizeAttachmentData>(coverAttachment);

                        summaryAttachmentDatas.Add(coverAttachmentData);

                        foreach (var attachmentID in attachmentIDs)
                        {
                            if (summaryAttachmentDatas.Count >= summaryAttachmentLimit)
                                break;

                            if (attachmentID == coverAttachmentID)
                                continue;

                            var attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachmentID));
                            var attachmentData = Mapper.Map<Attachment, MediumSizeAttachmentData>(attachment);
                            summaryAttachmentDatas.Add(attachmentData);
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

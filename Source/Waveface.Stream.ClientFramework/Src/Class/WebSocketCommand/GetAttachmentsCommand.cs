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

namespace Waveface.Stream.ClientFramework
{
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


        #region Constructor
        public GetAttachmentsCommand()
        {
            Mapper.CreateMap<Attachment, AttachmentData>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.object_id))
                .ForMember(dest => dest.timestamp, opt => opt.MapFrom(src => src.event_time.HasValue ? TimeHelper.ToCloudTimeString(src.event_time.Value) : null))
                .ForMember(dest => dest.url, opt => opt.MapFrom(src => GetAttachmentFilePath(src.url, src.saved_file_name)));

            Mapper.CreateMap<ImageProperty, ImageMetaData>();

            Mapper.CreateMap<ThumbnailInfo, ThumbnailData>()
                .ForMember(dest => dest.url, opt => opt.MapFrom(src => GetAttachmentFilePath(src.url, src.saved_file_name)));

        }
        #endregion


        #region Private Method
        private string GetAttachmentFilePath(string url, string savedFileName)
        {
            if (string.IsNullOrEmpty(savedFileName))
                return null;

            var loginedSession = LoginedSessionCollection.Instance.FindOne();

            if (loginedSession == null)
                return null;

            var groupID = loginedSession.groups.FirstOrDefault().group_id;

            Driver user = DriverCollection.Instance.FindDriverByGroupId(groupID);
            if (user == null)
                return null;

            var imageMetaType = ImageMeta.None;

            if (url.Contains("small"))
                imageMetaType = ImageMeta.Small;
            else if (url.Contains("medium"))
                imageMetaType = ImageMeta.Medium;
            else if (url.Contains("large"))
                imageMetaType = ImageMeta.Large;
            else
                imageMetaType = ImageMeta.Origin;

            var fileStorage = new FileStorage(user);
            return (new Uri(fileStorage.GetFullFilePath(savedFileName, imageMetaType))).ToString();
        }
        #endregion


        #region Public Method
        /// <summary>
		/// Executes the specified parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
        public override Dictionary<string, Object> Execute(Dictionary<string, Object> parameters = null)
		{
			var loginedSession = LoginedSessionCollection.Instance.FindOne();

			if (loginedSession == null)
				return null;

			var userID = loginedSession.user.user_id;
            var groupID = loginedSession.groups.FirstOrDefault().group_id;

			var sinceDate = parameters.ContainsKey("since_date") ? TimeHelper.ISO8601ToDateTime(parameters["since_date"].ToString()) : default(DateTime?);
			var untilDate = parameters.ContainsKey("until_date") ? TimeHelper.ISO8601ToDateTime(parameters["until_date"].ToString()) : default(DateTime?);
			var pageNo = parameters.ContainsKey("page_no") ? int.Parse(parameters["page_no"].ToString()) : 1;
			var pageSize = parameters.ContainsKey("page_size") ? int.Parse(parameters["page_size"].ToString()) : 10;
			var skipCount = (pageNo == 1) ? 0 : (pageNo - 1) * pageSize;

            var queryParam = Query.EQ("group_id", groupID);

            
            if (parameters.ContainsKey("post_id_array"))
            {
                var attachmentIDs = from postID in JArray.FromObject(parameters["post_id_array"]).Values()
                                    let post = PostCollection.Instance.FindOne(Query.EQ("_id", postID.ToString()))
                                    where post != null
                                    from attachmentID in post.attachment_id_array
                                    select attachmentID;
                queryParam = Query.And(queryParam, Query.In("_id", new BsonArray(attachmentIDs)));
            }

            if (parameters.ContainsKey("attachment_id_array"))
            {
                var attachmentIDs = from attachmentID in JArray.FromObject(parameters["attachment_id_array"]).Values()
                                    select attachmentID;
                queryParam = Query.And(queryParam, Query.In("_id", new BsonArray(attachmentIDs)));
            }

            var type = parameters.ContainsKey("type") ? int.Parse(parameters["type"].ToString()) : 0;

            if ((type & 1) == 1)
            {
                queryParam = Query.And(queryParam, Query.In("mime_type", new BsonArray(new string[] { "image/png", "image/jpeg" })));
            }

			if (sinceDate != null)
                queryParam = Query.And(queryParam, Query.GTE("event_time", sinceDate));

			if (untilDate != null)
                queryParam = Query.And(queryParam, Query.GTE("event_time", untilDate));

			var filteredAttachments = AttachmentCollection.Instance.Find(queryParam)
				.SetSkip(skipCount)
				.SetLimit(pageSize)
                .SetSortOrder(SortBy.Descending("event_time"));


			var totalCount = filteredAttachments.Count();
			var pageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);

         
            var attachmentDatas = Mapper.Map<IEnumerable<Attachment>, IEnumerable<AttachmentData>>(filteredAttachments);

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

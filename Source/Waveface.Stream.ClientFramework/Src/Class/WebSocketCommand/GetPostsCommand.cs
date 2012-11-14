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


        #region Constructor
        public GetPostsCommand()
        {
            Mapper.CreateMap<PostInfo, PostData>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.post_id))
                .ForMember(dest => dest.timestamp, opt => opt.MapFrom(src => src.event_time));

            Mapper.CreateMap<AttachmentInfo, AttachmentData>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.object_id))
                .ForMember(dest => dest.url, opt => opt.MapFrom(src => GetAttachmentFilePath(src.object_id, ImageMeta.Origin)));

            Mapper.CreateMap<AttachmentInfo.ImageMeta, ImageMetaData>()
                .ForMember(dest => dest.width, opt => opt.MapFrom(src => 
                    {
                        var attachment = GetOriginalAttachment(src);
                        return (attachment == null || attachment.image_meta == null) ? 0 : attachment.image_meta.width;
                    }))
                .ForMember(dest => dest.height, opt => opt.MapFrom(src => 
                    {
                        var attachment = GetOriginalAttachment(src);
                        return (attachment == null || attachment.image_meta == null) ? 0 : attachment.image_meta.height;
                    }));

            Mapper.CreateMap<AttachmentInfo.ImageMetaDetail, ThumbnailData>()
                .ForMember(dest => dest.url, opt => opt.MapFrom(src => GetAttachmentFilePath(src.url)));

        }
        #endregion


        #region Private Method
        private Attachment GetOriginalAttachment(AttachmentInfo.ImageMeta meta)
        {
            AttachmentInfo.ImageMetaDetail imageMeta = null;

            if (meta.small != null)
                imageMeta = meta.small;
            else if (meta.medium != null)
                imageMeta = meta.medium;
            else if (meta.large != null)
                imageMeta = meta.large;

            if (imageMeta == null)
                return null;


            var m = Regex.Match(imageMeta.url, OBJCCT_MATCH_PATTERN);

            if (!m.Success)
                return null;

            var attachmentID = m.Groups[OBJECT_GROUP_ID].Value;

            return AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachmentID));
        }

        private string GetAttachmentFilePath(string url)
        {
            if (url == null)
                return null;

            var m = Regex.Match(url, OBJCCT_MATCH_PATTERN);

            if (!m.Success)
                return null;

            var attachmentID = m.Groups[OBJECT_GROUP_ID].Value;

            var imageMetaType = ImageMeta.None;

            if (url.Contains("small"))
                imageMetaType = ImageMeta.Small;
            else if (url.Contains("medium"))
                imageMetaType = ImageMeta.Medium;
            else if (url.Contains("large"))
                imageMetaType = ImageMeta.Large;
            else
                imageMetaType = ImageMeta.Origin;

            return GetAttachmentFilePath(attachmentID, imageMetaType);
        }

        private string GetAttachmentFilePath(string attachmentID,ImageMeta meta)
        {
            if (meta == ImageMeta.None)
                return null;

            var attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachmentID));

            if (attachment == null)
                return null;

            var savedFileName = string.Empty;

            switch (meta)
            {
                case ImageMeta.Small:
                    savedFileName = attachment.image_meta.small.saved_file_name;
                    break;
                case ImageMeta.Medium:
                    savedFileName = attachment.image_meta.medium.saved_file_name;
                    break;
                case ImageMeta.Large:
                    savedFileName = attachment.image_meta.large.saved_file_name;
                    break;
                case ImageMeta.Origin:
                    savedFileName = attachment.saved_file_name;
                    break;
            }

            if (string.IsNullOrEmpty(savedFileName))
                return null;

            var loginedSession = LoginedSessionCollection.Instance.FindOne();

            if (loginedSession == null)
                return null;

            var groupID = loginedSession.groups.FirstOrDefault().group_id;

            Driver user = DriverCollection.Instance.FindDriverByGroupId(groupID);
            if (user == null)
                return null;

            var fileStorage = new FileStorage(user);
            return (new Uri(fileStorage.GetFullFilePath(savedFileName, meta))).ToString();
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

			var sinceDate = parameters.ContainsKey("since_date") ? TimeHelper.ISO8601ToDateTime(parameters["since_date"].ToString()) : default(DateTime?);
			var untilDate = parameters.ContainsKey("until_date") ? TimeHelper.ISO8601ToDateTime(parameters["until_date"].ToString()) : default(DateTime?);
            var pageNo = parameters.ContainsKey("page_no") ? int.Parse(parameters["page_no"].ToString()) : 1;
            var pageSize = parameters.ContainsKey("page_size") ? int.Parse(parameters["page_size"].ToString()) : 10;
			var skipCount = (pageNo == 1) ? 0 : (pageNo - 1) * pageSize;
			var queryParam = Query.And(Query.EQ("creator_id", userID), Query.EQ("hidden", "false"));

            if (parameters.ContainsKey("post_id_array"))
            {
                var postIDs = from postID in JArray.FromObject(parameters["post_id_array"]).Values()
                              select postID;
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


            if (sinceDate != null)
                queryParam = Query.And(queryParam, Query.GTE("event_time", sinceDate));

            if (untilDate != null)
                queryParam = Query.And(queryParam, Query.GTE("event_time", untilDate));


			var filteredPosts = PostCollection.Instance.Find(queryParam)
				.SetSkip(skipCount)
				.SetLimit(pageSize)
                .SetSortOrder(SortBy.Descending("event_time"));


			var totalCount = filteredPosts.Count();
            var pageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);

           
            var postDatas = Mapper.Map<IEnumerable<PostInfo>, IEnumerable<PostData>>(filteredPosts).ToArray();

            var summaryAttachmentLimit = parameters.ContainsKey("sumary_limit") ? int.Parse(parameters["sumary_limit"].ToString()) : 1;

            var idx = 0;
            foreach (var post in filteredPosts)
            {
                if (summaryAttachmentLimit > 0)
                {
                    var attachments =  post.attachments;
                    var summaryAttachmentDatas = new List<AttachmentData>(summaryAttachmentLimit);
                    var coverAttachment = (string.IsNullOrEmpty(post.cover_attach)) ?attachments.FirstOrDefault() : attachments.Where((item) => item.object_id == post.cover_attach).FirstOrDefault();
                    var coverAttachmentData = Mapper.Map<AttachmentInfo, AttachmentData>(coverAttachment);

                    summaryAttachmentDatas.Add(coverAttachmentData);

                    foreach (var attachment in attachments)
                    {
                        if (summaryAttachmentDatas.Count >= summaryAttachmentLimit)
                            break;

                        if (attachment == coverAttachment)
                            continue;

                        var attachmentData = Mapper.Map<AttachmentInfo, AttachmentData>(attachment);
                        summaryAttachmentDatas.Add(attachmentData);
                    }

                    if (summaryAttachmentDatas.Count > 0)
                        postDatas[idx].summary_attachments = summaryAttachmentDatas;

                    ++idx;
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

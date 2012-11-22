using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Stream.Model;

namespace Waveface.Stream.ClientFramework
{
    /// <summary>
    /// 
    /// </summary>
    public static class AutoMapperSetting
    {
        #region Private Static Method
        /// <summary>
        /// Gets the attachment file path.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="savedFileName">Name of the saved file.</param>
        /// <returns></returns>
        private static string GetAttachmentFilePath(string url, string savedFileName)
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


        #region Public Static Method
        public static void IniteMap()
        {
            Mapper.CreateMap<PostInfo, PostData>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.post_id))
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.event_time))
                .ForMember(dest => dest.CodeName, opt => opt.MapFrom(src => src.code_name))
                .ForMember(dest => dest.AttachmentCount, opt => opt.MapFrom(src => src.attachment_count))
                .ForMember(dest => dest.CoverAttachmentID, opt => opt.MapFrom(src => src.cover_attach))
                .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.comment_count))                                      
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.tags))
                .ForMember(dest => dest.AttachmentIDs, opt => opt.MapFrom(src => src.attachment_id_array))
                .ForMember(dest => dest.ExtraParams, opt => opt.MapFrom(src => src.extra_parameters));

            Mapper.CreateMap<PostGps, PostGpsData>();

            Mapper.CreateMap<Person, PeopleData>();

            Mapper.CreateMap<ExtraParameter, PostExtraData>();

            Mapper.CreateMap<Attachment, AttachmentData>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.object_id))
                .ForMember(dest => dest.timestamp, opt => opt.MapFrom(src => src.event_time.HasValue ? src.event_time.Value.ToUTCISO8601ShortString() : null))
                .ForMember(dest => dest.url, opt => opt.MapFrom(src => GetAttachmentFilePath(src.url, src.saved_file_name)));

            Mapper.CreateMap<ImageProperty, ImageMetaData>();

            Mapper.CreateMap<ThumbnailInfo, ThumbnailData>()
                .ForMember(dest => dest.url, opt => opt.MapFrom(src => GetAttachmentFilePath(src.url, src.saved_file_name)));
        }
        #endregion
    }
}

using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Waveface.Stream.Model;

namespace Waveface.Stream.Core
{
	/// <summary>
	/// 
	/// </summary>
	public static class AutoMapperSetting
	{
		#region Private Const
		private const string USER_ID_GROUP_KEY = @"USER_ID";
		private const string USER_ID_MATCH_PATTERN = @"cache\\(?<" + USER_ID_GROUP_KEY + @">[^\\]+)";
		#endregion

		#region Private Static Method
		/// <summary>
		/// Gets the attachment file path.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="groupID">The group ID.</param>
		/// <param name="savedFileName">Name of the saved file.</param>
		/// <returns></returns>
		private static string GetAttachmentFilePath(string url, string groupID, string savedFileName)
		{
			if (string.IsNullOrEmpty(savedFileName))
				return null;

			if (String.IsNullOrEmpty(url))
				return null;

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

		private static string GetAttachmentFilePath(string url, string savedFileName)
		{
			if (string.IsNullOrEmpty(savedFileName))
				return null;

			if (String.IsNullOrEmpty(url))
				return null;

			var userID = Regex.Match(savedFileName, USER_ID_MATCH_PATTERN).Groups[USER_ID_GROUP_KEY].Value;

			Driver user = DriverCollection.Instance.FindOneById(userID);
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

		/// <summary>
		/// Gets the attachment file path.
		/// </summary>
		/// <param name="savedFileName">Name of the saved file.</param>
		/// <returns></returns>
		private static string GetStationFilePath(string savedFileName)
		{
			if (string.IsNullOrEmpty(savedFileName))
				return null;

			var userID = Regex.Match(savedFileName, USER_ID_MATCH_PATTERN).Groups[USER_ID_GROUP_KEY].Value;

			Driver user = DriverCollection.Instance.FindOneById(userID);
			if (user == null)
				return null;

			var fileStorage = new FileStorage(user);
			return (new Uri(fileStorage.GetStationFilePath(savedFileName))).ToString();
		}

		/// <summary>
		/// Gets the resource file path.
		/// </summary>
		/// <param name="savedFileName">Name of the saved file.</param>
		/// <returns></returns>
		private static string GetResourceFilePath(string groupID, string savedFileName)
		{
			if (string.IsNullOrEmpty(savedFileName))
				return null;

			Driver user = DriverCollection.Instance.FindDriverByGroupId(groupID);
			if (user == null)
				return null;

			var fileStorage = new FileStorage(user);
			return (new Uri(fileStorage.GetResourceFilePath(savedFileName))).ToString();
		}

		private static int GetNewClientAttachmentType(AttachmentType type)
		{
			switch (type)
			{
				case AttachmentType.image:
					return 1;
				case AttachmentType.doc:
					return 8;
				default:
					break;
			}
			return 0;
		}

		private static String GetAttachmentTimeStamp(Attachment attachment)
		{
			if (attachment == null)
				return null;

			if (attachment.type == AttachmentType.doc)
			{
				var lastAccessTime = attachment.doc_meta.access_time.Last();

				if (lastAccessTime != null)
					return lastAccessTime.ToUTCISO8601ShortString();
			}

			return attachment.event_time.HasValue ? attachment.event_time.Value.ToUTCISO8601ShortString() : null;
		}

		private static IEnumerable<PostCheckInData> GetCheckInDatas(IEnumerable<string> checkinIDs)
		{
			if (checkinIDs == null || !checkinIDs.Any())
				return null;

			var locations = LocationDBDataCollection.Instance.Find(Query.In("_id", new BsonArray(checkinIDs)));

			if (locations == null || !locations.Any())
				return null;

			var checkInDatas = Mapper.Map<IEnumerable<LocationDBData>, IEnumerable<PostCheckInData>>(locations);

			return checkInDatas;
		}

		private static PostGpsData GetPostGPSDatas(string locationID)
		{
			if (string.IsNullOrEmpty(locationID))
				return null;

			var location = LocationDBDataCollection.Instance.FindOneById(locationID);

			if (location == null)
				return null;

			var postGPSData = Mapper.Map<LocationDBData, PostGpsData>(location);

			return postGPSData;
		}

		private static IEnumerable<FriendData> GetFriends(IEnumerable<string> friendIDs)
		{
			if (friendIDs == null || !friendIDs.Any())
				return null;

			var friends = FriendDBDataCollection.Instance.Find(Query.In("_id", new BsonArray(friendIDs)));

			if (friends == null || !friends.Any())
				return null;

			var friendDatas = Mapper.Map<IEnumerable<FriendDBData>, IEnumerable<FriendData>>(friends);

			return friendDatas;
		}
		#endregion


		#region Public Static Method
		/// <summary>
		/// Inites the map.
		/// </summary>
		public static void IniteMap()
		{
			Mapper.CreateMap<LocationDBData, PostCheckInData>();
			Mapper.CreateMap<LocationDBData, PostGpsData>();

			Mapper.CreateMap<PostDBData, SmallSizePostData>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.EventSinceTime));

			Mapper.CreateMap<PostDBData, MediumSizePostData>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.EventSinceTime))
				.ForMember(dest => dest.CoverAttachmentID, opt => opt.MapFrom(src => src.CoverAttachmentID))
				.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
				.ForMember(dest => dest.AttachmentIDs, opt => opt.MapFrom(src => src.AttachmentIDs))
				.ForMember(dest => dest.CheckIns, opt => opt.MapFrom(src => GetCheckInDatas(src.CheckinIDs)))
				.ForMember(dest => dest.Location, opt => opt.MapFrom(src => (src.CheckinIDs == null)? null: GetPostGPSDatas(src.CheckinIDs.FirstOrDefault())))
				.ForMember(dest => dest.Friends, opt => opt.MapFrom(src => GetFriends(src.FriendIDs)))
				.ForMember(dest => dest.ExtraParams, opt => opt.MapFrom(src => src.ExtraParams));

			Mapper.CreateMap<PostCheckIn, PostCheckInData>();

			Mapper.CreateMap<PostGps, PostGpsData>()
				.ForMember(dest => dest.ZoomLevel, opt => opt.MapFrom(src => src.zoom_level));

			Mapper.CreateMap<FriendDBData, FriendData>();

			Mapper.CreateMap<ExtraParameter, PostExtraData>();

			Mapper.CreateMap<Attachment, MediumSizeAttachmentData>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.object_id))
				.ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.file_name))
				.ForMember(dest => dest.Type, opt => opt.MapFrom(src => GetNewClientAttachmentType(src.type)))
				.ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => GetAttachmentTimeStamp(src)))
				.ForMember(dest => dest.Url, opt => opt.MapFrom(src => (src.type == AttachmentType.doc) ? GetResourceFilePath(src.group_id, src.saved_file_name) : GetAttachmentFilePath(src.url, src.group_id, src.saved_file_name)))
				.ForMember(dest => dest.MetaData, opt => opt.MapFrom(src => (src.type == AttachmentType.doc) ? (object)src.doc_meta : (object)src.image_meta));

			Mapper.CreateMap<Attachment, LargeSizeAttachmentData>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.object_id))
				.ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.file_name))
				.ForMember(dest => dest.Type, opt => opt.MapFrom(src => GetNewClientAttachmentType(src.type)))
				.ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => GetAttachmentTimeStamp(src)))
				.ForMember(dest => dest.Url, opt => opt.MapFrom(src => (src.type == AttachmentType.doc) ? GetResourceFilePath(src.group_id, src.saved_file_name) : GetAttachmentFilePath(src.url, src.group_id, src.saved_file_name)))
				.ForMember(dest => dest.MetaData, opt => opt.MapFrom(src => (src.type == AttachmentType.doc) ? (object)src.doc_meta : (object)src.image_meta));


			Mapper.CreateMap<ImageProperty, MediumSizeMetaData>();

			Mapper.CreateMap<ImageProperty, LargeSizeMetaData>();

			Mapper.CreateMap<DocProperty, MediumSizeMetaData>()
				.ForMember(dest => dest.AccessTimes, opt => opt.MapFrom(src => src.access_time))
				.ForMember(dest => dest.PageCount, opt => opt.MapFrom(src => src.preview_pages))
				.ForMember(dest => dest.PreviewFiles, opt => opt.MapFrom(src => (src.preview_files == null) ? null : src.preview_files.Select(file => GetStationFilePath(file))));


			Mapper.CreateMap<DocProperty, LargeSizeMetaData>()
				.ForMember(dest => dest.AccessTimes, opt => opt.MapFrom(src => src.access_time))
				.ForMember(dest => dest.PageCount, opt => opt.MapFrom(src => src.preview_pages))
				.ForMember(dest => dest.PreviewFiles, opt => opt.MapFrom(src => (src.preview_files == null) ? null : src.preview_files.Select(file => GetStationFilePath(file))));


			Mapper.CreateMap<ThumbnailInfo, ThumbnailData>()
				.ForMember(dest => dest.Url, opt => opt.MapFrom(src => GetAttachmentFilePath(src.url, src.saved_file_name)));

			Mapper.CreateMap<exif, ExifData>();

			Mapper.CreateMap<GPSInfo, GPSInfoData>();

			Mapper.CreateMap<Gps, AttachmentGPSData>();

			Mapper.CreateMap<LoginedSession, UserData>()
				.ForMember(dest => dest.SessionToken, opt => opt.MapFrom(src => src.session_token))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.user.email))
				.ForMember(dest => dest.NickName, opt => opt.MapFrom(src => src.user.nickname))
				.ForMember(dest => dest.Devices, opt => opt.MapFrom(src => src.user.devices));

			Mapper.CreateMap<Device, DeviceData>();

			Mapper.CreateMap<Collection, SmallSizeCollcetionData>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.collection_id))
				.ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.modify_time));

			Mapper.CreateMap<Collection, MediumSizeCollcetionData>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.collection_id))
				.ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.create_time))
				.ForMember(dest => dest.AttachmentIDs, opt => opt.MapFrom(src => src.attachment_id_array))
				.ForMember(dest => dest.CoverAttachmentID, opt => opt.MapFrom(src => src.cover));
		}
		#endregion
	}
}

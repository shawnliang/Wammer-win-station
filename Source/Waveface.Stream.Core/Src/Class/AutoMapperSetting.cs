using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.IO;
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

			var userID = location.CreatorID;
			var cacheDir = Path.Combine(Path.Combine(Environment.CurrentDirectory, "cache"), string.Format(@"{0}\Map", userID));

			var mapFile = Path.Combine(cacheDir, string.Format("{0}.jpg", locationID));

			if (File.Exists(mapFile))
				postGPSData.Map = mapFile;

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

		private static string GetStationAttachmentUrl(string url)
		{
			if (string.IsNullOrEmpty(url))
				return null;

			var loginedUser = LoginedSessionCollection.Instance.FindOne();
			return string.Format(@"http://127.0.0.1:9981{0}&apikey={1}&session_token={2}", url, StationAPI.API_KEY, loginedUser.session_token);
		}

		private static string GetAttachmentUrl(Attachment attachment)
		{
			switch (attachment.type)
			{
				case AttachmentType.image:
					return GetStationAttachmentUrl(attachment.url);
				case AttachmentType.doc:
					return GetResourceFilePath(attachment.group_id, attachment.saved_file_name);
				case AttachmentType.webthumb:
					return attachment.web_meta.url;
			}
			return null;
		}

		private static object GetAttachmentMetaData(Attachment attachment)
		{
			switch (attachment.type)
			{
				case AttachmentType.image:
					return (object)attachment.image_meta;
				case AttachmentType.doc:
					return (object)attachment.doc_meta;
				case AttachmentType.webthumb:
					return (object)attachment.web_meta;
			}
			return null;
		}
		private static string GetStationAttachmentUrl(string attachmentID, ImageMeta imageMeta)
		{
			var loginedUser = LoginedSessionCollection.Instance.FindOne();
			return string.Format(@"http://127.0.0.1:9981/v3/attachments/view/?apikey={0}&session_token={1}&object_id={2}&image_meta={3}",
				StationAPI.API_KEY,
				loginedUser.session_token,
				attachmentID,
				imageMeta);
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
				.ForMember(dest => dest.Location, opt => opt.MapFrom(src => GetPostGPSDatas(src.LocationID)))
				.ForMember(dest => dest.Friends, opt => opt.MapFrom(src => GetFriends(src.FriendIDs)))
				.ForMember(dest => dest.ExtraParams, opt => opt.MapFrom(src => src.ExtraParams));

			Mapper.CreateMap<PostCheckIn, PostCheckInData>();

			Mapper.CreateMap<PostGps, PostGpsData>()
				.ForMember(dest => dest.ZoomLevel, opt => opt.MapFrom(src => src.zoom_level));

			Mapper.CreateMap<FriendDBData, FriendData>();

			Mapper.CreateMap<ExtraParameter, PostExtraData>();

			Mapper.CreateMap<Attachment, MediumSizeAttachmentData>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.object_id))
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => (src.type != AttachmentType.webthumb) ? null : src.web_meta.title))
				.ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.file_name))
				.ForMember(dest => dest.Type, opt => opt.MapFrom(src => GetNewClientAttachmentType(src.type)))
				.ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => GetAttachmentTimeStamp(src)))
				.ForMember(dest => dest.Url, opt => opt.MapFrom(src => GetAttachmentUrl(src)))
				.ForMember(dest => dest.MetaData, opt => opt.MapFrom(src => GetAttachmentMetaData(src)))
				.AfterMap((src,dest)=> 
				{
					if(src.type == AttachmentType.webthumb)
					{
						dest.MetaData.MediumPreviews = new ThumbnailData[]
						{
							new ThumbnailData() 
							{
								Width = src.image_meta.width,
								Height = src.image_meta.height,
								Url = GetStationAttachmentUrl(src.url)
							}
						};
					}
				});

			Mapper.CreateMap<Attachment, LargeSizeAttachmentData>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.object_id))
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => (src.type != AttachmentType.webthumb)? null: src.web_meta.title))
				.ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.file_name))
				.ForMember(dest => dest.Type, opt => opt.MapFrom(src => GetNewClientAttachmentType(src.type)))
				.ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => GetAttachmentTimeStamp(src)))
				.ForMember(dest => dest.Url, opt => opt.MapFrom(src => GetAttachmentUrl(src)))
				.ForMember(dest => dest.MetaData, opt => opt.MapFrom(src => GetAttachmentMetaData(src)))
				.AfterMap((src, dest) =>
				{
					if (src.type == AttachmentType.webthumb)
					{
						dest.MetaData.MediumPreviews = new ThumbnailData[]
						{
							new ThumbnailData() 
							{
								Width = src.image_meta.width,
								Height = src.image_meta.height,
								Url = GetStationAttachmentUrl(src.url)
							}
						};
					}
				});


			Mapper.CreateMap<ImageProperty, MediumSizeMetaData>()
				.ForMember(dest => dest.SmallPreviews, opt => opt.MapFrom(src => (src.small == null)? null: new ThumbnailInfo[] { src.small }))
				.ForMember(dest => dest.MediumPreviews, opt => opt.MapFrom(src => (src.medium == null) ? null : new ThumbnailInfo[] { src.medium }))
				.ForMember(dest => dest.LargePreviews, opt => opt.MapFrom(src => (src.large == null) ? null : new ThumbnailInfo[] { src.large }));


			Mapper.CreateMap<ImageProperty, LargeSizeMetaData>()
				.ForMember(dest => dest.SmallPreviews, opt => opt.MapFrom(src => (src.small == null)? null: new ThumbnailInfo[] { src.small }))
				.ForMember(dest => dest.MediumPreviews, opt => opt.MapFrom(src => (src.medium == null) ? null : new ThumbnailInfo[] { src.medium }))
				.ForMember(dest => dest.LargePreviews, opt => opt.MapFrom(src => (src.large == null) ? null : new ThumbnailInfo[] { src.large }));

			Mapper.CreateMap<DocProperty, MediumSizeMetaData>()
				.ForMember(dest => dest.AccessTimes, opt => opt.MapFrom(src => src.access_time))
				.ForMember(dest => dest.PageCount, opt => opt.MapFrom(src => src.preview_pages))
				.ForMember(dest => dest.MediumPreviews, opt => opt.MapFrom(src => (src.preview_files == null) ? null : src.preview_files.Select(file => 
					new ThumbnailData()
					{
						Url = GetStationFilePath(file)
					})));


			Mapper.CreateMap<DocProperty, LargeSizeMetaData>()
				.ForMember(dest => dest.AccessTimes, opt => opt.MapFrom(src => src.access_time))
				.ForMember(dest => dest.PageCount, opt => opt.MapFrom(src => src.preview_pages))
				.ForMember(dest => dest.MediumPreviews, opt => opt.MapFrom(src => (src.preview_files == null) ? null : src.preview_files.Select(file =>
					new ThumbnailData()
					{
						Url = GetStationFilePath(file)
					})));

			Mapper.CreateMap<WebProperty, MediumSizeMetaData>()
				.ForMember(dest => dest.AccessTimes, opt => opt.MapFrom(src => src.accesses.Select(item => item.time.ToUTCISO8601ShortString())))
				.ForMember(dest => dest.From, opt => opt.MapFrom(src => src.accesses.First().from))
				.ForMember(dest => dest.Favicon, opt => opt.MapFrom(src => src.favicon));

			Mapper.CreateMap<WebProperty, LargeSizeMetaData>()
				.ForMember(dest => dest.AccessTimes, opt => opt.MapFrom(src => src.accesses.Select(item => item.time.ToUTCISO8601ShortString())))
				.ForMember(dest => dest.From, opt => opt.MapFrom(src => src.accesses.First().from))
				.ForMember(dest => dest.Favicon, opt => opt.MapFrom(src => src.favicon));


			Mapper.CreateMap<ThumbnailInfo, ThumbnailData>()
				.ForMember(dest => dest.Url, opt => opt.MapFrom(src => GetStationAttachmentUrl(src.url)));

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

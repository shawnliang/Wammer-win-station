using AutoMapper;
using System.Linq;
using Waveface.Stream.Model;

namespace Waveface.Stream.Model
{
	/// <summary>
	/// 
	/// </summary>
	public static class AutoMapperSetting
	{
		#region Public Static Method
		/// <summary>
		/// Inites the map.
		/// </summary>
		public static void IniteMap()
		{
			Mapper.CreateMap<PostInfo, PostDBData>()
					.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.post_id))
					.ForMember(dest => dest.Type, opt => opt.MapFrom(src => (src.type.Equals("doc"))? PostType.Doc: PostType.Photo)) //TODO: 待處理
					.ForMember(dest => dest.CoverAttachmentID, opt => opt.MapFrom(src => src.cover_attach))
					.ForMember(dest => dest.AttachmentIDs, opt => opt.MapFrom(src => src.attachment_id_array))
					.ForMember(dest => dest.CreatorID, opt => opt.MapFrom(src => src.creator_id))
					.ForMember(dest => dest.Visibility, opt => opt.MapFrom(src => src.hidden.Equals("false", System.StringComparison.CurrentCultureIgnoreCase)))
					.ForMember(dest => dest.EventSinceTime, opt => opt.MapFrom(src => src.event_time))
					.ForMember(dest => dest.EventUntilTime, opt => opt.MapFrom(src => src.timestamp))
					.ForMember(dest => dest.ModifyTime, opt => opt.MapFrom(src => src.update_time));

			Mapper.CreateMap<FriendInfo, FriendDBData>();

			Mapper.CreateMap<PostCheckIn, LocationDBData>();

			Mapper.CreateMap<PostGps, LocationDBData>()
				.ForMember(dest => dest.ZoomLevel, opt => opt.MapFrom(src => src.zoom_level))
				.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.region_tags));
		}
		#endregion
	}
}

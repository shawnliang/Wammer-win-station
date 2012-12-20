using AutoMapper;
using System.Linq;
using Waveface.Stream.Model;

namespace Wammer.Station
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
			Mapper.CreateMap<Cloud.Collection, Collection>()
				.ForMember(dest => dest.attachment_id_array, opt => opt.MapFrom(src => src.object_list.Select(item => item.object_id)));
		}
		#endregion
	}
}

using Newtonsoft.Json;
using System;

namespace Waveface.Stream.ClientFramework
{
	public class PeopleData
	{
		#region Public Property
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public String Name { get; set; }

		[JsonProperty("avatar", NullValueHandling = NullValueHandling.Ignore)]
		public String Avatar { get; set; }
		#endregion

		#region Public Method
		/// <summary>
		/// Shoulds the serialize namel.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeName()
		{
			return !String.IsNullOrEmpty(Name);
		}

		/// <summary>
		/// Shoulds the serialize avatar.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeAvatar()
		{
			return !String.IsNullOrEmpty(Avatar);
		}
		#endregion
	}
}

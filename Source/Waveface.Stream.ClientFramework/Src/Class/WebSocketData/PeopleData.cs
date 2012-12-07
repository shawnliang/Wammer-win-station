using Newtonsoft.Json;
using System;

namespace Waveface.Stream.ClientFramework
{
	public class PeopleData
	{
		#region Public Property
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public String Name { get; set; }
		#endregion

		#region Public Method
		/// <summary>
		/// Shoulds the serialize namel.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeName()
		{
			return Name != null && Name.Length > 0;
		}
		#endregion
	}
}

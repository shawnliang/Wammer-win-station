﻿using Newtonsoft.Json;
using System;

namespace Waveface.Stream.ClientFramework
{
	public class PostCheckInData
	{
		#region Public Property
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public String Name { get; set; }

		[JsonProperty("latitude", NullValueHandling = NullValueHandling.Ignore)]
		public float? Latitude { get; set; }

		[JsonProperty("longitude", NullValueHandling = NullValueHandling.Ignore)]
		public float? Longitude { get; set; }
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
		#endregion
	}
}
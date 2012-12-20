using Newtonsoft.Json;
using System;

namespace Waveface.Stream.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class ThumbnailData
	{
		#region Public Property
		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
		public String Url { get; set; }

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>
		/// The width.
		/// </value>
		[JsonProperty("width")]
		public int Width { get; set; }

		/// <summary>
		/// Gets or sets the height.
		/// </summary>
		/// <value>
		/// The height.
		/// </value>
		[JsonProperty("height")]
		public int Height { get; set; }
		#endregion

		#region Public Method
		/// <summary>
		/// Shoulds the serialize URL.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeUrl()
		{
			return Url != null && Url.Length > 0;
		}

		/// <summary>
		/// Shoulds the width of the serialize.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeWidth()
		{
			return Width > 0;
		}

		/// <summary>
		/// Shoulds the height of the serialize.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeHeight()
		{
			return Height > 0;
		}
		#endregion
	}
}

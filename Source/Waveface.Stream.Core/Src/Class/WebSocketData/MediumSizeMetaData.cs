
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
namespace Waveface.Stream.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class MediumSizeMetaData
	{
		#region Public Property
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

		/// <summary>
		/// Gets or sets the small.
		/// </summary>
		/// <value>
		/// The small.
		/// </value>
		[JsonProperty("small_previews", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<ThumbnailData> SmallPreviews { get; set; }

		/// <summary>
		/// Gets or sets the medium.
		/// </summary>
		/// <value>
		/// The medium.
		/// </value>
		[JsonProperty("medium_previews", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<ThumbnailData> MediumPreviews { get; set; }

		/// <summary>
		/// Gets or sets the large.
		/// </summary>
		/// <value>
		/// The large.
		/// </value>
		[JsonProperty("large_previews", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<ThumbnailData> LargePreviews { get; set; }

		/// <summary>
		/// Gets or sets the favicon.
		/// </summary>
		/// <value>
		/// The favicon.
		/// </value>
		[JsonProperty("favicon")]
		public string Favicon { get; set; }

		/// <summary>
		/// Gets or sets from.
		/// </summary>
		/// <value>
		/// From.
		/// </value>
		[JsonProperty("from")]
		public string From { get; set; }

		/// <summary>
		/// Gets or sets the page count.
		/// </summary>
		/// <value>
		/// The page count.
		/// </value>
		[JsonProperty("page_count")]
		public int PageCount { get; set; }

		/// <summary>
		/// Gets or sets the access times.
		/// </summary>
		/// <value>
		/// The access times.
		/// </value>
		[JsonProperty("access_time", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<string> AccessTimes { get; set; }
		#endregion


		#region Public Method
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

		/// <summary>
		/// Shoulds the serialize access times.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeAccessTimes()
		{
			return AccessTimes != null && AccessTimes.Any();
		}
		#endregion
	}
}

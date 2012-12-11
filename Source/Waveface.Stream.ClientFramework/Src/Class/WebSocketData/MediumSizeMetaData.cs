
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
namespace Waveface.Stream.ClientFramework
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
		[JsonProperty("small", NullValueHandling = NullValueHandling.Ignore)]
		public ThumbnailData Small { get; set; }

		/// <summary>
		/// Gets or sets the medium.
		/// </summary>
		/// <value>
		/// The medium.
		/// </value>
		[JsonProperty("medium", NullValueHandling = NullValueHandling.Ignore)]
		public ThumbnailData Medium { get; set; }

		/// <summary>
		/// Gets or sets the large.
		/// </summary>
		/// <value>
		/// The large.
		/// </value>
		[JsonProperty("large", NullValueHandling = NullValueHandling.Ignore)]
		public ThumbnailData Large { get; set; }

		/// <summary>
		/// Gets or sets the preview files.
		/// </summary>
		/// <value>
		/// The preview files.
		/// </value>
		[JsonProperty("preview_files", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<string> PreviewFiles { get; set; }

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
		/// Shoulds the serialize preview files.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializePreviewFiles()
		{
			return PreviewFiles != null && PreviewFiles.Any();
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

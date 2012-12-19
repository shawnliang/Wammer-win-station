using Newtonsoft.Json;

namespace Waveface.Stream.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class CalendarEntry
	{
		#region Public Property
		[JsonProperty("since_date", NullValueHandling = NullValueHandling.Ignore)]
		public string SinceDate { get; set; }

		[JsonProperty("until_date", NullValueHandling = NullValueHandling.Ignore)]
		public string UntilDate { get; set; }

		[JsonProperty("post_count")]
		public int PostCount { get; set; }

		[JsonProperty("attachment_count")]
		public int AttachmentCount { get; set; }
		#endregion


		#region Public Method
		/// <summary>
		/// Shoulds the serialize since date.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeSinceDate()
		{
			return SinceDate != null && SinceDate.Length > 0;
		}

		/// <summary>
		/// Shoulds the serialize until date.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeUntilDate()
		{
			return UntilDate != null && UntilDate.Length > 0;
		}

		/// <summary>
		/// Shoulds the serialize post count.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializePostCount()
		{
			return PostCount > 0;
		}

		/// <summary>
		/// Shoulds the serialize attachment count.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeAttachmentCount()
		{
			return AttachmentCount > 0;
		}
		#endregion
	}
}

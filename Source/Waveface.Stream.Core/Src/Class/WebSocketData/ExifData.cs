using Newtonsoft.Json;
using System.Collections.Generic;

namespace Waveface.Stream.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class ExifData
	{
		#region Public Property
		[JsonProperty("y_resolution", NullValueHandling = NullValueHandling.Ignore)]
		public List<int> YResolution { get; set; }

		[JsonProperty("make", NullValueHandling = NullValueHandling.Ignore)]
		public string Make { get; set; }

		[JsonProperty("flash", NullValueHandling = NullValueHandling.Ignore)]
		public int? Flash { get; set; }

		[JsonProperty("datetime", NullValueHandling = NullValueHandling.Ignore)]
		public string DateTime { get; set; }

		[JsonProperty("metering_mode", NullValueHandling = NullValueHandling.Ignore)]
		public int? MeteringMode { get; set; }

		[JsonProperty("x_resolution", NullValueHandling = NullValueHandling.Ignore)]
		public List<int> XResolution { get; set; }

		[JsonProperty("exposure_program", NullValueHandling = NullValueHandling.Ignore)]
		public int? ExposureProgram { get; set; }

		[JsonProperty("color_space", NullValueHandling = NullValueHandling.Ignore)]
		public int? ColorSpace { get; set; }

		[JsonProperty("exif_image_width", NullValueHandling = NullValueHandling.Ignore)]
		public int? ExifImageWidth { get; set; }

		[JsonProperty("date_time_digitized", NullValueHandling = NullValueHandling.Ignore)]
		public string DateTimeDigitized { get; set; }

		[JsonProperty("date_time_original", NullValueHandling = NullValueHandling.Ignore)]
		public string DateTimeOriginal { get; set; }

		[JsonProperty("exposure_time", NullValueHandling = NullValueHandling.Ignore)]
		public List<int> ExposureTime { get; set; }

		[JsonProperty("sensing_method", NullValueHandling = NullValueHandling.Ignore)]
		public int? SensingMethod { get; set; }

		[JsonProperty("f_number", NullValueHandling = NullValueHandling.Ignore)]
		public List<int> FNumber { get; set; }

		[JsonProperty("aperture_value", NullValueHandling = NullValueHandling.Ignore)]
		public List<int> ApertureValue { get; set; }

		[JsonProperty("focal_length", NullValueHandling = NullValueHandling.Ignore)]
		public List<int> FocalLength { get; set; }

		[JsonProperty("white_balance", NullValueHandling = NullValueHandling.Ignore)]
		public int? WhiteBalance { get; set; }

		[JsonProperty("components_configuration", NullValueHandling = NullValueHandling.Ignore)]
		public string ComponentsConfiguration { get; set; }

		[JsonProperty("exif_offset", NullValueHandling = NullValueHandling.Ignore)]
		public int? ExifOffset { get; set; }

		[JsonProperty("exif_image_height", NullValueHandling = NullValueHandling.Ignore)]
		public int? ExifImageHeight { get; set; }

		[JsonProperty("iso_speed_ratings", NullValueHandling = NullValueHandling.Ignore)]
		public int? ISOSpeedRatings { get; set; }

		[JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
		public string Model { get; set; }

		[JsonProperty("software", NullValueHandling = NullValueHandling.Ignore)]
		public string Software { get; set; }

		[JsonProperty("flash_pix_version", NullValueHandling = NullValueHandling.Ignore)]
		public string FlashPixVersion { get; set; }

		[JsonProperty("y_cbcr_positioning", NullValueHandling = NullValueHandling.Ignore)]
		public int? YCbCrPositioning { get; set; }

		[JsonProperty("exif_version", NullValueHandling = NullValueHandling.Ignore)]
		public string ExifVersion { get; set; }

		[JsonProperty("gps_info", NullValueHandling = NullValueHandling.Ignore)]
		public GPSInfoData GPSInfo { get; set; }

		[JsonProperty("gps", NullValueHandling = NullValueHandling.Ignore)]
		public AttachmentGPSData Gps { get; set; }
		#endregion


		#region Public Method
		/// <summary>
		/// Shoulds the serialize Y resolution.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeYResolution()
		{
			return YResolution != null && YResolution.Count > 0;
		}

		/// <summary>
		/// Shoulds the serialize X resolution.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeXResolution()
		{
			return XResolution != null && XResolution.Count > 0;
		}

		/// <summary>
		/// Shoulds the serialize exposure time.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeExposureTime()
		{
			return ExposureTime != null && ExposureTime.Count > 0;
		}

		/// <summary>
		/// Shoulds the serialize F number.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeFNumber()
		{
			return FNumber != null && FNumber.Count > 0;
		}

		/// <summary>
		/// Shoulds the serialize aperture value.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeApertureValue()
		{
			return ApertureValue != null && ApertureValue.Count > 0;
		}

		/// <summary>
		/// Shoulds the length of the serialize focal.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeFocalLength()
		{
			return FocalLength != null && FocalLength.Count > 0;
		}
		#endregion
	}
}

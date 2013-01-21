using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Stream.Model;
using System.Globalization;
using log4net;

namespace Wammer.Station.AttachmentUpload
{
	public static class EventTime
	{
		private static ILog logger = LogManager.GetLogger(typeof(EventTime));

		public static DateTime GuessFromExif(exif exif, DateTime? file_create_time, int? localTimeZone, string file_path)
		{
			if (exif != null && exif.gps != null && !string.IsNullOrEmpty(exif.gps.GPSDateStamp) && exif.gps.GPSTimeStamp != null)
			{
				var eventTime = guessFromGPSDateTimeStamp(exif, file_path);
				if (eventTime.HasValue)
					return eventTime.Value;
			}

			if (localTimeZone.HasValue && exif != null && !string.IsNullOrEmpty(exif.DateTimeOriginal))
			{
				var exifTime = caliberateExifDateTime(localTimeZone, exif.DateTimeOriginal, file_path);

				if (exifTime.HasValue)
					return exifTime.Value;
			}

			if (localTimeZone.HasValue && exif != null && !string.IsNullOrEmpty(exif.DateTimeDigitized))
			{
				var exifTime = caliberateExifDateTime(localTimeZone, exif.DateTimeDigitized, file_path);

				if (exifTime.HasValue)
					return exifTime.Value;
			}

			if (localTimeZone.HasValue && exif != null && !string.IsNullOrEmpty(exif.DateTime))
			{
				var exifTime = caliberateExifDateTime(localTimeZone, exif.DateTime, file_path);

				if (exifTime.HasValue)
					return exifTime.Value;
			}

			if (file_create_time.HasValue)
				return file_create_time.Value;
			else
				return DateTime.Now;
		}

		private static DateTime? caliberateExifDateTime(int? localTimeZone, string timeStr, string file_path)
		{
			try
			{
				var exifTime = DateTime.ParseExact(timeStr, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);
				return exifTime.AddMinutes(-localTimeZone.Value);
			}
			catch (Exception e)
			{
				logger.WarnFormat("bad datetime format {0} in {1}: {2}", timeStr, file_path, e.Message);
				return null;
			}
		}

		private static DateTime? guessFromGPSDateTimeStamp(exif exif, string file_path)
		{
			try
			{
				var eventTime = DateTime.ParseExact(exif.gps.GPSDateStamp, "yyyy:MM:dd", CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);

				var hour = getRationalValue(exif.gps.GPSTimeStamp[0]);
				var min = getRationalValue(exif.gps.GPSTimeStamp[1]);
				var sec = getRationalValue(exif.gps.GPSTimeStamp[2]);

				return eventTime.AddHours((double)hour).AddMinutes((double)min).AddSeconds((double)sec);
			}
			catch (Exception e)
			{
				logger.WarnFormat("bad GPD datestamp or timestamp in {0}: {1}", file_path, e.Message);
				return null;
			}
		}

		private static uint getRationalValue(object[] rational)
		{
			var value = Convert.ToUInt32(rational[0]) / Convert.ToUInt32(rational[1]);
			return value;
		}
	}
}

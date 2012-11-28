using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using ExifLibrary;
using System.Collections;

namespace Wammer.Station.AttachmentUpload
{
	public class ExifExtractor : IExifExtractor
	{
		public exif extract(ArraySegment<byte> rawImage)
		{
			DebugInfo.ShowMethod();
			try
			{
				using (var m = new MemoryStream(rawImage.Array, rawImage.Offset, rawImage.Count))
				{
					return extract(m);
				}
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Cannot extract exif information", e);
				return null;
			}
		}

		public exif extract(string file)
		{
			DebugInfo.ShowMethod();

			var extension = Path.GetExtension(file);

			if (!extension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase) && 
				!extension.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase))
				return null;

			try
			{
				using (var m = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					return extract(m);
				}
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Cannot extract exif information: " + file, e);
				return null;
			}
		}

		private exif extract(Stream m)
		{
			ExifFile exifFile = ExifFile.Read(m);

			var exif = new exif();

			foreach (ExifProperty item in exifFile.Properties.Values)
			{
				extractProperty(exif, item);
			}

			extractGPSInfo(exifFile, exif);

			return exif;
		}

		private void extractGPSInfo(ExifFile exifFile, Model.exif exif)
		{
			try
			{
				if (exifFile.Properties.ContainsKey(ExifTag.GPSLatitudeRef) &&
								   exifFile.Properties.ContainsKey(ExifTag.GPSLatitude) &&
								   exifFile.Properties.ContainsKey(ExifTag.GPSLongitudeRef) &&
								   exifFile.Properties.ContainsKey(ExifTag.GPSLongitude))
				{
					var gpsLatitudeRef = exifFile.Properties[ExifTag.GPSLatitudeRef].Value.ToString();
					var gpsLatitude = exifFile.Properties[ExifTag.GPSLatitude] as GPSLatitudeLongitude;
					var gpsLongitudeRef = exifFile.Properties[ExifTag.GPSLongitudeRef].Value.ToString();
					var gpsLongitude = exifFile.Properties[ExifTag.GPSLongitude] as GPSLatitudeLongitude;

					var gpsInfo = new GPSInfo()
					{
						GPSLatitudeRef = gpsLatitudeRef[0].ToString(),
						GPSLongitudeRef = gpsLongitudeRef[0].ToString()
					};

					gpsInfo.GPSLatitude = new List<List<int>>();
					gpsInfo.GPSLatitude.Add(new List<int>() { (int)gpsLatitude.Degrees.Numerator, (int)gpsLatitude.Degrees.Denominator });
					gpsInfo.GPSLatitude.Add(new List<int>() { (int)gpsLatitude.Minutes.Numerator, (int)gpsLatitude.Minutes.Denominator });
					gpsInfo.GPSLatitude.Add(new List<int>() { (int)gpsLatitude.Seconds.Numerator, (int)gpsLatitude.Seconds.Denominator });

					gpsInfo.GPSLongitude = new List<List<int>>();
					gpsInfo.GPSLongitude.Add(new List<int>() { (int)gpsLongitude.Degrees.Numerator, (int)gpsLongitude.Degrees.Denominator });
					gpsInfo.GPSLongitude.Add(new List<int>() { (int)gpsLongitude.Minutes.Numerator, (int)gpsLongitude.Minutes.Denominator });
					gpsInfo.GPSLongitude.Add(new List<int>() { (int)gpsLongitude.Seconds.Numerator, (int)gpsLongitude.Seconds.Denominator });

					exif.GPSInfo = gpsInfo;

					if (exif.gps == null)
						exif.gps = new Gps();

					var Nmult = gpsLatitudeRef.Equals("North", StringComparison.CurrentCultureIgnoreCase) ? 1 : -1;
					var Ndeg = (double)gpsLatitude.Degrees.Numerator / (double)gpsLatitude.Degrees.Denominator;
					var Nmin = (double)gpsLatitude.Minutes.Numerator / (double)gpsLatitude.Minutes.Denominator;
					var Nsec = (double)gpsLatitude.Seconds.Numerator / (double)gpsLatitude.Seconds.Denominator;
					exif.gps.latitude = Nmult * (Ndeg + (Nmin + Nsec / 60.0) / 60.0);

					var Wmult = gpsLongitudeRef.Equals("East", StringComparison.CurrentCultureIgnoreCase) ? 1 : -1;
					var Wdeg = (double)gpsLongitude.Degrees.Numerator / (double)gpsLongitude.Degrees.Denominator;
					var Wmin = (double)gpsLongitude.Minutes.Numerator / (double)gpsLongitude.Minutes.Denominator;
					var Wsec = (double)gpsLongitude.Seconds.Numerator / (double)gpsLongitude.Seconds.Denominator;
					exif.gps.longitude = Wmult * (Wdeg + (Wmin + Wsec / 60.0) / 60.0);

					if (exifFile.Properties.ContainsKey(ExifTag.GPSDateStamp))
					{
						var gpsDate = (DateTime)exifFile.Properties[ExifTag.GPSDateStamp].Value;
						exif.gps.GPSDateStamp = gpsDate.ToString("yyyy:MM:dd");
					}

					if (exifFile.Properties.ContainsKey(ExifTag.GPSTimeStamp))
					{
						var gpsTime = (GPSTimeStamp)exifFile.Properties[ExifTag.GPSTimeStamp];
						exif.gps.GPSTimeStamp = new List<object[]>
						{
							new object[] { gpsTime.Hour.Numerator.ToString(), gpsTime.Hour.Denominator.ToString() },
							new object[] { gpsTime.Minute.Numerator.ToString(), gpsTime.Minute.Denominator.ToString() },
							new object[] { gpsTime.Second.Numerator.ToString(), gpsTime.Minute.Denominator.ToString() },
						};
					}
				}
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Failed to extract GPS info", e);
			}
		}

		private void extractProperty(Model.exif exif, ExifProperty item)
		{
			try
			{
				switch (item.Tag)
				{
					case ExifTag.DateTimeOriginal:
						exif.DateTimeOriginal = ((DateTime)item.Value).ToString("yyyy:MM:dd HH:mm:ss");
						break;
					case ExifTag.DateTimeDigitized:
						exif.DateTimeDigitized = ((DateTime)item.Value).ToString("yyyy:MM:dd HH:mm:ss");
						break;
					case ExifTag.DateTime:
						exif.DateTime = ((DateTime)item.Value).ToString("yyyy:MM:dd HH:mm:ss");
						break;
					case ExifTag.Model:
						exif.Model = item.Value.ToString();
						break;
					case ExifTag.Make:
						exif.Make = item.Value.ToString();
						break;
					case ExifTag.ExposureTime:
						exif.ExposureTime = new List<int>() { (int)((ExifURational)item).Value.Numerator, (int)((ExifURational)item).Value.Denominator };
						break;
					case ExifTag.FNumber:
						exif.FNumber = new List<int>() { (int)((ExifURational)item).Value.Numerator, (int)((ExifURational)item).Value.Denominator };
						break;
					case ExifTag.ApertureValue:
						exif.ApertureValue = new List<int>() { (int)((ExifURational)item).Value.Numerator, (int)((ExifURational)item).Value.Denominator };
						break;
					case ExifTag.FocalLength:
						exif.FocalLength = new List<int>() { (int)((ExifURational)item).Value.Numerator, (int)((ExifURational)item).Value.Denominator };
						break;
					case ExifTag.Flash:
						exif.Flash = (int)((Flash)item.Value);
						break;
					case ExifTag.ISOSpeedRatings:
						exif.ISOSpeedRatings = (int)((ExifLibrary.ExifUShort)(item)).Value;
						break;
					case ExifTag.ColorSpace:
						exif.ColorSpace = (int)((ColorSpace)item.Value);
						break;
					case ExifTag.WhiteBalance:
						exif.WhiteBalance = (int)((WhiteBalance)item.Value);
						break;

					case ExifTag.YResolution:
						exif.YResolution = new List<int>() { (int)((ExifURational)item).Value.Numerator, (int)((ExifURational)item).Value.Denominator };
						break;

					case ExifTag.MeteringMode:
						exif.MeteringMode = (int)((MeteringMode)item.Value);
						break;
					case ExifTag.XResolution:
						exif.XResolution = new List<int>() { (int)((ExifURational)item).Value.Numerator, (int)((ExifURational)item).Value.Denominator };
						break;
					case ExifTag.ExposureProgram:
						exif.ExposureProgram = (int)((ExposureMode)item.Value);
						break;
					case ExifTag.SensingMethod:
						exif.SensingMethod = (int)(ExifLibrary.SensingMethod)item.Value;
						break;
					case ExifTag.Software:
						exif.Software = item.Value.ToString();
						break;
					case ExifTag.FlashpixVersion:
						exif.FlashPixVersion = item.Value.ToString();
						break;
					case ExifTag.YCbCrPositioning:
						exif.YCbCrPositioning = (int)((YCbCrPositioning)item.Value);
						break;
					case ExifTag.ExifVersion:
						exif.ExifVersion = item.Value.ToString();
						break;
				}
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Failed to extract exif property: " + item.ToString(), e);
			}
		}
	}
}

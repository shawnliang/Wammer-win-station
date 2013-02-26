using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station.AttachmentUpload;
using Waveface.Stream.Model;

namespace UT_WammerStation.AttachmentUpload
{
	[TestClass]
	public class TestEventTimeGuess
	{



		[TestMethod]
		public void guessTimeFromGPSDateStampAndGPSTimeStamp()
		{
			var t = EventTime.GuessFromExif(
				new exif
				{
					gps = new Gps
					{
						GPSDateStamp = "2012:10:20",
						GPSTimeStamp = new List<object[]> {
							new object[] { 11, 1 },
							new object[] { 22, 1 },
							new object[] { 33, 11 }
						}
					}
				},

				// these 2 args are DONT-CARE
				DateTime.Now, 480, "file_path");

			Assert.AreEqual(new DateTime(2012, 10, 20, 11, 22, 3, DateTimeKind.Utc).ToLocalTime(), t);
		}

		[TestMethod]
		public void UseDateTimeOriginalIfNoGPSDateTimeStamp()
		{
			var t = EventTime.GuessFromExif(
				new exif
				{
					gps = new Gps
					{
						GPSDateStamp = "2012:10:20",
					},

					DateTimeOriginal = "2012:11:22 11:44:33"
				},
				DateTime.Now, 480, "file_path");

			Assert.AreEqual(new DateTime(2012, 11, 22, 11, 44, 33, DateTimeKind.Local), t);
		}

		[TestMethod]
		public void UseDateTimeOriginalIfGPSDateTimeStampError()
		{
			var t = EventTime.GuessFromExif(
				new exif
				{
					gps = new Gps
					{
						GPSDateStamp = "0113:0:20",
						GPSTimeStamp = new List<object[]> {
							new object[] { 11, 1 },
							new object[] { 22, 1 },
							new object[] { 33, 11 }
						}
					},

					DateTimeOriginal = "2012:11:22 11:44:33"
				},
				DateTime.Now, 480, "file_path");

			Assert.AreEqual(new DateTime(2012, 11, 22, 11, 44, 33, DateTimeKind.Local), t);
		}

		[TestMethod]
		public void UseDateTimeOriginalIfGPSDateTimeStampError2()
		{
			var t = EventTime.GuessFromExif(
				new exif
				{
					gps = new Gps
					{
						GPSDateStamp = "0113:01:20",
						GPSTimeStamp = new List<object[]> {
							new object[] { 11, 1 },
							new object[] { 22, 1 },
							new object[] { 33, 11 }
						}
					},

					DateTimeOriginal = "2012:11:22 11:44:33"
				},
				DateTime.Now, 480, "file_path");

			Assert.AreEqual(new DateTime(2012, 11, 22, 11, 44, 33, DateTimeKind.Local), t);
		}

		[TestMethod]
		public void UseDigitizedTimeIfNoDateTimeOriginal()
		{
			var t = EventTime.GuessFromExif(
				new exif
				{
					DateTimeDigitized = "2012:11:22 11:44:33",
				},
				DateTime.Now, 480, "file_path");

			Assert.AreEqual(new DateTime(2012, 11, 22, 11, 44, 33, DateTimeKind.Local), t);
		}

		[TestMethod]
		public void UseDigitizedTimeIfDateTimeOriginalError()
		{
			var t = EventTime.GuessFromExif(
				new exif
				{
					DateTimeOriginal = "2012:11:22 11:44:338",
					DateTimeDigitized = "2012:11:22 11:44:33",
				},
				DateTime.Now, 480, "file_path");

			Assert.AreEqual(new DateTime(2012, 11, 22, 11, 44, 33, DateTimeKind.Local), t);
		}

		[TestMethod]
		public void UseDateTimeIfNoDateTimeDigitized()
		{
			var t = EventTime.GuessFromExif(
				new exif
				{
					DateTime = "2012:11:22 11:44:33",
				},
				DateTime.Now, 480, "file_path");

			Assert.AreEqual(new DateTime(2012, 11, 22, 11, 44, 33, DateTimeKind.Local), t);
		}

		[TestMethod]
		public void UseDateTimeIfDateTimeError()
		{
			var t = EventTime.GuessFromExif(
				new exif
				{
					DateTimeDigitized = "2030404040450404040400404",
					DateTime = "2012:11:22 11:44:33",
				},
				DateTime.Now, 480, "file_path");

			Assert.AreEqual(new DateTime(2012, 11, 22, 11, 44, 33, DateTimeKind.Local), t);
		}

		[TestMethod]
		public void UseFileCreateTimeIfNoDateTime()
		{
			var file_create_time = new DateTime(2012, 11, 22, 11, 44, 33, DateTimeKind.Local);

			var t = EventTime.GuessFromExif(
				new exif
				{
				},
				file_create_time, 480, "file_path");

			Assert.AreEqual(file_create_time, t);
		}

		[TestMethod]
		public void UseFileCreateTimeIfDateTimeError()
		{
			var file_create_time = new DateTime(2012, 11, 22, 11, 44, 33, DateTimeKind.Local);

			var t = EventTime.GuessFromExif(
				new exif
				{
					DateTime = "1234567 eeeoooorrrrorrrr"
				},
				file_create_time, 480, "file_path");

			Assert.AreEqual(file_create_time, t);
		}

		[TestMethod]
		public void UseCurrentTimeIfNoFileCreateTime()
		{
			var file_create_time = new DateTime(2012, 11, 22, 11, 44, 33, DateTimeKind.Local);

			var t = EventTime.GuessFromExif(
				new exif
				{
					DateTime = "1234567 eeeoooorrrrorrrr"
				},
				null, 480, "file_path");

			Assert.IsTrue(t - DateTime.Now < TimeSpan.FromSeconds(2.0));
		}
	}
}

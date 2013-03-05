using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Model;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public static class SyncStatus
	{
		#region Const
		private const string CATEGORY_NAME = "Waveface Station";
		private const string UPSTREAM_RATE = "Upstream rate (bytes/sec)";
		private const string DWSTREAM_RATE = "Downstream rate (bytes/sec)";
		private const string UP_REMAINED_COUNT = "Atachement upload remained count";
		private const string DW_REMAINED_COUNT = "Atachement download remained count";
		private const string BYTES_TO_DOWNLOAD = "Bytes to download";
		private const string BYTES_TO_UPLOAD = "Bytes to upload";
		#endregion


		#region Private Static Var
		private static Queue<float> _upSpeeds = new Queue<float>();
		private static Queue<float> _downSpeeds = new Queue<float>();

		private static PerformanceCounter m_DownRemainedCountCounter = new PerformanceCounter(CATEGORY_NAME, DW_REMAINED_COUNT, false);
		private static PerformanceCounter m_DownStreamRateCounter = new PerformanceCounter(CATEGORY_NAME, DWSTREAM_RATE, false);
		private static PerformanceCounter m_UpRemainedCountCounter = new PerformanceCounter(CATEGORY_NAME, UP_REMAINED_COUNT, false);
		private static PerformanceCounter m_UpStreamRateCounter = new PerformanceCounter(CATEGORY_NAME, UPSTREAM_RATE, false);
		private static PerformanceCounter m_BytesToDownloadCounter = new PerformanceCounter(CATEGORY_NAME, BYTES_TO_DOWNLOAD, false);
		private static PerformanceCounter m_BytesToUploadCounter = new PerformanceCounter(CATEGORY_NAME, BYTES_TO_UPLOAD, false);
		#endregion


		#region Public Static Property
		public static Boolean IsServiceRunning { get; set; }

		public static float UploadRemainedCount
		{
			get
			{
				var count = m_UpRemainedCountCounter.NextValue();
				return count;
			}
		}

		public static float DownloadRemainedCount
		{
			get
			{
				var count = m_DownRemainedCountCounter.NextValue();
				return count;
			}
		}

		public static float UploadSpeed
		{
			get
			{
				if (UploadRemainedCount == 0)
					return 0;

				var upSpeed = m_UpStreamRateCounter.NextValue();

				if (_upSpeeds.Count >= 5)
					_upSpeeds.Dequeue();

				_upSpeeds.Enqueue(upSpeed);

				if (_upSpeeds.Count >= 0)
					upSpeed = _upSpeeds.Average();

				return upSpeed;
			}
		}

		public static float DownloadSpeed
		{
			get
			{
				if (DownloadRemainedCount == 0)
					return 0;

				var downSpeed = m_DownStreamRateCounter.NextValue();

				if (_downSpeeds.Count >= 5)
					_downSpeeds.Dequeue();

				_downSpeeds.Enqueue(downSpeed);

				if (_downSpeeds.Count >= 0)
					downSpeed = _downSpeeds.Average();

				return downSpeed;
			}
		}
		#endregion

		#region Public Static Method
		public static SyncRange GetSyncRange()
		{
			try
			{
				SyncRange syncRange = null;

				if (StreamClient.Instance.IsLogined)
				{
					var user = DriverCollection.Instance.FindOneById(StreamClient.Instance.LoginedUser.UserID);
					if (user != null)
						syncRange = user.sync_range;
				}

				if (syncRange == null)
					syncRange = new SyncRange();


				return syncRange;
			}
			catch
			{
				return new SyncRange();
			}
		}

		public static void GetSpeedAndUnit(float value, ref float speed, ref string unit)
		{
			var units = new string[] { "B/s", "KB/s", "MB/s" };
			var index = Array.IndexOf(units, unit);

			if (index == -1)
				index = 0;

			if (value > 1024)
			{
				value = value / 1024;
				speed = value;
				unit = units[index + 1];
				GetSpeedAndUnit(value, ref speed, ref unit);
				return;
			}

			speed = value;
			unit = units[index];
		}


		public static string GetSyncTransferStatus()
		{
			var upRemainedCount = SyncStatus.UploadRemainedCount;
			var downloadRemainedCount = SyncStatus.DownloadRemainedCount;

			if ((!IsServiceRunning) || (upRemainedCount <= 0 && downloadRemainedCount <= 0))
				return string.Empty;

			SyncRange syncRange = GetSyncRange();
			if (!string.IsNullOrEmpty(syncRange.GetUploadDownloadError()))
			{
				return Resources.SYNC_ERROR + syncRange.GetUploadDownloadError();
			}

			var transferStatus = string.Empty;
			var upSpeed = SyncStatus.UploadSpeed;
			var downloadSpeed = SyncStatus.DownloadSpeed;

			string upSpeedUnit = string.Empty;
			GetSpeedAndUnit(upSpeed, ref upSpeed, ref upSpeedUnit);

			string downloadSpeedUnit = string.Empty;
			GetSpeedAndUnit(downloadSpeed, ref downloadSpeed, ref downloadSpeedUnit);

			upSpeed = upRemainedCount == 0 ? 0 : upSpeed;
			downloadSpeed = downloadSpeed == 0 ? 0 : downloadSpeed;


			if (upRemainedCount > 0)
			{
				var mbToUpload = getMBytesToUpload();

				transferStatus = string.Format(Resources.INDICATOR_PATTERN,
										 transferStatus,
										 (transferStatus.Length == 0) ? string.Empty : Environment.NewLine,
										 Resources.UPLOAD_INDICATOR,
					//(upRemainedCount > 999) ? "999+" : upRemainedCount.ToString(),
										 mbToUpload,
										 upSpeed.ToString(),
										 upSpeedUnit);
			}

			if (downloadRemainedCount > 0)
			{
				var mbToDownload = getMBytesToDownload();

				transferStatus = string.Format(Resources.INDICATOR_PATTERN,
										 transferStatus,
										 (transferStatus.Length == 0) ? string.Empty : Environment.NewLine,
										 Resources.DOWNLOAD_INDICATOR,
					//(downloadRemainedCount > 999) ? "999+" : downloadRemainedCount.ToString(),
										 mbToDownload,
										 downloadSpeed.ToString(),
										 downloadSpeedUnit);
			}

			return transferStatus;
		}

		private static string getMBytesToDownload()
		{
			var mbytesToDownload = "<1";
			var mbytes = m_BytesToDownloadCounter.RawValue / 1024 / 1024;
			if (mbytes > 0)
				mbytesToDownload = mbytes.ToString();

			return mbytesToDownload;
		}

		private static string getMBytesToUpload()
		{
			var mbToUpload = "<1";
			var mbytes = m_BytesToUploadCounter.RawValue / 1024 / 1024;
			if (mbytes > 0)
				mbToUpload = mbytes.ToString();
			return mbToUpload;
		}

		public static string GetSyncStatus()
		{
			SyncRange syncRange = GetSyncRange();

			if (!string.IsNullOrEmpty(syncRange.download_index_error))
			{
				return Resources.SYNC_ERROR + syncRange.download_index_error;
			}

			var upRemainedCount = SyncStatus.UploadRemainedCount;
			var downloadRemainedCount = SyncStatus.DownloadRemainedCount;

			if (upRemainedCount > 0 || downloadRemainedCount > 0)
			{
				return IsServiceRunning ? Resources.SYNCING_WITH_CLOUD : Resources.SERVICE_STOP;
			}
			else
			{
				if (syncRange.syncing)
				{
					return Resources.DOWNLOAD_INDEX;
				}

				return IsServiceRunning ? Resources.ALL_DATA_SYNCED : Resources.SERVICE_STOP;
			}
		}


		public static string GetSyncDescription()
		{
			var syncDescription = GetSyncStatus();
			var syncTransferStatus = GetSyncTransferStatus();

			if (!string.IsNullOrEmpty(syncTransferStatus))
				syncDescription = string.Format("{0}{1}{2}", syncDescription, Environment.NewLine, syncTransferStatus);

			return syncDescription;
		}
		#endregion
	}
}

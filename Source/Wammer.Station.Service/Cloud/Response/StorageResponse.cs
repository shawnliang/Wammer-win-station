using System;
using Waveface.Stream.Model;

namespace Wammer.Cloud
{
	public class StorageResponse : CloudResponse
	{
		public StorageResponse()
		{
		}

		public StorageResponse(int status, DateTime timestamp)
			: base(status, timestamp)
		{
		}
	}

	public class StorageAuthResponse : StorageResponse
	{
		public StorageAuthResponse()
		{
		}

		public StorageAuthResponse(int status, DateTime timestamp, StorageUrl storages)
			: base(status, timestamp)
		{
			this.storages = storages;
		}

		public StorageUrl storages { get; set; }

		#region Nested type: StorageUrl

		public class StorageUrl
		{
			public string authorization_url { get; set; }
		}

		#endregion
	}

	public class StorageLinkResponse : StorageResponse
	{
		public StorageInfo storages { get; set; }

		public StorageLinkResponse()
		{
		}

		public StorageLinkResponse(int status, DateTime timestamp, StorageInfo storages)
			: base(status, timestamp)
		{
			this.storages = storages;
		}

		#region Nested type: StorageInfo

		public class StorageInfo
		{
			public string token { get; set; }
		}

		#endregion
	}

	public class StorageCheckResponse : StorageResponse
	{
		public StorageStatus storages { get; set; }

		public StorageCheckResponse()
		{
		}

		public StorageCheckResponse(int status, DateTime timestamp, StorageStatus storages)
			: base(status, timestamp)
		{
			this.storages = storages;
		}

		#region Nested type: StorageStatus

		public class StorageStatus
		{
			public int status { get; set; }
			public long updatetime { get; set; }
			public string account { get; set; }
		}

		#endregion
	}

	public class StorageUsageResponse : CloudResponse
	{
		public Storages storages { get; set; }

		public StorageUsageResponse()
		{
		}

		public StorageUsageResponse(int status, DateTime timestamp, Storages storages)
			: base(status, timestamp)
		{
			this.storages = storages;
		}

		#region Nested type: Storages

		public class Storages
		{
			public WFStorage waveface { get; set; }

			#region Nested type: WFStorage

			public class WFStorage
			{
				public WFStorageUsage usage { get; set; }
				public WFStorageAvailable available { get; set; }
				public WFStorageQuota quota { get; set; }
				public QuotaInterval interval { get; set; }
				public bool over_quota { get; set; }

				#region Nested type: QuotaInterval

				public class QuotaInterval
				{
					public long quota_interval_end { get; set; }
					public long quota_interval_begin { get; set; }
					public int quota_interval_left_days { get; set; }
				}

				#endregion

				#region Nested type: WFStorageAvailable

				public class WFStorageAvailable
				{
					public long avail_month_image_objects { get; set; }
					public long avail_month_total_objects { get; set; }
					public long avail_month_doc_objects { get; set; }
				}

				#endregion

				#region Nested type: WFStorageQuota

				public class WFStorageQuota
				{
					public long dropbox_objects { get; set; }
					public long origin_sizes { get; set; }
					public long total_objects { get; set; }
					public long month_total_objects { get; set; }
					public long origin_files { get; set; }
					public long month_doc_objects { get; set; }
					public long meta_files { get; set; }
					public long meta_sizes { get; set; }
					public long total_files { get; set; }
					public long month_image_objects { get; set; }
					public long image_objects { get; set; }
					public long total_sizes { get; set; }
				}

				#endregion

				#region Nested type: WFStorageUsage

				public class WFStorageUsage
				{
					public long dropbox_objects { get; set; }
					public long origin_sizes { get; set; }
					public long total_objects { get; set; }
					public long month_total_objects { get; set; }
					public long origin_files { get; set; }
					public long month_doc_objects { get; set; }
					public long doc_objects { get; set; }
					public long meta_files { get; set; }
					public long meta_sizes { get; set; }
					public long total_files { get; set; }
					public long month_image_objects { get; set; }
					public long image_objects { get; set; }
					public long total_sizes { get; set; }
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Cloud
{
	public class StorageResponse : CloudResponse
	{
		public StorageResponse()
			: base()
		{
		}

		public StorageResponse(int status, DateTime timestamp)
			: base(status, timestamp)
		{
		}
	}

	public class StorageAuthResponse : StorageResponse
	{
		public class StorageUrl
		{
			public string authorization_url { get; set; }
		}

		public StorageUrl storage { get; set; }
		
		public StorageAuthResponse()
			: base()
		{
		}

		public StorageAuthResponse(int status, DateTime timestamp, StorageUrl storage)
			: base(status, timestamp)
		{
			this.storage = storage;
		}
	}

	public class StorageCheckResponse : StorageResponse
	{
		public class DropboxStatus
		{
			public int status { get; set; }
			public long updatetime { get; set; }
		}

		public class StorageStatus
		{
			public DropboxStatus dropbox { get; set; }
		}

		public StorageStatus storage;

		public StorageCheckResponse()
			: base()
		{
		}

		public StorageCheckResponse(int status, DateTime timestamp, StorageStatus storage)
			: base(status, timestamp)
		{
			this.storage = storage;
		}
	}
}

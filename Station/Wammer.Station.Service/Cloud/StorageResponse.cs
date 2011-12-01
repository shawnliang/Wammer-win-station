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

		public StorageUrl storages { get; set; }
		
		public StorageAuthResponse()
			: base()
		{
		}

		public StorageAuthResponse(int status, DateTime timestamp, StorageUrl storages)
			: base(status, timestamp)
		{
			this.storages = storages;
		}
	}

	public class StorageCheckResponse : StorageResponse
	{
		public class StorageStatus
		{
			public int status { get; set; }
			public long updatetime { get; set; }
			public string account { get; set; }
		}

		public StorageStatus storages;

		public StorageCheckResponse()
			: base()
		{
		}

		public StorageCheckResponse(int status, DateTime timestamp, StorageStatus storages)
			: base(status, timestamp)
		{
			this.storages = storages;
		}
	}
}

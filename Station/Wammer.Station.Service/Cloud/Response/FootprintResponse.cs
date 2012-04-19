using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Cloud
{
	public class FootprintResponse : CloudResponse
	{
		public LastScanInfo last_scan { get; set; }

		public FootprintResponse()
			: base()
		{
		}

		public FootprintResponse(int status, DateTime timestamp)
			: base(status, timestamp)
		{
		}
	}

	public class FootprintSetLastScanResponse : FootprintResponse
	{
		public FootprintSetLastScanResponse()
			: base()
		{
		}

		public FootprintSetLastScanResponse(LastScanInfo last_scan)
		{
			this.last_scan = last_scan;
		}
	}

	public class FootprintGetLastScanResponse : FootprintResponse
	{
		public FootprintGetLastScanResponse()
			: base()
		{
		}

		public FootprintGetLastScanResponse(LastScanInfo last_scan)
		{
			this.last_scan = last_scan;
		}
	}

	public class LastScanInfo
	{
		public DateTime timestamp { get; set; }
		public string user_id { get; set; }
		public string group_id { get; set; }
		public string post_id { get; set; }
	}
}

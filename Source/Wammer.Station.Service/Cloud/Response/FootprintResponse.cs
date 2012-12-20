using System;
using Waveface.Stream.Model;

namespace Wammer.Cloud
{
	public class FootprintResponse : CloudResponse
	{
		public FootprintResponse()
		{
		}

		public FootprintResponse(int status, DateTime timestamp)
			: base(status, timestamp)
		{
		}

		public LastScanInfo last_scan { get; set; }
	}

	public class FootprintSetLastScanResponse : FootprintResponse
	{
		public FootprintSetLastScanResponse()
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
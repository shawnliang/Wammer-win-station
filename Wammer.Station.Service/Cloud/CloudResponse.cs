using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
	public class CloudResponse
	{
		public int status { get; set; }
		public DateTime timestamp { get; set; }
		public int app_ret_code { get; set; }
		public string app_ret_msg { get; set; }

		public CloudResponse()
		{
		}

		public CloudResponse(int status, DateTime timestamp)
		{
			this.status = status;
			this.timestamp = timestamp;
		}

		public CloudResponse(int status, DateTime timestamp, int app_code, string app_msg)
		{
			this.status = status;
			this.timestamp = timestamp;
			this.app_ret_code = app_code;
			this.app_ret_msg = app_msg;
		}

		public CloudResponse(int status, int app_code, string app_msg)
		{
			this.status = status;
			this.timestamp = DateTime.Now.ToUniversalTime();
			this.app_ret_code = app_code;
			this.app_ret_msg = app_msg;
		}
	}
}

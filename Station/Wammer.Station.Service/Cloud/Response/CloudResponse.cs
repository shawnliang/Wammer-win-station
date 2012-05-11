using System;

namespace Wammer.Cloud
{
	public class CloudResponse
	{
		public int status { get; set; }
		public DateTime timestamp { get; set; }
		public int api_ret_code { get; set; }
		public string api_ret_message { get; set; }

		public CloudResponse()
		{
			status = 200;
			this.timestamp = DateTime.UtcNow;
			this.api_ret_code = 0;
			this.api_ret_message = "success";
		}

		public CloudResponse(int status, DateTime timestamp)
		{
			this.status = status;
			this.timestamp = timestamp;
			this.api_ret_code = 0;
			this.api_ret_message = "success";
		}

		public CloudResponse(int status, DateTime timestamp, int app_code, string app_msg)
		{
			this.status = status;
			this.timestamp = timestamp;
			this.api_ret_code = app_code;
			this.api_ret_message = app_msg;
		}

		public CloudResponse(int status, int app_code, string app_msg)
		{
			this.status = status;
			this.timestamp = DateTime.Now.ToUniversalTime();
			this.api_ret_code = app_code;
			this.api_ret_message = app_msg;
		}
	}
}

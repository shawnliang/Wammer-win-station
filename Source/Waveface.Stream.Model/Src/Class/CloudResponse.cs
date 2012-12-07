using System;

namespace Waveface.Stream.Model
{
	public class CloudResponse
	{
		public CloudResponse()
		{
			status = 200;
			timestamp = DateTime.UtcNow;
			api_ret_code = 0;
			api_ret_message = "success";
		}

		public CloudResponse(int status, DateTime timestamp)
		{
			this.status = status;
			this.timestamp = timestamp;
			api_ret_code = 0;
			api_ret_message = "success";
		}

		public CloudResponse(int status, DateTime timestamp, int app_code, string app_msg)
		{
			this.status = status;
			this.timestamp = timestamp;
			api_ret_code = app_code;
			api_ret_message = app_msg;
		}

		public CloudResponse(int status, int app_code, string app_msg)
		{
			this.status = status;
			timestamp = DateTime.Now.ToUniversalTime();
			api_ret_code = app_code;
			api_ret_message = app_msg;
		}

		public int status { get; set; }
		public DateTime timestamp { get; set; }
		public int api_ret_code { get; set; }
		public string api_ret_message { get; set; }
	}
}

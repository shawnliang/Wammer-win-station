using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
	public class CloudResponse
	{
		protected int _status;
		protected DateTime _timestamp;
		protected int _app_ret_code;
		protected string _app_ret_msg;

		public CloudResponse()
		{
		}

		public CloudResponse(int status, DateTime timestamp)
		{
			this._status = status;
			this._timestamp = timestamp;
		}

		public CloudResponse(int status, DateTime timestamp, int app_code, string app_msg)
		{
			this._status = status;
			this._timestamp = timestamp;
			this._app_ret_code = app_code;
			this._app_ret_msg = app_msg;
		}

		public CloudResponse(int status, int app_code, string app_msg)
		{
			this._status = status;
			this._timestamp = DateTime.Now.ToUniversalTime();
			this._app_ret_code = app_code;
			this._app_ret_msg = app_msg;
		}

		public int status
		{
			get { return _status; }
			set { _status = value; }
		}

		public DateTime timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}

		public int app_ret_code
		{
			get { return _app_ret_code; }
			set { _app_ret_code = value; }
		}

		public string app_ret_msg
		{
			get { return _app_ret_msg; }
			set { _app_ret_msg = value; }
		}
	}
}

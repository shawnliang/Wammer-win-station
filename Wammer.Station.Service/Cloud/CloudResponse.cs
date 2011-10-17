using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
	public abstract class CloudResponse
	{
		protected int _status;
		protected DateTime _timestamp;

		protected CloudResponse()
		{
		}

		protected CloudResponse(int status, DateTime timestamp)
		{
			this._status = status;
			this._timestamp = timestamp;
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
	}
}

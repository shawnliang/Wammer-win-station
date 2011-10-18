using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
	public class ObjectUploadResponse
	{
		private int _http_status;
		private int _app_ret_code;
		private string _app_ret_msg;
		private DateTime _timestamp;
		private string _object_id;

		public ObjectUploadResponse()
		{

		}

		public static ObjectUploadResponse CreateSuccess(string objectId)
		{
			ObjectUploadResponse res =new ObjectUploadResponse();
			res._app_ret_code = 0;
			res._app_ret_msg = "Success";
			res._http_status = 200;
			res._object_id = objectId;
			res._timestamp = DateTime.Now.ToUniversalTime();

			return res;
		}

		public static ObjectUploadResponse CreateFailure(string objectId,
														int appErr, Exception e)
		{
			ObjectUploadResponse res = new ObjectUploadResponse();
			res._app_ret_code = appErr;
			res._app_ret_msg = e.Message;
			res._http_status = 200;
			res._object_id = objectId;
			res._timestamp = DateTime.Now.ToUniversalTime();

			return res;
		}

		public int status
		{
			get { return _http_status; }
			set { _http_status = value; }
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

		public DateTime timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}

		public string object_id
		{
			get { return _object_id; }
			set { _object_id = value; }
		}
	}
}

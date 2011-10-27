using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
	public class ObjectUploadResponse : CloudResponse
	{
		private string _object_id;

		public ObjectUploadResponse()
		{

		}

		public static ObjectUploadResponse CreateSuccess(string objectId)
		{
			ObjectUploadResponse res = new ObjectUploadResponse();
			res.app_ret_code = 0;
			res.app_ret_msg = "Success";
			res.status = 200;
			res.timestamp = DateTime.Now.ToUniversalTime();
			res._object_id = objectId;

			return res;
		}

		public static ObjectUploadResponse CreateFailure(string objectId,
														int httpStatus, Exception e)
		{
			ObjectUploadResponse res = new ObjectUploadResponse();
			res.app_ret_code = -1;
			res.app_ret_msg = e.Message;
			res.status = httpStatus;
			res.timestamp = DateTime.Now.ToUniversalTime();
			res._object_id = objectId;

			return res;
		}
	
		public string object_id
		{
			get { return _object_id; }
			set { _object_id = value; }
		}
	}
}

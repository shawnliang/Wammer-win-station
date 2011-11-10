using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
	public class ObjectUploadResponse : CloudResponse
	{
		public ObjectUploadResponse()
		{

		}

		public static ObjectUploadResponse CreateSuccess(string objectId)
		{
			ObjectUploadResponse res = new ObjectUploadResponse();
			res.api_ret_code = 0;
			res.api_ret_msg = "Success";
			res.status = 200;
			res.timestamp = DateTime.Now.ToUniversalTime();
			res.object_id = objectId;

			return res;
		}

		public static ObjectUploadResponse CreateFailure(string objectId,
														int httpStatus, Exception e)
		{
			ObjectUploadResponse res = new ObjectUploadResponse();
			res.api_ret_code = -1;
			res.api_ret_msg = e.Message;
			res.status = httpStatus;
			res.timestamp = DateTime.Now.ToUniversalTime();
			res.object_id = objectId;

			return res;
		}
	
		public string object_id { get;set;}
	}
}

using System;

namespace Wammer.Cloud
{
	public class ObjectUploadResponse : CloudResponse
	{
		public static ObjectUploadResponse CreateSuccess(string objectId)
		{
			var res = new ObjectUploadResponse
			          	{
			          		api_ret_code = 0,
			          		api_ret_message = "Success",
			          		status = 200,
			          		timestamp = DateTime.Now.ToUniversalTime(),
			          		object_id = objectId
			          	};

			return res;
		}

		public static ObjectUploadResponse CreateFailure(string objectId,
														int httpStatus, Exception e)
		{
			var res = new ObjectUploadResponse
			          	{
			          		api_ret_code = -1,
			          		api_ret_message = e.Message,
			          		status = httpStatus,
			          		timestamp = DateTime.Now.ToUniversalTime(),
			          		object_id = objectId
			          	};

			return res;
		}
	
		public string object_id { get;set;}
	}
}

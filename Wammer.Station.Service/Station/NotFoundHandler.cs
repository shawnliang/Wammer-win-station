using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace Wammer.Station
{
	public class NotFoundHandler : IHttpHandler
	{
		public NotFoundHandler()
		{

		}

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			response.StatusCode = 404;
			response.ContentType = "application/json";
			using (StreamWriter w = new StreamWriter(response.OutputStream))
			{
				w.Write(
				"{\"status\":\"404\"," +
					"\"app_ret_code\":\"-1\"," +
					"\"app_ret_msg\":\"unknown class/method\" }");
			}
		}
	}
}

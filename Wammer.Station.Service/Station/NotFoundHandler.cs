﻿using System;
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

		public void Handle(object state)
		{
			HttpListenerContext context = (HttpListenerContext)state;

			context.Response.StatusCode = 404;
			context.Response.ContentType = "application/json";
			using (StreamWriter w = new StreamWriter(context.Response.OutputStream))
			{
				w.Write(
				"{\"status\":\"404\"," +
					"\"app_ret_code\":\"-1\"," +
					"\"app_ret_msg\":\"unknown class/method\" }");
			}
		}
	}
}
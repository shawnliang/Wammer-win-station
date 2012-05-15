using System;
using System.IO;
using System.Net;
using Wammer.Cloud;
using Wammer.Utility;
using fastJSON;
using log4net;

namespace Wammer.Station
{
	public class HttpHelper
	{
		private static readonly ILog logger = LogManager.GetLogger("HttpHandler");

		public static void RespondFailure(HttpListenerResponse response, CloudResponse json)
		{
			try
			{
				string resText = json.ToFastJSON();

				response.StatusCode = json.status;
				response.ContentType = "application/json";

				using (var w = new StreamWriter(response.OutputStream))
				{
					w.Write(resText);
				}
			}
			catch (Exception ex)
			{
				logger.Error("Unable to respond failure", ex);
			}
		}

		public static void RespondFailure(HttpListenerResponse response, WammerStationException e, int status)
		{
			CloudResponse json = e.ErrorResponse ??
				new CloudResponse(status, DateTime.Now.ToUniversalTime(), e.WammerError, e.Message);

			RespondFailure(response, json);
		}

		public static void RespondFailure(HttpListenerResponse response, Exception e, int status)
		{
			var json = new CloudResponse(status,
			                             DateTime.Now.ToUniversalTime(), -1, e.Message);

			RespondFailure(response, json);
		}

		public static void RespondSuccess(HttpListenerResponse response, object jsonObj)
		{
			response.StatusCode = 200;
			response.ContentType = "application/json";

			using (var w = new StreamWriter(response.OutputStream))
			{
				if (jsonObj is string)
					w.Write(jsonObj);
				else
				{
					string json = JSON.Instance.ToJSON(jsonObj, false, false, false, false);
					w.Write(json);
				}
			}
		}
	}
}
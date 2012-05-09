using System;
using System.IO;
using System.Net;
using log4net;
using Wammer.Cloud;
using Wammer.Utility;

namespace Wammer.Station
{
	public class HttpHelper
	{
		private static ILog logger = log4net.LogManager.GetLogger("HttpHandler");

		public static void RespondFailure(HttpListenerResponse response, CloudResponse json)
		{
			try
			{
				string resText = json.ToFastJSON();

				response.StatusCode = json.status;
				response.ContentType = "application/json";

				using (StreamWriter w = new StreamWriter(response.OutputStream))
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

			CloudResponse json = null;

			if (e.ErrorResponse != null)
				json = e.ErrorResponse;
			else
				json = new CloudResponse(status,DateTime.Now.ToUniversalTime(), e.WammerError, e.Message);

			RespondFailure(response, json);
		}

		public static void RespondFailure(HttpListenerResponse response, Exception e, int status)
		{
			CloudResponse json = new CloudResponse(status,
					DateTime.Now.ToUniversalTime(), -1, e.Message);

			RespondFailure(response, json);
		}

		public static void RespondSuccess(HttpListenerResponse response, object jsonObj)
		{
			response.StatusCode = 200;
			response.ContentType = "application/json";

			using (StreamWriter w = new StreamWriter(response.OutputStream))
			{
				if (jsonObj is string)
					w.Write((string)jsonObj);
				else
				{
					string json = fastJSON.JSON.Instance.ToJSON(jsonObj, false, false, false, false);
					w.Write(json);
				}
			}
		}
	}
}

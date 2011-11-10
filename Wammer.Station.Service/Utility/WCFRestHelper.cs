using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Wammer.Cloud;

namespace Wammer.Utility
{
	public class WCFRestHelper
	{
		public static MemoryStream GenerateErrStream(WebOperationContext webContext,
			HttpStatusCode status, int apiErrCode, string errMsg)
		{
			try
			{
				webContext.OutgoingResponse.ContentType = "application/json";
				webContext.OutgoingResponse.StatusCode = status;
				CloudResponse res = new CloudResponse((int)status, apiErrCode, errMsg);
				MemoryStream m = new MemoryStream();
				StreamWriter w1 = new StreamWriter(m);
				w1.Write(fastJSON.JSON.Instance.ToJSON(res, false, false, false, false));
				w1.Flush();
				m.Position = 0;
				return m;
			}
			catch (Exception e)
			{
				webContext.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
				log4net.LogManager.GetLogger("HttpHandler").Warn(
														"Internal error when responding error", e);
				return null;
			}
		}

		public static MemoryStream GenerateSucessStream(WebOperationContext webContext, object res)
		{
			try
			{
				webContext.OutgoingResponse.ContentType = "application/json";
				webContext.OutgoingResponse.StatusCode = HttpStatusCode.OK;
				MemoryStream m = new MemoryStream();
				StreamWriter w = new StreamWriter(m);
				w.Write(fastJSON.JSON.Instance.ToJSON(res, false, false, false, false));
				w.Flush();
				m.Position = 0;
				return m;
			}
			catch (Exception e)
			{
				webContext.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
				log4net.LogManager.GetLogger("HttpHandler").Warn(
														"Internal error when responding", e);
				return null;
			}
		}

		public static NameValueCollection ParseFormData(Stream requestContent)
		{
			using (StreamReader r = new StreamReader(requestContent))
			{
				string requestText = r.ReadToEnd();
				return System.Web.HttpUtility.ParseQueryString(requestText);
			}
		}
	}
}

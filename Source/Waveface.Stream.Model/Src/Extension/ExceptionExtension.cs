using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Waveface.Stream.Model;


public static class ExceptionExtension
{
	public static string GetDisplayDescription(this Exception e)
	{
		WammerCloudException cloudErr = null;

		if (e is WebException)
			cloudErr = new WammerCloudException(e.Message, e);
		else if (e is WammerCloudException)
			cloudErr = (WammerCloudException)e;
		else
			return e.Message;


		if (cloudErr.HttpError == WebExceptionStatus.ProtocolError)
			return cloudErr.GetCloudRetMsg();
		else if (cloudErr.InnerException != null)
			return cloudErr.InnerException.Message;
		else
			return cloudErr.Message;
	}
}


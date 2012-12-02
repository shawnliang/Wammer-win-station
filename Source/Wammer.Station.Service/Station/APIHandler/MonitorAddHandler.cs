using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.APIHandler
{
	[APIHandlerInfo(APIHandlerType.ManagementAPI, "/station/monitor/add")]
	public class MonitorAddHandler : HttpHandler
	{
		private Doc.MonitorAddHandlerImp imp = 
			new Doc.MonitorAddHandlerImp(new Doc.MonitorAddHandlerDB(), new Doc.MonitorAddHandlerUtility());

		#region Public Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			CheckParameter("apikey", "session_token", "user_id", "file");

			imp.Process(Parameters["apikey"], Parameters["session_token"], Parameters["user_id"], Parameters["file"]);
			RespondSuccess();
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion
	}
}

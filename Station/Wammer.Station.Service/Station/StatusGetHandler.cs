using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using log4net;
using Wammer.Cloud;

namespace Wammer.Station
{
	public class StatusGetHandler : HttpHandler
	{
		private static ILog logger = LogManager.GetLogger("StationManagement");

		protected override void HandleRequest()
		{
			logger.Debug("GetStatus is called");
			StationDetail res = StatusChecker.GetDetail();

			RespondSuccess(
				new GetStatusResponse
				{
					api_ret_code = 0,
					api_ret_msg = "success",
					status = 200,
					timestamp = DateTime.UtcNow,
					station_status = res
				});
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	public class GetStatusResponse : CloudResponse
	{
		public StationDetail station_status { get; set; }

		public GetStatusResponse()
			: base()
		{
		}
	}
}

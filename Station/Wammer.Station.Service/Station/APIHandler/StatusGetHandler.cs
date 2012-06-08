using System;
using Wammer.Cloud;
using log4net;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.ManagementAPI, "/station/status/get/")]
	public class StatusGetHandler : HttpHandler
	{
		private static readonly ILog logger = LogManager.GetLogger("StationManagement");

		public override void HandleRequest()
		{
			logger.Info("GetStatus is called");
			StationDetail res = StatusChecker.GetDetail();

			RespondSuccess(
				new GetStatusResponse
					{
						api_ret_code = 0,
						api_ret_message = "success",
						status = 200,
						timestamp = DateTime.UtcNow,
						station_status = res
					});
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}

	public class GetStatusResponse : CloudResponse
	{
		public StationDetail station_status { get; set; }
	}
}
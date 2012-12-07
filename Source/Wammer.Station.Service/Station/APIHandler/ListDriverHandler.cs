using System.Collections.Generic;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.ManagementAPI, "/station/drivers/list/")]
	public class ListDriverHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			var drivers = new List<Driver>(DriverCollection.Instance.FindAll());

			RespondSuccess(new ListDriverResponse { drivers = drivers });
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}

	public class ListDriverResponse : CloudResponse
	{
		public List<Driver> drivers { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

using Wammer.Model;
using Wammer.Cloud;

namespace Wammer.Station
{
	public class ListDriverHandler: HttpHandler
	{
		private static ILog logger = LogManager.GetLogger("ListDriverHandler");

		protected override void HandleRequest()
		{
			List<Driver> drivers = new List<Driver>(DriverCollection.Instance.FindAll());

			RespondSuccess(new ListDriverResponse { drivers = drivers});
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	public class ListDriverResponse: CloudResponse
	{
		public List<Driver> drivers { get; set; }

		public ListDriverResponse()
			:base()
		{
		}
	}
}

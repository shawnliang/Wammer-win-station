﻿using System.Collections.Generic;
using log4net;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station
{
	public class ListDriverHandler: HttpHandler
	{
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
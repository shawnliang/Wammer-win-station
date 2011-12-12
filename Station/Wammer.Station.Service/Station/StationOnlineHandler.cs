using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Wammer.Model;

namespace Wammer.Station
{
	public class StationOnlineHandler : HttpHandler
	{
		private readonly HttpServer functionalServer;

		public StationOnlineHandler(HttpServer functionalServer)
		{
			this.functionalServer = functionalServer;
		}

		protected override void HandleRequest()
		{
			string email = Parameters["email"];
			string password = Parameters["password"];
			string stationId = Parameters["station_id"];

			if (email == null || password == null || stationId == null)
			{
				throw new FormatException("email or password or stationId is missing");
			}

			Drivers driver = Drivers.collection.FindOne();
			if (driver == null || driver.email != email)
			{
			}
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		} 
	}
}

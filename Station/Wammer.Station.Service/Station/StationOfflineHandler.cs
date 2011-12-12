using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	public class StationOfflineHandler : HttpHandler
	{
		private readonly HttpServer functionalServer;

		public StationOfflineHandler(HttpServer functionalServer)
		{
			this.functionalServer = functionalServer;
		}

		protected override void HandleRequest()
		{
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}

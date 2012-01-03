using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	class PingHandler: HttpHandler
	{
		protected override void HandleRequest()
		{
			this.RespondSuccess();
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}

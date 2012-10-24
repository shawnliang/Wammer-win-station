using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray.Src.Class
{
	class FacebookConnectableService : IConnectableService
	{
		public string Name
		{
			get { return "Facebook"; }
		}

		public bool Enable
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public System.Drawing.Image Icon
		{
			get { return StationSystemTray.Properties.Resources.f_logo; }
		}
	}
}

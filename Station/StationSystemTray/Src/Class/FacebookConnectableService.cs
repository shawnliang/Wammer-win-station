using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray.Src.Class
{
	class FacebookConnectableService : IConnectableService
	{

		private bool enabled;

		public string Name
		{
			get { return "Facebook"; }
		}

		public bool Enable
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		public System.Drawing.Image Icon
		{
			get { return StationSystemTray.Properties.Resources.f_logo; }
		}
	}
}

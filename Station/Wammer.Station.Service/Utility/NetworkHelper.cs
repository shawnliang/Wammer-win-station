using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Wammer.Station;

namespace Wammer.Utility
{
	class NetworkHelper
	{
		public static IPAddress[] GetIPAddressesV4()
		{
			IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
			List<IPAddress> ret = new List<IPAddress>();

			foreach (IPAddress ip in ips)
			{
				if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
					!IPAddress.IsLoopback(ip))
					ret.Add(ip);
			}

			return ret.ToArray();
		}

		public static string GetBaseURL()
		{
			// fix IP for testing
			string staticIP = (string)StationRegistry.GetValue("staticIP", "");
			if (staticIP != "")
			{
				return "http://" + staticIP + ":9981";
			}

			IPAddress[] addresses = GetIPAddressesV4();

			if (addresses.Length > 0)
				return "http://" + addresses[0] + ":9981";
			else
				// in case there is no external network connection
				return "http://localhost:9981";
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

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
			IPAddress ip = GetIPAddressesV4()[0];
			return "http://" + ip + ":9981/";
		}
	}
}

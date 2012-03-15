using System.Collections.Generic;
using System.Net;

using Wammer.Station;

namespace Wammer.Utility
{
	public class NetworkHelper
	{
		private static byte[] LINK_LOCAL_ADDR = IPAddress.Parse("169.254.0.0").GetAddressBytes();

		public static IPAddress[] GetIPAddressesV4()
		{
			IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
			List<IPAddress> ret = new List<IPAddress>();

			foreach (IPAddress ip in ips)
			{
				if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
					!IPAddress.IsLoopback(ip) &&
					!IsLinkLocal(ip))
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

		public static bool IsLinkLocal(IPAddress addr)
		{
			if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
				return addr.IsIPv6LinkLocal;

			byte[] bytes = addr.GetAddressBytes();
			return (bytes[0] == LINK_LOCAL_ADDR[0] && bytes[1] == LINK_LOCAL_ADDR[1]);
		}
	}
}

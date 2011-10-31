using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;


namespace Wammer.Station
{
	public static class StationInfo
	{
		public static IPAddress IPv4Address { get; private set; }
		public static string BaseURL { get; private set; }


		static StationInfo()
		{
			IPv4Address = GetIPAddressesV4()[0];
			BaseURL = string.Format("http://{0}:9981/{1}/", IPv4Address, 
																Cloud.CloudServer.DEF_BASE_PATH);
		}

		private static IPAddress[] GetIPAddressesV4()
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
	}
}

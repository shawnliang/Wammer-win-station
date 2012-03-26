using System.Collections.Generic;
using System.Net;
using System.Linq;
using Wammer.Station;
using System;
using System.Net.NetworkInformation;

namespace Wammer.Utility
{
	public class NetworkHelper
	{
        private static IEnumerable<String> GetLocalIPAddresses()
        {
            return (from adapter in NetworkInterface.GetAllNetworkInterfaces()
                    let statistics = adapter.GetIPv4Statistics()
                    where (!adapter.IsReceiveOnly && adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback && adapter.NetworkInterfaceType != NetworkInterfaceType.Tunnel) && (statistics.BytesReceived > 0 && statistics.BytesSent > 0)
                    from AddressInfo in adapter.GetIPProperties().UnicastAddresses
                    where AddressInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                    let ipAddress = AddressInfo.Address.ToString()
                    select ipAddress);
        }

		public static string GetBaseURL()
		{
            const string BASE_URL_PATTERN = "http://{0}:9981";
            const string STATIC_IP_KEY = "staticIP";

			// fix IP for testing
            string staticIP = (string)StationRegistry.GetValue(STATIC_IP_KEY, string.Empty);
			if (staticIP.Length > 0)
			{
				return string.Format(BASE_URL_PATTERN, staticIP);
			}

            IEnumerable<String> addresses = GetLocalIPAddresses();

            if (!addresses.Any())
                return string.Empty;

            string address = addresses.First();
            return string.Format(BASE_URL_PATTERN, address);
		}
	}
}

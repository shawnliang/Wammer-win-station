using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Wammer.Station;
using Waveface.Stream.Model;

namespace Wammer.Utility
{
	/// <summary>
	/// 
	/// </summary>
	public class NetworkHelper
	{
		#region Const

		private const string BASE_URL_PATTERN = "http://{0}:9981";
		private const string STATIC_IP_KEY = "staticIP";

		#endregion

		#region Private Static Var

		private static IEnumerable<String> _localIPAddresses;

		#endregion

		#region Private Static Property

		private static DateTime m_LastDateTime { get; set; }

		/// <summary>
		/// Gets or sets the m_localIP addresses.
		/// </summary>
		/// <value>The m_localIP addresses.</value>
		private static IEnumerable<String> m_LocalIPAddresses
		{
			get { return _localIPAddresses ?? (_localIPAddresses = GetLocalIPAddresses()); }
			set { _localIPAddresses = value; }
		}

		/// <summary>
		/// Gets or sets the m_baseURL.
		/// </summary>
		/// <value>The m_baseURL.</value>
		private static String m_BaseURL { get; set; }

		#endregion

		#region Static Constructor

		/// <summary>
		/// Initializes the <see cref="NetworkHelper"/> class.
		/// </summary>
		static NetworkHelper()
		{
			NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
			NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
		}

		#endregion

		#region Private Static Method

		/// <summary>
		/// Resets this instance.
		/// </summary>
		private static void Reset()
		{
			m_LocalIPAddresses = null;
			m_BaseURL = null;
		}

		/// <summary>
		/// Gets the local IP addresses.
		/// </summary>
		/// <returns></returns>
		private static IEnumerable<String> GetLocalIPAddresses()
		{
			return (from adapter in NetworkInterface.GetAllNetworkInterfaces()
					let statistics = adapter.GetIPv4Statistics()
					where (!adapter.IsReceiveOnly && adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
							adapter.NetworkInterfaceType != NetworkInterfaceType.Tunnel) &&
							(statistics.BytesReceived > 0 && statistics.BytesSent > 0)
					from AddressInfo in adapter.GetIPProperties().UnicastAddresses
					let ipAddress = AddressInfo.Address.ToString()
					where !ipAddress.StartsWith("169.254.") && AddressInfo.Address.AddressFamily == AddressFamily.InterNetwork
					select ipAddress);
		}

		#endregion

		#region Public Static Method

		/// <summary>
		/// Gets the base URL.
		/// </summary>
		/// <returns></returns>
		public static string GetBaseURL()
		{
			var currentTime = DateTime.Now;

			if (currentTime.Subtract(m_LastDateTime).TotalMinutes > 1)
			{
				Reset();
				m_LastDateTime = currentTime;
			}

			if (m_BaseURL != null)
				return m_BaseURL;

			// set local lambda function to get final ip address
			Func<string, String> getBaseIPAddress = ip =>
														{
															m_BaseURL = string.Format(BASE_URL_PATTERN, ip);
															return m_BaseURL;
														};

			// fix IP for testing
			var staticIP = (string)StationRegistry.GetValue(STATIC_IP_KEY, string.Empty);
			if (staticIP.Length > 0)
			{
				return getBaseIPAddress(staticIP);
			}

			IEnumerable<String> addresses = m_LocalIPAddresses;

			string address = addresses.FirstOrDefault();

			if (String.IsNullOrEmpty(address))
				return String.Empty;

			return getBaseIPAddress(address);
		}

		#endregion

		#region Event Process

		/// <summary>
		/// Handles the NetworkAvailabilityChanged event of the NetworkChange control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Net.NetworkInformation.NetworkAvailabilityEventArgs"/> instance containing the event data.</param>
		private static void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
		{
			Reset();
		}

		/// <summary>
		/// Handles the NetworkAddressChanged event of the NetworkChange control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private static void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
		{
			Reset();
		}

		#endregion
	}
}
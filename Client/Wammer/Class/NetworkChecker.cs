﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

namespace Waveface
{
	public class NetworkChecker
	{
		#region Static Var
		private static NetworkChecker _instance;
		#endregion


		#region Var
		private Boolean _isNetworkAvailable;
		#endregion


		#region Public Static Property
		public static NetworkChecker Instance
		{
			get
			{
				return _instance ?? (_instance = new NetworkChecker());
			}
		}
		#endregion


		#region Private Property
		private DateTime m_UpdateTime { get; set; }
		#endregion


		#region Public Property
		public Boolean IsNetworkAvailable
		{
			get
			{
				if ((DateTime.Now - m_UpdateTime).TotalMinutes > 1)
					IsNetworkAvailable = Ping(@"www.google.com");
				return _isNetworkAvailable;
			}
			private set
			{
				if (_isNetworkAvailable == value)
					return;

				_isNetworkAvailable = value;
				m_UpdateTime = DateTime.Now;
			}
		}
		#endregion


		#region Constructor
		private NetworkChecker()
		{
			IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
			NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
			NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
		}
		#endregion


		#region Private Method
		private Boolean Ping(string address)
		{
			using (Ping p = new Ping())
			{
				try
				{
					PingReply reply = p.Send(address, 300);
					return reply.Status == IPStatus.Success;
				}
				catch (Exception)
				{
				}
			}

			return false;
		}
		#endregion


		#region Event Process
		void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
		{
			IsNetworkAvailable = e.IsAvailable;
		}

		void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
		{
			IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
		}
		#endregion
	}
}
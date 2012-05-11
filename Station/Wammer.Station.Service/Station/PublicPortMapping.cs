using System;
using System.Net;
using System.Threading;
using TCMPortMapper;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using log4net;

namespace Wammer.Station
{
	public class PublicPortMapping
	{
		//private static PortMapper portMapper = TCMPortMapper.PortMapper.SharedInstance;
		private static readonly ILog logger = LogManager.GetLogger("UPnP");

		private static readonly PublicPortMapping instance;
		private static readonly IPAddress NOT_SUPPORT = IPAddress.Parse("0.0.0.0");

		private readonly Timer checkTimer;
		private readonly object lockObj = new object();
		private IPAddress externalIP;
		private ushort externalPort;
		private bool hasDriver;
		private PortMapping myMapping;
		private UPnPState state;


		static PublicPortMapping()
		{
			instance = new PublicPortMapping();
		}

		private PublicPortMapping()
		{
			hasDriver = (DriverCollection.Instance.FindOne() != null);

			PortMapper.SharedInstance.DidChangeMappingStatus += PortMappingChanged;
			PortMapper.SharedInstance.ExternalIPAddressDidChange += ExternalIPChanged;
			PortMapper.SharedInstance.WillStartSearchForRouter += WillStartSearchForRouter;
			PortMapper.SharedInstance.DidStartWork += DidStartWork;
			PortMapper.SharedInstance.AllowMultithreadedCallbacks = true;
			PortMapper.SharedInstance.Start();


			state = new NoUPnPDeviceFoundState();
			var checkIntervalSec = (int) StationRegistry.GetValue("UPnPCheckInterval", 120);
			checkTimer = new Timer(CheckState, null, 30*1000, checkIntervalSec*1000);
		}

		public static PublicPortMapping Instance
		{
			get { return instance; }
		}

		private void CheckState(object notUsed)
		{
			lock (lockObj)
			{
				UPnPState newState = state.CheckAndTransit();

				while (newState != null && state != newState)
				{
					state = newState;
					newState = state.CheckAndTransit();
				}

				if (newState == null)
				{
					logger.Debug("UPnP state checker reaches end state.");
					checkTimer.Change(Timeout.Infinite, Timeout.Infinite);
					return;
				}
				else
					logger.Debug("Current state is " + newState + ". Try again later..");
			}
		}

		private void AddMapping()
		{
			var r = new Random();
			int desiredPort = r.Next(10000, 60000);

			myMapping = new PortMapping(9981, (ushort) desiredPort, PortMappingTransportProtocol.TCP);
			PortMapper.SharedInstance.AddPortMapping(myMapping);
		}

		public UPnPInfo GetUPnPInfo()
		{
			lock (lockObj)
			{
				var info = new UPnPInfo();
				if (IsReadyToNotifyCloud())
				{
					info.status = true;
					info.public_addr = externalIP.ToString();
					info.public_port = externalPort;
				}
				else
				{
					info.status = false;
					info.public_port = 0;
					info.public_addr = null;
				}

				return info;
			}
		}

		public void Close()
		{
			try
			{
				PortMapper.SharedInstance.DidChangeMappingStatus -= PortMappingChanged;
				PortMapper.SharedInstance.StopBlocking();
			}
			catch (Exception e)
			{
				logger.Warn("Exception when closing public port mapping", e);
			}
		}

		private void WillStartSearchForRouter(PortMapper sender)
		{
		}

		private void DidStartWork(PortMapper sender)
		{
		}


		private void PortMappingChanged(PortMapper sender, PortMapping pm)
		{
			lock (lockObj)
			{
				if (pm != myMapping)
					return;

				if (pm.MappingStatus != PortMappingStatus.Mapped)
					return;

				externalPort = pm.ExternalPort;
				logger.Info("External port is registered: " + externalPort);

				if (IsReadyToNotifyCloud())
					TaskQueue.Enqueue(new NotifyCloudOfExternalPortTask(), TaskPriority.Medium);
			}
		}

		private void ExternalIPChanged(PortMapper sender, IPAddress externalIP)
		{
			lock (lockObj)
			{
				if (externalIP == null)
					return;

				this.externalIP = externalIP;
				logger.Info("Public IP detected: " + externalIP);

				if (IsReadyToNotifyCloud())
					TaskQueue.Enqueue(new NotifyCloudOfExternalPortTask(), TaskPriority.Medium);
			}
		}

		public void DriverAdded(object sender, DriverAddedEvtArgs evt)
		{
			lock (lockObj)
			{
				hasDriver = true;

				if (IsReadyToNotifyCloud())
					TaskQueue.Enqueue(new NotifyCloudOfExternalPortTask(), TaskPriority.Medium);
			}
		}

		public bool IsReadyToNotifyCloud()
		{
			return externalIP != null && externalPort != 0 && hasDriver;
		}

		////////////////////////////////////////////////////////////////////////////
		// Innter classes
		////////////////////////////////////////////////////////////////////////////

		#region Nested type: NoExternalPortState

		private class NoExternalPortState : UPnPState
		{
			#region UPnPState Members

			public UPnPState CheckAndTransit()
			{
				if (Instance.externalPort != 0)
					return null;

				logger.Debug("Add port mapping");
				Instance.AddMapping();
				return this;
			}

			#endregion
		}

		#endregion

		#region Nested type: NoUPnPDeviceFoundState

		private class NoUPnPDeviceFoundState : UPnPState
		{
			#region UPnPState Members

			public UPnPState CheckAndTransit()
			{
				if (Instance.externalIP != null &&
				    !Instance.externalIP.Equals(NOT_SUPPORT))
					return new NoExternalPortState();


				logger.Debug("Refresh to find UPnP Gateway");
				PortMapper.SharedInstance.Refresh();
				return this;
			}

			#endregion
		}

		#endregion

		#region Nested type: UPnPState

		private interface UPnPState
		{
			UPnPState CheckAndTransit();
		}

		#endregion
	}


	internal class NotifyCloudOfExternalPortTask : ITask
	{
		private static readonly ILog logger = LogManager.GetLogger("UPnP");

		#region ITask Members

		public void Execute()
		{
			try
			{
				StationInfo stationInfo = StationCollection.Instance.FindOne();
				if (stationInfo == null)
					return;

				var api = new StationApi(stationInfo.Id, stationInfo.SessionToken);

				StationDetail detail = StatusChecker.GetDetail();

				logger.Debug("Notify cloud about station info changed: " + detail.ToFastJSON());

				using (WebClient client = new DefaultWebClient())
				{
					api.Heartbeat(client, detail);
				}
			}
			catch (Exception e)
			{
				logger.Warn("Unable to notify external ip/port to cloud", e);
			}
		}

		#endregion
	}
}
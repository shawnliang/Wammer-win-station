using System;
using System.Net;
using System.Threading;
using TCMPortMapper;
using Wammer.Utility;

namespace Wammer.Station
{
	public class PublicPortMapping
	{
		//private static PortMapper portMapper = TCMPortMapper.PortMapper.SharedInstance;
		private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("UPnP");

		private bool hasDriver = false;
		private ushort externalPort = 0;
		private IPAddress externalIP = null;

		private PortMapping myMapping = null;
		private readonly object lockObj = new object();
		private static readonly PublicPortMapping instance = null;

		private readonly Timer checkTimer;
		private UPnPState state;


		private static readonly IPAddress NOT_SUPPORT = IPAddress.Parse("0.0.0.0");

		static PublicPortMapping()
		{
			instance = new PublicPortMapping();
		}

		public static PublicPortMapping Instance
		{
			get
			{
				return instance;
			}
		}

		private PublicPortMapping()
		{
			this.hasDriver = (Model.DriverCollection.Instance.FindOne() != null);

			TCMPortMapper.PortMapper.SharedInstance.DidChangeMappingStatus += new PortMapper.PMDidChangeMappingStatus(PortMappingChanged);
			TCMPortMapper.PortMapper.SharedInstance.ExternalIPAddressDidChange += new PortMapper.PMExternalIPAddressDidChange(ExternalIPChanged);
			TCMPortMapper.PortMapper.SharedInstance.WillStartSearchForRouter += new PortMapper.PMWillStartSearchForRouter(WillStartSearchForRouter);
			TCMPortMapper.PortMapper.SharedInstance.DidStartWork += new PortMapper.PMDidStartWork(DidStartWork);
			TCMPortMapper.PortMapper.SharedInstance.AllowMultithreadedCallbacks = true;
			TCMPortMapper.PortMapper.SharedInstance.Start();


			this.state = new NoUPnPDeviceFoundState();
			var checkIntervalSec = (int)StationRegistry.GetValue("UPnPCheckInterval", 120);
			this.checkTimer = new Timer(this.CheckState, null, 30 * 1000, checkIntervalSec * 1000);
		}

		private void CheckState(object notUsed)
		{
			lock (lockObj)
			{
				UPnPState newState = this.state.CheckAndTransit();

				while (newState != null && this.state != newState)
				{
					this.state = newState;
					newState = this.state.CheckAndTransit();
				}

				if (newState == null)
				{
					logger.Debug("UPnP state checker reaches end state.");
					this.checkTimer.Change(Timeout.Infinite, Timeout.Infinite);
					return;
				}
				else
					logger.Debug("Current state is " + newState.ToString() + ". Try again later..");
			}
		}

		private void AddMapping()
		{
			Random r = new Random();
			int desiredPort = r.Next(10000, 60000);

			myMapping = new PortMapping(9981, (ushort)desiredPort, PortMappingTransportProtocol.TCP);
			TCMPortMapper.PortMapper.SharedInstance.AddPortMapping(myMapping);
		}

		public Cloud.UPnPInfo GetUPnPInfo()
		{
			lock (lockObj)
			{
				Cloud.UPnPInfo info = new Cloud.UPnPInfo();
				if (IsReadyToNotifyCloud())
				{
					info.status = true;
					info.public_addr = externalIP.ToString();
					info.public_port = this.externalPort;
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
				TCMPortMapper.PortMapper.SharedInstance.DidChangeMappingStatus -= this.PortMappingChanged;
				TCMPortMapper.PortMapper.SharedInstance.StopBlocking();
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
				logger.Info("Public IP detected: " + externalIP.ToString());

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
			return this.externalIP != null && this.externalPort != 0 && this.hasDriver;
		}
		
		////////////////////////////////////////////////////////////////////////////
		// Innter classes
		////////////////////////////////////////////////////////////////////////////
		interface UPnPState
		{
			UPnPState CheckAndTransit();
		}

		class NoUPnPDeviceFoundState : UPnPState
		{
			public UPnPState CheckAndTransit()
			{
				if (PublicPortMapping.Instance.externalIP != null &&
					!PublicPortMapping.Instance.externalIP.Equals(NOT_SUPPORT))
					return new NoExternalPortState();


				logger.Debug("Refresh to find UPnP Gateway");
				PortMapper.SharedInstance.Refresh();
				return this;
			}
		}

		class NoExternalPortState: UPnPState
		{
			public UPnPState CheckAndTransit()
			{
				if (PublicPortMapping.Instance.externalPort != 0)
					return null;

				logger.Debug("Add port mapping");
				PublicPortMapping.Instance.AddMapping();
				return this;
			}
		}
	}


	class NotifyCloudOfExternalPortTask : ITask
	{
		private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("UPnP");

		public void Execute()
		{
			try
			{
				Model.StationInfo stationInfo = Model.StationCollection.Instance.FindOne();
				if (stationInfo == null)
					return;

				Cloud.StationApi api = new Cloud.StationApi(stationInfo.Id, stationInfo.SessionToken);

				Cloud.StationDetail detail = StatusChecker.GetDetail();

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
	}
	
}

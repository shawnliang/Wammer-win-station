using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCMPortMapper;
using System.Net;
using System.Threading;

using Wammer.Utility;

namespace Wammer.Station
{
	public class PublicPortMapping
	{
		private static PortMapper portMapper = TCMPortMapper.PortMapper.SharedInstance;
		private static log4net.ILog logger = log4net.LogManager.GetLogger("UPnP");

		private bool hasDriver = false;
		private ushort externalPort = 0;
		private IPAddress externalIP = null;

		private PortMapping myMapping = null;
		private object lockObj = new object();
		private static PublicPortMapping instance = null;

		private Timer checkTimer;
		private UPnPState state;

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
			this.hasDriver = (Model.Drivers.collection.FindOne() != null);

			TCMPortMapper.PortMapper.SharedInstance.DidChangeMappingStatus += new PortMapper.PMDidChangeMappingStatus(PortMappingChanged);
			TCMPortMapper.PortMapper.SharedInstance.ExternalIPAddressDidChange += new PortMapper.PMExternalIPAddressDidChange(ExternalIPChanged);
			TCMPortMapper.PortMapper.SharedInstance.WillStartSearchForRouter += new PortMapper.PMWillStartSearchForRouter(WillStartSearchForRouter);
			TCMPortMapper.PortMapper.SharedInstance.DidStartWork += new PortMapper.PMDidStartWork(DidStartWork);
			TCMPortMapper.PortMapper.SharedInstance.AllowMultithreadedCallbacks = true;
			TCMPortMapper.PortMapper.SharedInstance.Start();


			this.state = new NoUPnPDeviceFoundState();
			int checkIntervalSec = (int)StationRegistry.GetValue("UPnPCheckInterval", 120);
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
					logger.Debug("Current state is " + newState.ToString() + "Try again later..");
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
					ThreadPool.QueueUserWorkItem(this.NotifyCloudOfExternalPort);
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
					ThreadPool.QueueUserWorkItem(this.NotifyCloudOfExternalPort);
			}
		}

		public void DriverAdded(object sender, DriverAddedEvtArgs evt)
		{
			lock (lockObj)
			{
				hasDriver = true;

				if (IsReadyToNotifyCloud())
					ThreadPool.QueueUserWorkItem(this.NotifyCloudOfExternalPort);
			}
		}

		public bool IsReadyToNotifyCloud()
		{
			return this.externalIP != null && this.externalPort != 0 && this.hasDriver;
		}

		private void NotifyCloudOfExternalPort(object nil)
		{
			logger.InfoFormat("Notify public port info to cloud: ext ip {0}, ext port {1} via heartbeat",
				this.externalIP, this.externalPort);

			try
			{
				Model.StationInfo stationInfo = Model.StationInfo.collection.FindOne();
				if (stationInfo == null)
					return;

				Cloud.Station station = new Cloud.Station(stationInfo.Id, stationInfo.SessionToken);

				string json = StatusChecker.GetDetail().ToFastJSON();

				logger.Debug("detail: " + json);

				station.Heartbeat(new WebClient(), new Dictionary<object, object>{
					{ "detail",  json }
				});
			}
			catch (Exception e)
			{
				logger.Warn("Unable to notify external ip/port to cloud", e);
			}
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
				if (PublicPortMapping.Instance.externalIP != null)
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
	
}

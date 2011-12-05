using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCMPortMapper;
using System.Net;
using System.Threading;

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


			Timer delayPortMapping = new Timer(this.addPortMapping, null, 10 * 1000, -1);
		}

		private void addPortMapping(object state)
		{
			myMapping = new PortMapping(9981, 21981, PortMappingTransportProtocol.TCP);
			TCMPortMapper.PortMapper.SharedInstance.AddPortMapping(myMapping);
		}

		public UPnPInfo GetUPnPInfo()
		{
			lock (lockObj)
			{
				UPnPInfo info = new UPnPInfo();
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

				string json = fastJSON.JSON.Instance.ToJSON(
					StatusChecker.GetDetail(),false,false,false,false);

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
	}
}

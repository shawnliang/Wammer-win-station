using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Reflection;

using Microsoft.Win32;
using Wammer.Station;

namespace Wammer.Station.Service
{
	public partial class StationService : ServiceBase
	{
		private static log4net.ILog logger = log4net.LogManager.GetLogger("StationService");
		private HttpServer server;


		public StationService()
		{
			log4net.Config.XmlConfigurator.Configure();
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			Environment.CurrentDirectory = Path.GetDirectoryName(
									Assembly.GetExecutingAssembly().Location);

			fastJSON.JSON.Instance.UseUTCDateTime = true;

			// TODO: these lines will be removed after how space is used is defined.
			if (!Directory.Exists("resource"))
				Directory.CreateDirectory("resource");
			if (!Directory.Exists(@"resource\space1"))
				Directory.CreateDirectory(@"resource\space1");
			if (!Directory.Exists(@"resource\space1\100"))
				Directory.CreateDirectory(@"resource\space1\100");
			if (!Directory.Exists(@"resource\space1\101"))
				Directory.CreateDirectory(@"resource\space1\101");
			if (!Directory.Exists(@"resource\space1\102"))
				Directory.CreateDirectory(@"resource\space1\102");
			if (!Directory.Exists(@"resource\space1\103"))
				Directory.CreateDirectory(@"resource\space1\103");
			if (!Directory.Exists(@"resource\space1\104"))
				Directory.CreateDirectory(@"resource\space1\104");

			server = new HttpServer(9981); // TODO: remove hard code
			server.AddDefaultHandler(new NotFoundHandler());

			//TODO: v1 is hard coded
			server.AddHandler("/v1/objects/view/", new ViewObjectHandler("resource"));
			server.AddHandler("/v1/objects/upload/", new ObjectUploadHandler());
			server.Start();

			if (!LogOnStation(9981))
			{
				logger.Info("Not connected with Wammer Cloud yet");
				//TODO: start a timer to retry
			}
			else
				logger.Info("Station log on Wammer Cloud successfully");
		}

		private bool LogOnStation(int port)
		{
			try
			{
				string stationId = (string)StationRegistry.GetValue("stationId", null);
				string stationToken = (string)StationRegistry.GetValue("stationToken", null);

				if (stationId == null || stationToken == null)
					return false;

				Wammer.Cloud.Station station = new Cloud.Station(stationId, stationToken);
				Dictionary<object, object> parameters = new Dictionary<object, object>();
				parameters.Add("host_name", Dns.GetHostName());
				parameters.Add("ip_address", GetOneLocalIPAddress());
				parameters.Add("port", port.ToString());
				station.LogOn(new System.Net.WebClient(), parameters);

				StationRegistry.SetValue("stationToken", station.Token);
				return true;
			}
			catch (Exception e)
			{
				logger.Warn("Unable to logon station with Wammer Cloud", e);
				return false;
			}
		}

		private string GetOneLocalIPAddress()
		{
			IPAddress[] ips =  Dns.GetHostAddresses(Dns.GetHostName());
			foreach (IPAddress ip in ips)
			{
				if (!IPAddress.IsLoopback(ip))
					return ip.ToString();
			}

			return "";
		}

		protected override void OnStop()
		{
			server.Stop();
			server.Close();
		}
	}
}

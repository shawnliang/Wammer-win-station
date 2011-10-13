using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using System.Text;
using Microsoft.Win32;
using Wammer.Station;

namespace Wammer.Station.Service
{
    public partial class StationService : ServiceBase
    {
        public static log4net.ILog logger = log4net.LogManager.GetLogger("StationService");

        private HttpListener httpListener = new HttpListener();

        public StationService()
        {
            log4net.Config.XmlConfigurator.Configure();
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            httpListener.Prefixes.Add("http://+:9981/api/v2/");
            httpListener.Start();
            //httpListener.BeginGetContext(......);

            if (!LogOnStation())
            {
                logger.Info("Not connected with Wammer Cloud yet");
                //TODO: start a timer to retry
            }

            //TODO: start http listener to serve request
        }

        private bool LogOnStation()
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
                parameters.Add("port", "9981"); //TODO: resolve hard code
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
            httpListener.Stop();
            httpListener.Close();
        }
    }
}

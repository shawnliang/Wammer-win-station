using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using System.Text;
using Microsoft.Win32;

namespace Wammer.Station.Service
{
    public partial class StationService : ServiceBase
    {
        public StationService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (!LogOnStation())
            {
                //TODO: start a timer to retry
            }

            //TODO: start http listener to serve request
        }

        private bool LogOnStation()
        {
            try
            {
                string stationId = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wammer\WinStation", "stationId", null);
                string stationToken = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wammer\WinStation", "stationToken", null);

                Wammer.Cloud.Station station = new Cloud.Station(stationId, stationToken);
                Dictionary<object, object> parameters = new Dictionary<object, object>();
                parameters.Add("host_name", Dns.GetHostName());
                parameters.Add("ip_address", GetOneLocalIPAddress());
                station.LogOn(new System.Net.WebClient(), parameters);

                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wammer\WinStation", "stationToken", station.Token);
                return true;
            }
            catch (Exception e)
            {
                //TODO: write log
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
        }
    }
}

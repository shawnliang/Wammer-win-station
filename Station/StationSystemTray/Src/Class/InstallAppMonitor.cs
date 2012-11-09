using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Wammer.Model;
using MongoDB.Driver.Builders;
using System.Windows.Forms;
using StationSystemTray.Src.Control;

namespace StationSystemTray.Src.Class
{
	class InstallAppMonitor
	{
		private long initialConnectionCount;
		private Timer timer = new Timer();
		

		public InstallAppMonitor()
		{
			this.timer.Interval = 1000; // ms
			this.timer.Tick += new EventHandler(timer_Tick);
			this.timer.Enabled = false;
		}

		public void OnAppInstall(object sender, AppInstalEventArgs arg)
		{
			initialConnectionCount = getCurConnectionCount(arg.user_id);
			timer.Start();
			timer.Tag = new TimerParam()
			{
				build_control = (BuildPersonalCloudUserControl)sender,
				user_id = arg.user_id
			};
		}

		private long getCurConnectionCount(string user_id)
		{
			var connections = getConnectedClients(user_id);

			return connections.Count();
		}

		private MongoDB.Driver.MongoCursor<LoginedSession> getConnectedClients(string user_id)
		{
			var connections = string.IsNullOrEmpty(user_id) ?
								  ConnectionCollection.Instance.FindAll() :
								  ConnectionCollection.Instance.Find(Query.EQ("user.user_id", user_id));
			return connections;
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Stop();
			
			var parameters = (TimerParam)timer.Tag;

			var curCount = getCurConnectionCount(parameters.user_id);

			if (curCount != initialConnectionCount)
			{
				var clients = getConnectedClients(parameters.user_id);
				var devices = string.Join("/", clients.Select(x => x.device.device_name).ToArray());

				parameters.build_control.ShowDeviceConnected(devices);
				parameters.build_control.CloseInstallDialog();
			}
			else
				timer.Enabled = true;
		}

		public void OnAppInstallCanceled(object sender, AppInstalEventArgs arg)
		{
			timer.Stop();
		}
	}

	class TimerParam
	{
		public string user_id { get; set; }
		public BuildPersonalCloudUserControl build_control { get; set; }
	}
}
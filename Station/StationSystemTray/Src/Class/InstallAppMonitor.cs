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
		private string user_id;
		private long initialConnectionCount;
		private Timer timer = new Timer();
		

		public InstallAppMonitor(string user_id = null)
		{
			this.user_id = user_id;
			this.timer.Interval = 1000; // ms
			this.timer.Tick += new EventHandler(timer_Tick);
			this.timer.Enabled = false;
		}

		public void OnAppInstall(object sender, EventArgs arg)
		{
			initialConnectionCount = getCurConnectionCount();
			timer.Start();
			timer.Tag = (BuildPersonalCloudUserControl)sender;
		}

		private long getCurConnectionCount()
		{
			var connections = string.IsNullOrEmpty(user_id) ?
						 ConnectionCollection.Instance.FindAll() :
						 ConnectionCollection.Instance.Find(Query.EQ("user.user_id", user_id));

			return connections.Count();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Stop();

			var curCount = getCurConnectionCount();

			if (curCount != initialConnectionCount)
			{
				var buildCloud = (BuildPersonalCloudUserControl)timer.Tag;
				buildCloud.CloseInstallDialog();
			}
			else
				timer.Enabled = true;
		}

		public void OnAppInstallCanceled(object sender, EventArgs arg)
		{
			timer.Stop();
		}
	}
}
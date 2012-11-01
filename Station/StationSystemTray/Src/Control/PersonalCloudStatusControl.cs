using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray
{
	public partial class PersonalCloudStatusControl : StepPageControl
	{
		private IPersonalCloudStatusService service;
		private string user_id;
		private string session_token;

		public PersonalCloudStatusControl(IPersonalCloudStatusService service)
		{
			InitializeComponent();
			this.service = service;

			timer.Tick += new EventHandler(timer_Tick);
		}

		void timer_Tick(object sender, EventArgs e)
		{
			timer.Stop();
			try
			{
				var status = service.GetStatus(user_id);
				var devices = service.GetDeviceList(user_id, StationAPI.API_KEY, session_token);
				refreshData(status, devices);
			}
			catch (Exception ex)
			{
			}
			finally
			{
				timer.Start();
			}
		}

		public override void OnEnteringStep(WizardParameters parameters)
		{
			user_id = (string)parameters.Get("user_id");
			session_token = (string)parameters.Get("session_token");

			var status = service.GetStatus(user_id);
			var devices = service.GetDeviceList(user_id, StationAPI.API_KEY, session_token);
			refreshData(status, devices);

			timer.Start();
		}

		public override void OnLeavingStep(WizardParameters parameters)
		{
			timer.Stop();
		}

		private void refreshData(PersonalCloudStatus status, IEnumerable<StreamDevice> devices)
		{
			if (InvokeRequired)
				Invoke(new MethodInvoker(() => {
					refreshData(status, devices);
				}));

			photoCount.Text = status.PhotoCount.ToString();
			eventCount.Text = status.EventCount.ToString();
			deviceGridView.Rows.Clear();
			foreach (var device in devices)
			{
				deviceGridView.Rows.Add(
					new object[]
					{
						device.Name,
						device.Type,
						device.Online? "connected" : "not connected"
					});
			}
		}
	}
}

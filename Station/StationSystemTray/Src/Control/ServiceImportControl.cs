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
	public partial class ServiceImportControl : StepPageControl
	{
		private IConnectableService service;
		private string session_token;
		private string user_id;

		public ServiceImportControl(IConnectableService service)
		{
			InitializeComponent();
			this.service = service;
		}

		private void ServiceImportControl_Load(object sender, EventArgs e)
		{
			svcItem.ServiceIcon = service.Icon;
			svcItem.ServiceName = service.Name;
			svcItem.OnChange += svcItem_OnChange;
		}

		public override void OnEnteringStep(WizardParameters parameters)
		{
			this.user_id = (string)parameters.Get("user_id");
			this.session_token = (string)parameters.Get("session_token");

			svcItem.ServiceEnabled = service.IsEnabled(user_id, session_token, StationAPI.API_KEY);
		}

		void svcItem_OnChange(object sender, Src.Control.ServiceConnectivityChangeEventArgs e)
		{
			try
			{
				if (e.TurnedOn)
				{
					service.Connect(session_token, StationAPI.API_KEY);

					// detect if connect operation is cancelled by clicking "cancel" in web form
					if (!service.IsEnabled(user_id, session_token, StationAPI.API_KEY))
					{
						e.Cancel = true;
					}

				}
				else
				{
					service.Disconnect(session_token, StationAPI.API_KEY);
				}
			}
			catch (OperationCanceledException)
			{
				// user cancel the operation by closing web browser
				e.Cancel = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "[TBD] Unable to " +
					(e.TurnedOn ? "connect with " : "disconnect with ") + service.Name);

				e.Cancel = true;
			}
		}

		
	}
}

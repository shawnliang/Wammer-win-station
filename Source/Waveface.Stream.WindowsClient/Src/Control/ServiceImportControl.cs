using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;


namespace Waveface.Stream.WindowsClient
{
	public partial class ServiceImportControl : StepPageControl
	{
		private string session_token;
		private string user_id;
		private List<ServiceItemControl> itemControls = new List<ServiceItemControl>();

		[ImportMany(typeof(IConnectableService))]
		private IEnumerable<IConnectableService> services { get; set; }

		public ServiceImportControl()
		{
			InitializeComponent();
		}

		private void ServiceImportControl_Load(object sender, EventArgs e)
		{
			var catalog = new AssemblyCatalog(this.GetType().Assembly);
			var container = new CompositionContainer(catalog);
			container.ComposeParts(this);


			foreach (var service in services)
			{
				var svcItem = new ServiceItemControl();
				svcItem.ServiceName = service.Name;
				svcItem.ServiceIcon = service.Icon;
				svcItem.OnChange += svcItem_OnChange;
				svcItem.Tag = service;

				itemControls.Add(svcItem);
				svcPanel.Controls.Add(svcItem);
			}

			foreach (var svcItem in itemControls)
			{
				var service = (IConnectableService)svcItem.Tag;
				svcItem.ServiceEnabled = service.IsEnabled(user_id, session_token, StationAPI.API_KEY);
			}
		}

		public override void OnEnteringStep(WizardParameters parameters)
		{
			this.user_id = (string)parameters.Get("user_id");
			this.session_token = (string)parameters.Get("session_token");
		}

		void svcItem_OnChange(object sender, ServiceConnectivityChangeEventArgs e)
		{
			var service = (IConnectableService)((ServiceItemControl)sender).Tag;

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

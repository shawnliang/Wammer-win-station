using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;


namespace Waveface.Stream.WindowsClient
{
	public partial class ServiceImportControl : UserControl
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
			if (this.IsDesignMode())
				return;

			var user = StreamClient.Instance.LoginedUser;
			this.user_id = user.UserID;
			this.session_token = user.SessionToken;

			var catalog = new AssemblyCatalog(this.GetType().Assembly);
			var container = new CompositionContainer(catalog);
			container.ComposeParts(this);


			foreach (var service in services)
			{
				var svcItem = new ServiceItemControl();
				svcItem.ServiceName = service.Name;
				svcItem.Description = service.Description;
				svcItem.ServiceIcon = service.Icon;
				svcItem.OnChange += svcItem_OnChange;
				svcItem.Tag = service;

				itemControls.Add(svcItem);
				svcPanel.Controls.Add(svcItem);
			}

			SuspendLayout();
			foreach (var svcItemControl in svcPanel.Controls)
			{
				var item = (ServiceItemControl)svcItemControl;
				item.Size = new System.Drawing.Size(svcPanel.ClientSize.Width - 10, item.Height);
			}
			ResumeLayout();

			foreach (var svcItem in itemControls)
			{
				var service = (IConnectableService)svcItem.Tag;
				svcItem.ServiceEnabled = service.IsEnabled(user_id, session_token, StationAPI.API_KEY);
			}
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

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
	public partial class ServiceImportControl : UserControl
	{
		private IConnectableService service;

		public ServiceImportControl(IConnectableService service)
		{
			InitializeComponent();
			this.service = service;
		}

		private void ServiceImportControl_Load(object sender, EventArgs e)
		{
			svcItem.ServiceEnabled = service.Enabled;
			svcItem.ServiceIcon = service.Icon;
			svcItem.ServiceName = service.Name;
			svcItem.OnChange += svcItem_OnChange;
		}

		void svcItem_OnChange(object sender, Src.Control.ServiceConnectivityChangeEventArgs e)
		{
			try
			{
				service.Enabled = e.TurnedOn;

				// detect if connect operation is cancelled by user
				var serviceOn = service.Enabled;

				if (e.TurnedOn && !serviceOn)
				{
					e.Cancel = true;
				}

				// detect if disconnection is successful
				if (!e.TurnedOn && serviceOn)
				{
					e.Cancel = true;
				}
			}
			catch (OperationCanceledException)
			{
				e.Cancel = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show("[TBD]" + ex.Message, "[TBD] Unable to " +
					(e.TurnedOn ? "connect with " : "disconnect with ") + service.Name);

				e.Cancel = true;
			}
		}

		
	}
}

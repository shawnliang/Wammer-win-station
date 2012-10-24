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
	/// <summary>
	/// 
	/// </summary>
	public partial class ServiceImportControl : UserControl
	{
		private IConnectableService service;


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceImportControl"/> class.
		/// </summary>
		public ServiceImportControl(IConnectableService service)
		{
			InitializeComponent();

			this.service = service;
		}
		#endregion

		private void ServiceImportControl_Load(object sender, EventArgs e)
		{
			serviceItemControl1.ServiceName = service.Name;
			serviceItemControl1.ServiceIcon = service.Icon;
			serviceItemControl1.ServiceEnabled = service.Enable;

			serviceItemControl1.OnChange += serviceItemControl1_OnChange;
		}

		void serviceItemControl1_OnChange(object sender, Src.Control.ServiceConnectivityChangeEventArgs e)
		{
			try{
				service.Enable = e.TurnedOn;
			}
			catch(Exception ex)
			{
				MessageBox.Show("[TBD] " + ex.Message, "[TBD]Unable to change service...");
			}

		}
	}

	public interface IConnectableService
	{
		string Name { get; }
		bool Enable { get; set; }
		Image Icon { get; }
	}
}

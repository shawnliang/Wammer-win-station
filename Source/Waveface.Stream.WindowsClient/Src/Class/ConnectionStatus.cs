using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	public class ConnectionStatus
	{
		#region Static Var
        private static ConnectionStatus _instance;
        #endregion

		#region Var
		private HashSet<Device> _devices;
		private Timer _updateTimer;
		#endregion

		#region Public Static Property
		public static ConnectionStatus Instance
        { 
            get
            {
                return _instance ?? (_instance = new ConnectionStatus());
            }
        }
        #endregion


		#region Private Property
		private HashSet<Device> m_Devices
		{
			get
			{
				return _devices ?? (_devices = new HashSet<Device>());
			}
		}

		private Timer m_UpdateTimer
		{
			get
			{
				return _updateTimer ?? (_updateTimer = new Timer());
			}
		}
		#endregion


		#region Public Property
		public IEnumerable<Device> Devices 
		{
			get
			{
				return m_Devices;
			}
		}
		#endregion


		#region Event
		public event EventHandler<DevicesEventArgs> DeviceAdded;
		public event EventHandler<DevicesEventArgs> DeviceRemoved;
		#endregion


		#region Constructor
		private ConnectionStatus()
        {
			m_UpdateTimer.Interval = 1500;

			m_UpdateTimer.Tick += m_UpdateTimer_Tick;

			m_UpdateTimer.Enabled = true;
        }
        #endregion


		#region Private Method
		private void UpdateDevices()
		{
			var currentDevices = ConnectionCollection.Instance.FindAll().Select((item) => new Waveface.Stream.WindowsClient.Device(item.device.device_id, item.device.device_name)
			{
				RemainingBackUpCount = item.files_to_backup.HasValue ? item.files_to_backup.Value : 0
			});

			var addedDevices = currentDevices.Except(m_Devices).ToArray();
			var removedDevices = m_Devices.Except(currentDevices).ToArray();

			m_Devices.ExceptWith(removedDevices);
			m_Devices.UnionWith(addedDevices);

			if (addedDevices.Any())
				OnDeviceAdded(new DevicesEventArgs(addedDevices));

			if (removedDevices.Any())
				OnDeviceRemoved(new DevicesEventArgs(removedDevices));
		}
		#endregion

		#region Protected Method
		/// <summary>
		/// Raises the <see cref="E:DeviceAdded" /> event.
		/// </summary>
		/// <param name="e">The <see cref="DevicesEventArgs" /> instance containing the event data.</param>
		protected void OnDeviceAdded(DevicesEventArgs e)
		{
			this.RaiseEvent(DeviceAdded, e);
		}

		/// <summary>
		/// Raises the <see cref="E:DeviceRemoved" /> event.
		/// </summary>
		/// <param name="e">The <see cref="DevicesEventArgs" /> instance containing the event data.</param>
		protected void OnDeviceRemoved(DevicesEventArgs e)
		{
			this.RaiseEvent(DeviceRemoved, e);
		}
		#endregion

		#region Event Process
		private void m_UpdateTimer_Tick(object sender, EventArgs e)
		{
			UpdateDevices();
		}
		#endregion
	}
}

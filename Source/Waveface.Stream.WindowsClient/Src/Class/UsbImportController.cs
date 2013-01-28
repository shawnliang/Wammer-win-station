using Dolinay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;

namespace Waveface.Stream.WindowsClient
{
	public class UsbImportController
	{
		#region Static Var
        private static UsbImportController _instance;
        #endregion

		#region Var
		private UsbImportDialog _usbImportDialog;
		private Boolean _enabled;
		#endregion


		#region Public Static Property
		public static UsbImportController Instance
        { 
            get
            {
                return _instance ?? (_instance = new UsbImportController());
            }
        }
        #endregion


		#region Private Property
		private UsbImportDialog m_UsbImportDialog
		{
			get
			{
				return _usbImportDialog;
			}
			set
			{
				if (_usbImportDialog == value)
					return;

				if (_usbImportDialog != null)
					_usbImportDialog.Dispose();

				_usbImportDialog = value;
			}
		}
		#endregion


		#region Public Property
		public Boolean Enabled 
		{
			get
			{
				return _enabled;
			}
			set
			{
				if (_enabled == value)
					return;

				OnEnableStateChanging(EventArgs.Empty);
				_enabled = value;
				OnEnableStateChanged(EventArgs.Empty);
			}
		}
		#endregion


		#region Event
		public event EventHandler EnableStateChanging;
		public event EventHandler EnableStateChanged;
		#endregion


		#region Constructor
		private UsbImportController()
        {
			this.EnableStateChanged += UsbImportController_EnableStateChanged;
        }
        #endregion


		#region Protected Method
		protected void OnEnableStateChanging(EventArgs e)
		{
			this.RaiseEvent(EnableStateChanging, e);
		}

		protected void OnEnableStateChanged(EventArgs e)
		{
			this.RaiseEvent(EnableStateChanged, e);
		}
		#endregion


		#region Public Method
		public void StopDetect()
		{
			Enabled = false;
		}

		public void StartDetect()
		{
			Enabled = true;
		}
		#endregion


		#region Event Process
		void UsbImportController_EnableStateChanged(object sender, EventArgs e)
		{
			DriveDetector.Instance.DeviceArrived -= driveDetector_DeviceArrived;

			if (Enabled)
				DriveDetector.Instance.DeviceArrived += driveDetector_DeviceArrived;
		}

		private void driveDetector_DeviceArrived(object sender, DriveDetectorEventArgs e)
		{
			SynchronizationContextHelper.SendMainSyncContext(() =>
			{
				if (m_UsbImportDialog == null)
				{
					m_UsbImportDialog = new UsbImportDialog(e.Drive);
					m_UsbImportDialog.FormClosed += (s, arg) =>
					{
						_usbImportDialog = null;
					};
					m_UsbImportDialog.ShowDialog();
				}
			});
		}
		#endregion
	}
}

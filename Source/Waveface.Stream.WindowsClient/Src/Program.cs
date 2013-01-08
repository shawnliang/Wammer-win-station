using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.WindowsClient.Properties;
using Dolinay;
using Waveface.Stream.Core;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	internal static class Program
	{
		#region Const
		private const string CATEGORY_NAME = "Waveface Station";
		private const string UPSTREAM_RATE = "Upstream rate (bytes/sec)";
		private const string DWSTREAM_RATE = "Downstream rate (bytes/sec)";
		private const string UP_REMAINED_COUNT = "Atachement upload remained count";
		private const string DW_REMAINED_COUNT = "Atachement download remained count";
		#endregion


		#region Private Static Var
		private static NotifyIcon _notifyIcon;
		private static ContextMenuStrip _contextMenuStrip;
		private static StreamClient _client;
		private static System.Windows.Forms.Timer _timer;
		private static RecentDocumentWatcher recentDocWatcher = new RecentDocumentWatcher();

		private static DriveDetector driveDetector;
		private static UsbImportDialog usbImportDialog;

		private static Queue<float> _upRemainedCount = new Queue<float>();
		private static Queue<float> _downRemainedCount = new Queue<float>();
		private static Queue<float> _upSpeeds = new Queue<float>();
		private static Queue<float> _downSpeeds = new Queue<float>();

		private static PerformanceCounter m_DownRemainedCountCounter = new PerformanceCounter(CATEGORY_NAME, DW_REMAINED_COUNT, false);
		private static PerformanceCounter m_DownStreamRateCounter = new PerformanceCounter(CATEGORY_NAME, DWSTREAM_RATE, false);
		private static PerformanceCounter m_UpRemainedCountCounter = new PerformanceCounter(CATEGORY_NAME, UP_REMAINED_COUNT, false);
		private static PerformanceCounter m_UpStreamRateCounter = new PerformanceCounter(CATEGORY_NAME, UPSTREAM_RATE, false);


		private static Icon iconSyncing1 = Icon.FromHandle(Resources.stream_tray_syncing1.GetHicon());
		private static Icon iconSyncing2 = Icon.FromHandle(Resources.stream_tray_syncing2.GetHicon());
		private static Icon iconPaused = Icon.FromHandle(Resources.stream_tray_pause.GetHicon());
		private static Icon iconWarning = Icon.FromHandle(Resources.stream_tray_warn.GetHicon());
		private static Icon iconWorking = Icon.FromHandle(Resources.stream_tray_working.GetHicon());
		#endregion


		#region Private Static Property
		private static Mutex m_Mutex { get; set; }


		public static Boolean m_IsServiceRunning { get; set; }

		/// <summary>
		/// Gets the m_ notify icon.
		/// </summary>
		/// <value>The m_ notify icon.</value>
		private static NotifyIcon m_NotifyIcon
		{
			get
			{
				return _notifyIcon ?? (_notifyIcon = new NotifyIcon());
			}
		}

		/// <summary>
		/// Gets the m_ context menu strip.
		/// </summary>
		/// <value>The m_ context menu strip.</value>
		private static ContextMenuStrip m_ContextMenuStrip
		{
			get
			{
				return _contextMenuStrip ?? (_contextMenuStrip = new ContextMenuStrip());
			}
		}

		/// <summary>
		/// Gets the m_ client.
		/// </summary>
		/// <value>The m_ client.</value>
		private static StreamClient m_Client
		{
			get
			{
				return _client ?? (_client = StreamClient.Instance);
			}
		}

		private static System.Windows.Forms.Timer m_Timer
		{
			get { return _timer ?? (_timer = new System.Windows.Forms.Timer()); }
		}
		#endregion


		#region Private Static Method
		/// <summary>
		/// Mains the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		[STAThread]
		private static void Main(string[] args)
		{
			DebugInfo.ShowMethod();
			DebugInfo.ShowBugReportOnError();

			bool isFirstCreated;

			//Create a new mutex using specific mutex name
			m_Mutex = new Mutex(true, "StreamWindowsClient", out isFirstCreated);

			if (!isFirstCreated)
			{
				var currentProcess = Process.GetCurrentProcess();
				var processes = Process.GetProcessesByName(Assembly.GetExecutingAssembly().GetName().Name);

				if (processes.Any(process => process.Id != currentProcess.Id))
				{
					var handle = Win32Helper.FindWindow("NewClientTrayReceiver", null);

					if (handle == IntPtr.Zero)
						return;

					Win32Helper.SendMessage(handle, 0x401, IntPtr.Zero, IntPtr.Zero);
					return;
				}
				return;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			CommandLineHelper.ProcessCommandLineArgs(() => {}, new CommandLineCommand[] 
			{
				new CommandLineCommand("-RunMode",(parameters)=>
				{
					MainForm.Instance.IsDebugMode = parameters.FirstOrDefault().Equals("debug", StringComparison.CurrentCultureIgnoreCase);
				})
			});

			Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

			StreamClient.Instance.Logouted += Instance_Logouted;


			InitNotifyIcon();

			StationAPI.ResumeSync();
			m_IsServiceRunning = true;

			m_Timer.Interval = 500;
			m_Timer.Tick += (sender, e) => RefreshSyncingStatus();
			m_Timer.Start();


			if (StreamClient.Instance.IsLogined || ShowLoginDialog() == DialogResult.OK)
			{
				recentDocWatcher.FileTouched += recentDocWatcher_FileTouched;
				recentDocWatcher.Start();


				driveDetector = new DriveDetector();
				driveDetector.DeviceArrived += new DriveDetectorEventHandler(driveDetector_DeviceArrived);
				driveDetector.DeviceRemoved += new DriveDetectorEventHandler(driveDetector_DeviceRemoved);
				driveDetector.QueryRemove += new DriveDetectorEventHandler(driveDetector_QueryRemove);

				ShowMainWindow();
			}

			Application.Run();
		}

		static void driveDetector_QueryRemove(object sender, DriveDetectorEventArgs e)
		{
		}

		static void driveDetector_DeviceRemoved(object sender, DriveDetectorEventArgs e)
		{
		}

		static void driveDetector_DeviceArrived(object sender, DriveDetectorEventArgs e)
		{
			SynchronizationContextHelper.SendMainSyncContext(() =>
			{
				if (usbImportDialog == null)
				{
					usbImportDialog = new UsbImportDialog(
						e.Drive,
						StreamClient.Instance.LoginedUser.UserID,
						StreamClient.Instance.LoginedUser.SessionToken);
					usbImportDialog.FormClosed += (s, arg) =>
					{
						usbImportDialog = null;
					};
					usbImportDialog.Show();
				}
			});
		}

		static void recentDocWatcher_FileTouched(object sender, FileTouchEventArgs e)
		{
			try
			{
				StationAPI.AddMonitorFile(e.File, StreamClient.Instance.LoginedUser.UserID, StreamClient.Instance.LoginedUser.SessionToken);
			}
			catch (Exception ex)
			{
				MessageBox.Show("[TBD] Unable to tell station to monitor this file: " + e.File + "\r\n" + ex.Message);
			}
		}


		private static void GetSpeedAndUnit(float value, ref float speed, ref string unit)
		{
			var units = new string[] { "B/s", "KB/s", "MB/s" };
			var index = Array.IndexOf(units, unit);

			if (index == -1)
				index = 0;

			if (value > 1024)
			{
				value = value / 1024;
				speed = value;
				unit = units[index + 1];
				GetSpeedAndUnit(value, ref speed, ref unit);
				return;
			}

			speed = value;
			unit = units[index];
		}


		private static void RefreshSyncingStatus()
		{
			try
			{
				m_Timer.Stop();

				var iconText = Application.ProductName;
				var upRemainedCount = m_UpRemainedCountCounter.NextValue();
				var downloadRemainedCount = m_DownRemainedCountCounter.NextValue();

				if (_upRemainedCount.Count >= 10)
					_upRemainedCount.Dequeue();

				if (_downRemainedCount.Count >= 10)
					_downRemainedCount.Dequeue();

				_upRemainedCount.Enqueue(upRemainedCount);
				_downRemainedCount.Enqueue(downloadRemainedCount);

				SyncRange syncRange = null;

				if (StreamClient.Instance.IsLogined)
					syncRange = DriverCollection.Instance.FindOneById(StreamClient.Instance.LoginedUser.UserID).sync_range;

				if (syncRange == null)
					syncRange = new SyncRange();

				if (upRemainedCount > 0 || downloadRemainedCount > 0)
				{
					if (m_IsServiceRunning)
					{
						var upSpeed = m_UpStreamRateCounter.NextValue();
						var downloadSpeed = m_DownStreamRateCounter.NextValue();

						if (_upSpeeds.Count >= 5)
							_upSpeeds.Dequeue();
						_upSpeeds.Enqueue(upSpeed);

						if (_downSpeeds.Count >= 5)
							_downSpeeds.Dequeue();
						_downSpeeds.Enqueue(downloadSpeed);

						if (_upSpeeds.Count >= 0)
							upSpeed = _upSpeeds.Average();

						if (_downSpeeds.Count >= 0)
							downloadSpeed = _downSpeeds.Average();

						string upSpeedUnit = string.Empty;
						GetSpeedAndUnit(upSpeed, ref upSpeed, ref upSpeedUnit);

						string downloadSpeedUnit = string.Empty;
						GetSpeedAndUnit(downloadSpeed, ref downloadSpeed, ref downloadSpeedUnit);

						upSpeed = upRemainedCount == 0 ? 0 : upSpeed;
						downloadSpeed = downloadSpeed == 0 ? 0 : downloadSpeed;



						if (upRemainedCount > 0)
						{
							iconText = string.Format(Resources.INDICATOR_PATTERN,
													 iconText,
													 Environment.NewLine,
													 Resources.UPLOAD_INDICATOR,
													 (upRemainedCount > 999) ? "999+" : upRemainedCount.ToString(),
													 upSpeed,
													 upSpeedUnit);
						}

						if (downloadRemainedCount > 0)
						{
							iconText = string.Format(Resources.INDICATOR_PATTERN,
													 iconText,
													 Environment.NewLine,
													 Resources.DOWNLOAD_INDICATOR,
													 (downloadRemainedCount > 999) ? "999+" : downloadRemainedCount.ToString(),
													 downloadSpeed,
													 downloadSpeedUnit);
						}

						m_NotifyIcon.Icon = (m_NotifyIcon.Icon == iconSyncing1 ? iconSyncing2 : iconSyncing1);



						if (!string.IsNullOrEmpty(syncRange.GetUploadDownloadError()))
						{
							iconText = Application.ProductName + Environment.NewLine + Resources.SYNC_ERROR + syncRange.GetUploadDownloadError();
							m_NotifyIcon.Icon = iconWarning;
						}

					}
					else
					{
						m_NotifyIcon.Icon = iconPaused;
					}
				}
				else
				{
					m_NotifyIcon.Icon = iconWorking;

					if (!string.IsNullOrEmpty(syncRange.download_index_error))
					{
						iconText = Application.ProductName + Environment.NewLine + Resources.SYNC_ERROR + syncRange.download_index_error;
						if (iconText.Length > 127)
							iconText = iconText.Substring(0, 124) + "...";
						m_NotifyIcon.Icon = iconWarning;
					}
					else if (syncRange.syncing)
					{
						iconText = Resources.DOWNLOAD_INDEX;
						m_NotifyIcon.Icon = (m_NotifyIcon.Icon == iconSyncing1 ? iconSyncing2 : iconSyncing1);
					}
					
				}

				m_NotifyIcon.SetNotifyIconText(iconText);
			}
			finally
			{
				m_Timer.Start();
			}
		}

		private static DialogResult ShowMainWindow()
		{
			try
			{
				var dialog = MainForm.Instance;

				dialog.FormClosed -= dialog_FormClosed;
				dialog.FormClosed += dialog_FormClosed;

				var fileDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				var file = Path.Combine(fileDir, @"Web\index.html");
				
				dialog.Navigate(file);
				dialog.StartPosition = FormStartPosition.CenterParent;
				dialog.Activate();
				return dialog.ShowDialog();
			}
			catch (Exception)
			{
				return DialogResult.None;
			}
		}

		private static DialogResult ShowLoginDialog()
		{
			try
			{
				var dialog = LoginDialog.Instance;

				dialog.StartPosition = FormStartPosition.CenterParent;
				dialog.Activate();
				return dialog.ShowDialog(MainForm.Instance);
			}
			catch (Exception)
			{
				return DialogResult.None;
			}
		}

		private static DialogResult ShowSettingDialog()
		{
			try
			{
				var dialog = SettingDialog.Instance;

				dialog.StartPosition = FormStartPosition.CenterParent;
				dialog.Activate();
				return dialog.ShowDialog(MainForm.Instance);
			}
			catch (Exception)
			{
				return DialogResult.None;
			}
		}

		private static DialogResult ShowFileImportDialog()
		{
			try
			{
				var dialog = FileImportDialog.Instance;

				dialog.StartPosition = FormStartPosition.CenterParent;
				dialog.Activate();
				return dialog.ShowDialog(MainForm.Instance);
			}
			catch (Exception)
			{
				return DialogResult.None;
			}
		}

		private static DialogResult ShowContactUsDialog()
		{
			try
			{
				var dialog = ContactUsDialog.Instance;

				dialog.StartPosition = FormStartPosition.CenterParent;
				dialog.Activate();
				return dialog.ShowDialog(MainForm.Instance);
			}
			catch (Exception)
			{
				return DialogResult.None;
			}
		}

		/// <summary>
		/// Inits the notify icon.
		/// </summary>
		private static void InitNotifyIcon()
		{
			DebugInfo.ShowMethod();
			m_NotifyIcon.Text = Application.ProductName;
			m_NotifyIcon.Icon = Icon.FromHandle(Resources.stream_tray_init.GetHicon());
			m_NotifyIcon.ContextMenuStrip = m_ContextMenuStrip;
			m_NotifyIcon.Visible = true;

			m_NotifyIcon.MouseDown += new MouseEventHandler(m_NotifyIcon_MouseDown);
			m_NotifyIcon.DoubleClick += m_NotifyIcon_DoubleClick;
		}

		/// <summary>
		/// Inits the context menu strip items.
		/// </summary>
		private static void InitContextMenuStripItems()
		{
			DebugInfo.ShowMethod();

			m_ContextMenuStrip.Items.Clear();
			m_ContextMenuStrip.Items.Add("ResumeService", Resources.SERVICE_RESUME_MENU_ITEM, m_ContextMenuStrip_Resume_Click);
			m_ContextMenuStrip.Items.Add("PauseService", Resources.SERVICE_PAUSE_MENU_ITEM, m_ContextMenuStrip_Pause_Click);
			m_ContextMenuStrip.Items.Add("-");
			m_ContextMenuStrip.Items.Add("OpenStream", Resources.OPEN_STREAM_MENU_ITEM, m_ContextMenuStrip_Open_Click);
			m_ContextMenuStrip.Items.Add("Login", Resources.LOGIN_MENU_ITEM, m_ContextMenuStrip_Login_Click);
			m_ContextMenuStrip.Items.Add("-");
			m_ContextMenuStrip.Items.Add("Import", Resources.IMPORT_MENU_ITEM, m_ContextMenuStrip_Import_Click);
			m_ContextMenuStrip.Items.Add(Resources.SETTING_MENU_ITEM, m_ContextMenuStrip_Setting_Click);
			m_ContextMenuStrip.Items.Add("-");
			m_ContextMenuStrip.Items.Add(Resources.CONTACT_US_MENU_ITEM, m_ContextMenuStrip_ContactUs_Click);
			m_ContextMenuStrip.Items.Add(Resources.SERVICE_QUIT, m_ContextMenuStrip_Quit_Click);


		}

		private static void RunningService()
		{
			DebugInfo.ShowMethod();

			StationAPI.ResumeSync();
			m_IsServiceRunning = true;
		}

		private static void PauseService()
		{
			DebugInfo.ShowMethod();

			StationAPI.SuspendSync();
			m_IsServiceRunning = false;
		}

		private static void UpdateServiceMenuItemStatus()
		{
			m_ContextMenuStrip.Items["ResumeService"].Visible = !m_IsServiceRunning;
			m_ContextMenuStrip.Items["PauseService"].Visible = m_IsServiceRunning;
		}

		private static void UpdateLoginMenuItemStatus()
		{
			var isLogined = (StreamClient.Instance.LoginedUser != null && !string.IsNullOrEmpty(StreamClient.Instance.LoginedUser.SessionToken));
			m_ContextMenuStrip.Items["Login"].Visible = !isLogined;
			m_ContextMenuStrip.Items["OpenStream"].Visible = isLogined;
			m_ContextMenuStrip.Items["Import"].Visible = isLogined;
		}
		#endregion


		#region Event Process
		private static void m_ContextMenuStrip_Resume_Click(object sender, EventArgs e)
		{
			RunningService();
		}



		private static void m_ContextMenuStrip_Pause_Click(object sender, EventArgs e)
		{
			PauseService();
		}


		private static void m_ContextMenuStrip_Open_Click(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			ShowMainWindow();
		}

		private static void m_ContextMenuStrip_Login_Click(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			if (ShowLoginDialog() == DialogResult.OK)
			{
				ShowMainWindow();
			}
		}

		private static void m_ContextMenuStrip_Setting_Click(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			ShowSettingDialog();
		}

		private static void m_ContextMenuStrip_Import_Click(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			ShowFileImportDialog();
		}

		private static void m_ContextMenuStrip_ContactUs_Click(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			ShowContactUsDialog();
		}

		/// <summary>
		/// Handles the Click event of the m_ContextMenuStrip_Quit control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private static void m_ContextMenuStrip_Quit_Click(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			Application.Exit();
		}

		/// <summary>
		/// Handles the ApplicationExit event of the Application control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		static void Application_ApplicationExit(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			Settings.Default.Save();

			m_ContextMenuStrip.Dispose();
			m_NotifyIcon.Dispose();
		}


		/// <summary>
		/// Handles the MouseDown event of the m_NotifyIcon control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		static void m_NotifyIcon_MouseDown(object sender, MouseEventArgs e)
		{
			DebugInfo.ShowMethod();

			if (m_ContextMenuStrip.Items.Count == 0)
				InitContextMenuStripItems();

			UpdateServiceMenuItemStatus();
			UpdateLoginMenuItemStatus();
		}

		static void m_NotifyIcon_DoubleClick(object sender, EventArgs e)
		{
			if (StreamClient.Instance.IsLogined)
				ShowMainWindow();
			else
				ShowLoginDialog();
		}

		static void dialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			Settings.Default.CLIENT_WINDOW_STATE = MainForm.Instance.WindowState;
			Settings.Default.Save();
		}


		static void Instance_Logouted(object sender, EventArgs e)
		{
			SettingDialog.Instance.Dispose();
			AccountInfoForm.Instance.Dispose();
			MainForm.Instance.Dispose();

			if (ShowLoginDialog() == DialogResult.OK)
			{
				ShowMainWindow();
			}
		}
		#endregion
	}
}

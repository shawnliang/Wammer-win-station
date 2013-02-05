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
using CommandLine;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Net;
using fastJSON;
using log4net;
using log4net.Config;

namespace Waveface.Stream.WindowsClient
{
	internal static class Program
	{
		public const int WM_COPYDATA = 0x004A;

		#region Private Static Var
		private static MessageReceiver _messageReceiver;
		private static NotifyIcon _notifyIcon;
		private static ContextMenuStrip _contextMenuStrip;
		private static StreamClient _client;
		private static System.Windows.Forms.Timer _timer;

		private static int syncingIconIndex;
		private static Icon[] syncingIcons = new Icon[] 
		{
			Icon.FromHandle(Resources._1.GetHicon()),
			Icon.FromHandle(Resources._2.GetHicon()),
			Icon.FromHandle(Resources._3.GetHicon()),
			Icon.FromHandle(Resources._4.GetHicon()),
			Icon.FromHandle(Resources._5.GetHicon()),
			Icon.FromHandle(Resources._6.GetHicon()),
			Icon.FromHandle(Resources._7.GetHicon()),
			Icon.FromHandle(Resources._8.GetHicon()),
			Icon.FromHandle(Resources._9.GetHicon()),
			Icon.FromHandle(Resources._10.GetHicon()),
			Icon.FromHandle(Resources._11.GetHicon()),
			Icon.FromHandle(Resources._12.GetHicon()),
			Icon.FromHandle(Resources._13.GetHicon()),
		};

		private static Icon iconPaused = Icon.FromHandle(Resources.icon_16x16Pause.GetHicon());
		private static Icon iconWarning = Icon.FromHandle(Resources.icon_16x16Warning.GetHicon());
		private static Icon iconWorking = Icon.FromHandle(Resources.icon_16x16.GetHicon());
		#endregion


		#region Private Static Property
		private static MessageReceiver m_MessageReceiver
		{
			get
			{
				return _messageReceiver ?? (_messageReceiver = new MessageReceiver("StreamClientMessageReceiver", null));
			}
		}

		private static Mutex m_Mutex { get; set; }

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

			var options = new Options();
			ICommandLineParser parser = new CommandLineParser();
			if (!parser.ParseArguments(args, options))
				return;

			XmlConfigurator.Configure();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			MainForm.Instance.IsDebugMode = options.IsDebugMode;

			bool isFirstCreated;

			//Create a new mutex using specific mutex name
			m_Mutex = new Mutex(true, "StreamWindowsClient", out isFirstCreated);

			if (!isFirstCreated)
			{
				var currentProcess = Process.GetCurrentProcess();
				var processes = Process.GetProcessesByName(Assembly.GetExecutingAssembly().GetName().Name);

				if (processes.Any(process => process.Id != currentProcess.Id))
				{
					var handle = Win32Helper.FindWindow("StreamClientMessageReceiver", null);

					if (handle == IntPtr.Zero)
						return;

					Win32Helper.SendMessage(handle, 0x401, IntPtr.Zero, IntPtr.Zero);

					if (options.Imports != null && options.Imports.Any())
					{
						var msg = new ImportMsg { files = options.Imports.ToList() };

						(new SendMessageHelper()).SendMessage("StreamClientMessageReceiver", null, 0x402, msg.ToFastJSON(), true);
					}
				}
				return;
			}

			var splashScreen = new SplashScreen();

			splashScreen
				.AppendProcess("Waiting database service...", waitUntilMongodbReady)
				.AppendProcess("Starting AOStream...", () =>
				{
					StationServiceProxy.Instance.StartService();

					waitUntilStationAPIReady();
				})
				.ShowDialog();


			if (options.Imports != null && options.Imports.Any())
			{
				ImportFileAndFolders(options.Imports);
			}
			
			
			m_MessageReceiver.WndProc += m_MessageReceiver_WndProc;
			
			Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

			StreamClient.Instance.Logouted += Instance_Logouted;
			StreamClient.Instance.Logined += Instance_Logined;

			InitNotifyIcon();

			SyncStatus.IsServiceRunning = true;

			m_Timer.Interval = 500;
			m_Timer.Tick += (sender, e) => RefreshSyncingStatus();
			m_Timer.Start();


			if (StreamClient.Instance.IsLogined || ShowLoginDialog() == DialogResult.OK)
			{
				RecentDocumentWatcher.Instance.FileTouched += recentDocWatcher_FileTouched;
				RecentDocumentWatcher.Instance.Start();

				UsbImportController.Instance.StartDetect();

				if (!MainForm.Instance.IsDebugMode)
					ShowPreferencesDialog();
				else
					ShowMainWindow();
			}

			Application.Run();
		}

		private static void waitUntilStationAPIReady()
		{
			var begin = DateTime.Now;

			while (!StationAPI.PingFuncAndMgnt())
			{
				Thread.Sleep(500);

				var dur = (DateTime.Now - begin).TotalSeconds;

				if (dur > 30.0)
					throw new StationServiceNotReadyException("Station not ready in 30 seconds");
			}
		}

		private static void waitUntilMongodbReady()
		{
			var begin = DateTime.Now;

			while (!Waveface.Common.MongoDbHelper.IsMongoDBReady("127.0.0.1", 10319))
			{
				var dur = (DateTime.Now - begin).TotalSeconds;

				if (dur > 30.0)
					throw new MongoDBNotReadyException("MongoDB is not ready in 30 secs");
			}
		}


		private static void ImportFileAndFolders(IEnumerable<string> fileAndFolders)
		{
			if (!StreamClient.Instance.IsLogined)
				return;

			(new PhotoSearch()).ImportToStationAsync(fileAndFolders, StreamClient.Instance.LoginedUser.SessionToken);
		}

		static void m_MessageReceiver_WndProc(object sender, MessageEventArgs e)
		{
			switch ((int)e.Message)
			{
				case 0x401:
					{
                        if (StreamClient.Instance.IsLogined || ShowLoginDialog() == DialogResult.OK)
                        {
                            if (!MainForm.Instance.IsDebugMode)
                                ShowPreferencesDialog();
                            else
                                ShowMainWindow();
                        }
					}
					break;

				case WM_COPYDATA:
					{
						switch ((int)e.wParam)
						{
							case 0x402:
                                if (!StreamClient.Instance.IsLogined)
                                    return;
								var cd = (CopyDataStruct)Marshal.PtrToStructure(e.lParam, typeof(CopyDataStruct));

								var jsonstr = Marshal.PtrToStringAuto(cd.lpData, cd.cbData / 2);
								ImportFileAndFolders(fastJSON.JSON.Instance.ToObject<ImportMsg>(jsonstr).files);
								break;
						}
					}
					break;
			}
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


		private static void RefreshSyncingStatus()
		{
			try
			{
				m_Timer.Stop();

				var upRemainedCount = SyncStatus.UploadRemainedCount;
				var downloadRemainedCount = SyncStatus.DownloadRemainedCount;

				SyncRange syncRange = getSyncRange();

				if (upRemainedCount > 0 || downloadRemainedCount > 0)
				{
					if (SyncStatus.IsServiceRunning)
					{
						m_NotifyIcon.Icon = syncingIcons[syncingIconIndex ++ % syncingIcons.Length];

						if (!string.IsNullOrEmpty(syncRange.GetUploadDownloadError()))
						{
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
					m_NotifyIcon.Icon = (SyncStatus.IsServiceRunning) ? iconWorking : iconPaused;

					if (!string.IsNullOrEmpty(syncRange.download_index_error))
					{
						m_NotifyIcon.Icon = iconWarning;
					}
					else if (syncRange.syncing)
					{
						m_NotifyIcon.Icon = syncingIcons[syncingIconIndex++ % syncingIcons.Length];
					}

				}

				var syncStatus = (StreamClient.Instance.IsLogined) ? ImportStatus.Lookup(StreamClient.Instance.LoginedUser.UserID).Description : string.Empty;

				if (string.IsNullOrEmpty(syncStatus))
					syncStatus = SyncStatus.GetSyncDescription();


				var iconText = string.Format("{0}{1}{2}",
					Application.ProductName,
					Environment.NewLine,
					syncStatus);

				m_NotifyIcon.SetNotifyIconText(iconText);
			}
			finally
			{
				m_Timer.Start();
			}
		}

		private static SyncRange getSyncRange()
		{
			try
			{
				SyncRange syncRange = null;

				if (StreamClient.Instance.IsLogined)
				{
					var user = DriverCollection.Instance.FindOneById(StreamClient.Instance.LoginedUser.UserID);
					if (user != null)
						syncRange = user.sync_range;
				}

				if (syncRange == null)
					syncRange = new SyncRange();


				return syncRange;
			}
			catch
			{
				return new SyncRange();
			}
		}

		private static DialogResult ShowMainWindow()
		{
			try
			{
				var dialog = MainForm.Instance;

				if(!dialog.IsDebugMode)
					return DialogResult.OK;

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

		private static DialogResult ShowPreferencesDialog()
		{
			try
			{
				var dialog = PreferencesDialog.Instance;

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
			m_NotifyIcon.Icon = iconWorking;
			m_NotifyIcon.ContextMenuStrip = m_ContextMenuStrip;
			m_NotifyIcon.Visible = true;

			m_NotifyIcon.MouseDown += m_NotifyIcon_MouseDown;
			m_NotifyIcon.MouseDoubleClick += m_NotifyIcon_MouseDoubleClick;
		}


		/// <summary>
		/// Inits the context menu strip items.
		/// </summary>
		private static void InitContextMenuStripItems()
		{
			DebugInfo.ShowMethod();
			
			m_ContextMenuStrip.Items.Clear();

			var preferencesMenuItem = m_ContextMenuStrip.Items.Add("Preferences", Resources.CONTROL_PREFERENCES_MENU_ITEM, m_ContextMenuStrip_Preferences_Click);
			var loginMenuItem = m_ContextMenuStrip.Items.Add("Login", Resources.LOGIN_MENU_ITEM, m_ContextMenuStrip_Login_Click);

			m_ContextMenuStrip.Items.Add("OpenStream", Resources.OPEN_STREAM_MENU_ITEM, m_ContextMenuStrip_Open_Click);
			m_ContextMenuStrip.Items.Add("Seperator", "-", null);

			m_ContextMenuStrip.Items.Add("ResumeService", Resources.SERVICE_RESUME_MENU_ITEM, m_ContextMenuStrip_Resume_Click);
			m_ContextMenuStrip.Items.Add("PauseService", Resources.SERVICE_PAUSE_MENU_ITEM, m_ContextMenuStrip_Pause_Click);
			m_ContextMenuStrip.Items.Add("Import", Resources.IMPORT_MENU_ITEM, m_ContextMenuStrip_Import_Click);
			m_ContextMenuStrip.Items.Add("SyncStatusSeperator", "-", null);

			m_ContextMenuStrip.Items.Add("-", null);

			m_ContextMenuStrip.Items.Add(Resources.HELP_CENTER_MENU_ITEM, null);
			m_ContextMenuStrip.Items.Add(Resources.GET_APPS_MENU_ITEM, null);
			m_ContextMenuStrip.Items.Add(Resources.CHROME_EXTENSION_MENU_ITEM, null);
			m_ContextMenuStrip.Items.Add(Resources.CONTACT_US_MENU_ITEM, m_ContextMenuStrip_ContactUs_Click);
			m_ContextMenuStrip.Items.Add("-", null);

			m_ContextMenuStrip.Items.Add(Resources.SERVICE_QUIT, m_ContextMenuStrip_Quit_Click);

			preferencesMenuItem.Font = new Font(preferencesMenuItem.Font, FontStyle.Bold | preferencesMenuItem.Font.Style);
			loginMenuItem.Font = preferencesMenuItem.Font;
		}

		private static void RunningService()
		{
			DebugInfo.ShowMethod();

			StationAPI.ResumeSync();
			SyncStatus.IsServiceRunning = true;
		}

		private static void PauseService()
		{
			DebugInfo.ShowMethod();

			StationAPI.SuspendSync();
			SyncStatus.IsServiceRunning = false;
		}

		private static void UpdateServiceMenuItemStatus()
		{
			var isLogined = (StreamClient.Instance.LoginedUser != null && !string.IsNullOrEmpty(StreamClient.Instance.LoginedUser.SessionToken));
			m_ContextMenuStrip.Items["ResumeService"].Visible = isLogined && !SyncStatus.IsServiceRunning;
			m_ContextMenuStrip.Items["PauseService"].Visible = isLogined && SyncStatus.IsServiceRunning;
		}

		private static void UpdateLoginMenuItemStatus()
		{
			var isLogined = (StreamClient.Instance.LoginedUser != null && !string.IsNullOrEmpty(StreamClient.Instance.LoginedUser.SessionToken));
			m_ContextMenuStrip.Items["Login"].Visible = !isLogined;

			m_ContextMenuStrip.Items["OpenStream"].Visible = MainForm.Instance.IsDebugMode && isLogined;
			m_ContextMenuStrip.Items["Seperator"].Visible = isLogined;

			m_ContextMenuStrip.Items["Preferences"].Visible = isLogined;
			m_ContextMenuStrip.Items["Import"].Visible = isLogined;
		}

		private static void UpdateTrayMenuSyncStatus()
		{
			var insertIndex = m_ContextMenuStrip.Items.IndexOfKey("SyncStatusSeperator") + 1;

			while (!(m_ContextMenuStrip.Items[insertIndex] is ToolStripSeparator))
			{
				m_ContextMenuStrip.Items.RemoveAt(insertIndex);
			}

			var syncStatus = (StreamClient.Instance.IsLogined)? ImportStatus.Lookup(StreamClient.Instance.LoginedUser.UserID).Description: string.Empty;

			if (string.IsNullOrEmpty(syncStatus))
				syncStatus = SyncStatus.GetSyncDescription();

			if (string.IsNullOrEmpty(syncStatus))
				return;

			foreach (var status in syncStatus.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
			{
				m_ContextMenuStrip.Items.Insert(insertIndex++, new ToolStripMenuItem(status) 
				{
					Enabled = false
				});
			}
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
				if (!MainForm.Instance.IsDebugMode)
					ShowPreferencesDialog();
				else
					ShowMainWindow();
			}
		}

		private static void m_ContextMenuStrip_Preferences_Click(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			ShowPreferencesDialog();
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

			StationServiceProxy.Instance.StopService();

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
			UpdateTrayMenuSyncStatus();
		}

		static void m_NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			if (StreamClient.Instance.IsLogined || ShowLoginDialog() == DialogResult.OK)
			{
				if (!MainForm.Instance.IsDebugMode)
					ShowPreferencesDialog();
				else
					ShowMainWindow();
			}
		}

		static void dialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			Settings.Default.CLIENT_WINDOW_STATE = MainForm.Instance.WindowState;
			Settings.Default.Save();
		}


		static void Instance_Logined(object sender, ClientFramework.LoginedEventArgs e)
		{
			RecentDocumentWatcher.Instance.Start();
			UsbImportController.Instance.StartDetect();
		}

		static void Instance_Logouted(object sender, EventArgs e)
		{
			RecentDocumentWatcher.Instance.Stop();
			UsbImportController.Instance.StopDetect();

			PreferencesDialog.Instance.Dispose();
			MainForm.Instance.Dispose();

			if (ShowLoginDialog() == DialogResult.OK)
			{
				if (!MainForm.Instance.IsDebugMode)
					ShowPreferencesDialog();
				else
					ShowMainWindow();
			}
		}
		#endregion
	}
}

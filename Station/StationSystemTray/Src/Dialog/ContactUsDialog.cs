using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace StationSystemTray
{
	public partial class ContactUsDialog : Form
	{
		private ProcessingDialog m_inProgressDialog;
		private BackgroundWorker m_logCollector;

		public ContactUsDialog()
		{
			InitializeComponent();
			m_logCollector = new BackgroundWorker();
			m_logCollector.DoWork += logCollecBgWorker_DoWork;
			m_logCollector.RunWorkerCompleted += logCollecBgWorker_RunWorkerCompleted;
		}

		private void emailLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("mailto:contact@waveface.com");
		}

		private void twitterLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://twitter.com/#!/waveface");
		}

		private void fbLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://www.facebook.com/waveface");
		}

		private void collectLogsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			m_logCollector.RunWorkerAsync();
			m_inProgressDialog = new ProcessingDialog();
			m_inProgressDialog.StartPosition = FormStartPosition.CenterParent;
			m_inProgressDialog.Text = "Collecting logs...";
			m_inProgressDialog.ShowDialog();
		}

		void logCollecBgWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var wavefaceDir = Path.Combine(appdata, "waveface");
			var supportDir = Path.Combine(wavefaceDir, "support");
			var installDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var cacheDir = Path.Combine(wavefaceDir, "cache");
			var stationLogDir = Path.Combine(installDir, "log");

			makeEmptyDir(supportDir);
			copyLogFiles(stationLogDir, "*.*", supportDir);
			copyLogFiles(wavefaceDir, "*.log", supportDir);
			copyLogFiles(cacheDir, "*.dat", supportDir);
			copyLogFiles(cacheDir, "*.txt", supportDir);
			copyLogFiles(wavefaceDir, "trayIcon.*", supportDir);
			dumpMongoDB(supportDir, installDir);
			dumpRegistry(supportDir);

			e.Result = supportDir;
		}

		private static void dumpRegistry(string supportDir)
		{
			try
			{
				string arg;
				if (IntPtr.Size == 8)
					arg = string.Format("export HKLM\\Software\\Wow6432Node\\Wammer \"{0}\"", Path.Combine(supportDir, "Stream.reg"));
				else
					arg = string.Format("export HKLM\\Software\\Wammer \"{0}\"", Path.Combine(supportDir, "Stream.reg"));

				var p = Process.Start("reg.exe", arg);
				p.WaitForExit();
			}
			catch
			{
			}
		}

		void logCollecBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			try
			{
				if (e.Cancelled)
					return;

				if (e.Error != null)
				{
					MessageBox.Show(e.Error.Message);
					return;
				}

				var supportDir = (string)e.Result;
				Process.Start(supportDir);
			}
			finally
			{
				m_inProgressDialog.Close();
			}
		}

		private static void dumpMongoDB(string supportDir, string installDir)
		{
			try
			{
				var mongodump = Path.Combine(installDir, @"MongoDB\mongodump.exe");
				var dumpDest = Path.Combine(supportDir, "mongo");
				if (File.Exists(mongodump))
				{
					var args = string.Format("--port 10319 --forceTableScan --out \"{0}\" --db wammer", dumpDest);
					var info = new ProcessStartInfo(mongodump, args)
					{
						CreateNoWindow = true,
						WindowStyle = ProcessWindowStyle.Hidden, 
					};
					var p = Process.Start(info);
					p.WaitForExit();
				}
			}
			catch
			{
			}
		}

		private static void copyLogFiles(string srcDir, string logPattern, string supportDir)
		{
			var files = Directory.GetFiles(srcDir, logPattern);
			Array.ForEach(files, filePath => {
				var src = filePath;
				var dest = Path.Combine(supportDir, Path.GetFileName(filePath));

				using (var srcFile = new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					srcFile.WriteTo(dest, 64 * 1024);
				}
			});
		}

		private static void makeEmptyDir(string supportDir)
		{
			if (Directory.Exists(supportDir))
				Directory.Delete(supportDir, true);

			Directory.CreateDirectory(supportDir);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}

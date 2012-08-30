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
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

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
			try
			{
				Process.Start("mailto:contact@waveface.com");
			}
			catch
			{
				// mail client is not define or is not opened successfully
			}
		}

		private void twitterLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Process.Start("https://twitter.com/#!/waveface");
			}
			catch
			{
				// web browser is not define or is not opened successfully
			}
		}

		private void fbLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Process.Start("https://www.facebook.com/waveface");
			}
			catch
			{
				// web browser is not define or is not opened successfully
			}
		}

		private void collectLogsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			m_logCollector.RunWorkerAsync();
			m_inProgressDialog = new ProcessingDialog();
			m_inProgressDialog.StartPosition = FormStartPosition.CenterParent;
			m_inProgressDialog.Text = StationSystemTray.Properties.Resources.CollectingLogs;
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

			packLogsToTarGzFile(wavefaceDir, supportDir, "support.tar.gz");

			e.Result = supportDir;
		}

		private static void packLogsToTarGzFile(string wavefaceDir, string supportDir, string tarGzFileName)
		{
			var tarGzipFile = Path.Combine(wavefaceDir, tarGzFileName);
			using (var tarGzipStream = File.Open(tarGzipFile, FileMode.Create, FileAccess.Write))
			{
				var gzip = new GZipOutputStream(tarGzipStream);
				var tar = TarArchive.CreateOutputTarArchive(gzip);
				tar.RootPath = wavefaceDir;
				tar.WriteEntry(TarEntry.CreateEntryFromFile(supportDir), true);
				tar.RootPath = wavefaceDir;
				tar.Close();
			}
			makeEmptyDir(supportDir);
			File.Move(tarGzipFile, Path.Combine(supportDir, tarGzFileName));
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
			try
			{
				var files = Directory.GetFiles(srcDir, logPattern);
				Array.ForEach(files, filePath =>
				{
					var src = filePath;
					var dest = Path.Combine(supportDir, Path.GetFileName(filePath));

					using (var srcFile = new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						srcFile.WriteTo(dest, 64 * 1024);
					}
				});
			}
			catch
			{

			}
		}

		private static void makeEmptyDir(string supportDir)
		{
			if (!Directory.Exists(supportDir))
			{
				Directory.CreateDirectory(supportDir);
			}
			else
			{
				foreach (var entry in Directory.GetFileSystemEntries(supportDir))
				{
					if (Directory.Exists(entry))
						Directory.Delete(entry, true);
					else
						File.Delete(entry);
				}
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}

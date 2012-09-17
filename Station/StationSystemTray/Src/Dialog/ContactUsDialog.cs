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
using System.IO.Packaging;

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
			m_inProgressDialog.ProgressStyle = ProgressBarStyle.Marquee;
			m_inProgressDialog.ProcessMessage = StationSystemTray.Properties.Resources.CollectingLogs;
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

			using (var zip = ZipPackage.Open(Path.Combine(supportDir, "support.zip"), FileMode.Create))
			{
				addDirToZip(stationLogDir, "*.*", zip);
				addDirToZip(wavefaceDir, "*.log", zip);
				addDirToZip(cacheDir, "*.dat", zip);
				addDirToZip(cacheDir, "*.txt", zip);
				addDirToZip(wavefaceDir, "trayIcon.*", zip);

				var dumpDir = dumpMongoDB(supportDir, installDir);
				if (dumpDir != null)
				{
					addDirToZip(dumpDir, "*.*", zip);
					try
					{
						Directory.Delete(dumpDir, true);
					}
					catch
					{
					}
				}

				var regFile = dumpRegistry(supportDir);
				if (regFile != null)
				{
					addFileToZip(regFile, zip);
					try
					{
						File.Delete(regFile);
					}
					catch
					{
					}
				}
			}

			e.Result = supportDir;
		}

		private static string dumpRegistry(string supportDir)
		{
			try
			{
				string arg;
				string outputFile = Path.Combine(supportDir, "Stream.reg");

				if (IntPtr.Size == 8)
					arg = string.Format("export HKLM\\Software\\Wow6432Node\\Wammer \"{0}\"", outputFile);
				else
					arg = string.Format("export HKLM\\Software\\Wammer \"{0}\"", outputFile);

				var p = Process.Start("reg.exe", arg);
				p.WaitForExit();

				return outputFile;
			}
			catch
			{
				return null;
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

		private static string dumpMongoDB(string supportDir, string installDir)
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

					return dumpDest;
				}
				else
					return null;
			}
			catch
			{
				return null;
			}
		}

		private static void addDirToZip(string srcDir, string logPattern, Package zip)
		{
			try
			{
				var files = Directory.GetFileSystemEntries(srcDir, logPattern);
				Array.ForEach(files, filePath =>
				{
					if (Directory.Exists(filePath))
					{
						addDirToZip(filePath, logPattern, zip);
					}
					else
					{
						addFileToZip(filePath, zip);
					}
				});
			}
			catch
			{
			}
		}

		private static void addFileToZip(string filePath, Package zip)
		{
			try
			{
				var fileName = Path.GetFileName(filePath);

				var partUri = PackUriHelper.CreatePartUri(new Uri(fileName, UriKind.Relative));
				var part = zip.CreatePart(partUri, "application/octet-stream", CompressionOption.Normal);

				using (var srcFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					srcFile.WriteTo(part.GetStream());
				}
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

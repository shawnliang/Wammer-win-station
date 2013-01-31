using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Windows.Forms;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public partial class ContactUsDialog : Form
	{
		#region Static Var
		private static ContactUsDialog _instance;
		#endregion

		#region Var
		private ProcessingDialog m_inProgressDialog;
		private BackgroundWorker m_logCollector;
		#endregion

		#region Public Static Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static ContactUsDialog Instance
		{
			get
			{
				return _instance ?? (_instance = new ContactUsDialog());
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Prevents a default instance of the <see cref="ContactUsDialog" /> class from being created.
		/// </summary>
		private ContactUsDialog()
		{
			InitializeComponent();
			m_logCollector = new BackgroundWorker();
			m_logCollector.DoWork += logCollecBgWorker_DoWork;
			m_logCollector.RunWorkerCompleted += logCollecBgWorker_RunWorkerCompleted;
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Dumps the registry.
		/// </summary>
		/// <param name="supportDir">The support dir.</param>
		/// <returns></returns>
		private string DumpRegistry(string supportDir)
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

		/// <summary>
		/// Dumps the mongo DB.
		/// </summary>
		/// <param name="supportDir">The support dir.</param>
		/// <param name="installDir">The install dir.</param>
		/// <returns></returns>
		private string DumpMongoDB(string supportDir, string installDir)
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

		/// <summary>
		/// Adds the dir to zip.
		/// </summary>
		/// <param name="srcDir">The SRC dir.</param>
		/// <param name="logPattern">The log pattern.</param>
		/// <param name="zip">The zip.</param>
		private void AddDirToZip(string srcDir, string logPattern, Package zip)
		{
			try
			{
				var files = Directory.GetFileSystemEntries(srcDir, logPattern);
				Array.ForEach(files, filePath =>
				{
					if (Directory.Exists(filePath))
					{
						AddDirToZip(filePath, logPattern, zip);
					}
					else
					{
						AddFileToZip(filePath, zip);
					}
				});
			}
			catch
			{
			}
		}

		/// <summary>
		/// Adds the file to zip.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="zip">The zip.</param>
		private void AddFileToZip(string filePath, Package zip)
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

		/// <summary>
		/// Makes the empty dir.
		/// </summary>
		/// <param name="supportDir">The support dir.</param>
		private void MakeEmptyDir(string supportDir)
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
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the LinkClicked event of the emailLink control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="LinkLabelLinkClickedEventArgs" /> instance containing the event data.</param>
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

		/// <summary>
		/// Handles the LinkClicked event of the twitterLink control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="LinkLabelLinkClickedEventArgs" /> instance containing the event data.</param>
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

		/// <summary>
		/// Handles the LinkClicked event of the fbLink control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="LinkLabelLinkClickedEventArgs" /> instance containing the event data.</param>
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

		/// <summary>
		/// Handles the LinkClicked event of the collectLogsLink control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="LinkLabelLinkClickedEventArgs" /> instance containing the event data.</param>
		private void collectLogsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			m_logCollector.RunWorkerAsync();
			m_inProgressDialog = new ProcessingDialog();
			m_inProgressDialog.StartPosition = FormStartPosition.CenterParent;
			m_inProgressDialog.ProgressStyle = ProgressBarStyle.Marquee;
			m_inProgressDialog.ProcessMessage = Resources.COLLECTING_LOGS;
			m_inProgressDialog.ShowDialog();
		}

		/// <summary>
		/// Handles the DoWork event of the logCollecBgWorker control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DoWorkEventArgs" /> instance containing the event data.</param>
		void logCollecBgWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var streamDir = Path.Combine(appdata, @"waveface\AOStream");
			var supportDir = Path.Combine(streamDir, "support");
			var installDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var stationLogDir = Path.Combine(installDir, "log");

			MakeEmptyDir(supportDir);

			using (var zip = ZipPackage.Open(Path.Combine(supportDir, "support.zip"), FileMode.Create))
			{
				AddDirToZip(stationLogDir, "*.*", zip);
				AddDirToZip(streamDir, "*.*", zip);

				var dumpDir = DumpMongoDB(supportDir, installDir);
				if (dumpDir != null)
				{
					AddDirToZip(dumpDir, "*.*", zip);
					try
					{
						Directory.Delete(dumpDir, true);
					}
					catch
					{
					}
				}

				var regFile = DumpRegistry(supportDir);
				if (regFile != null)
				{
					AddFileToZip(regFile, zip);
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


		/// <summary>
		/// Handles the RunWorkerCompleted event of the logCollecBgWorker control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RunWorkerCompletedEventArgs" /> instance containing the event data.</param>
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

		/// <summary>
		/// Handles the Click event of the btnOK control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void btnOK_Click(object sender, EventArgs e)
		{
			Close();
		}
		#endregion
	}
}

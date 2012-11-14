using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Waveface.Stream.WindowsClient
{
	/// <summary>
	/// 
	/// </summary>
	public partial class AutoImportDialog : Form
	{
		#region Const
		const string PICASA_DB_RELATIVED_STORAGE_PATH = @"Google\Picasa2\db3";
		const string ALBUM_PATH_PMP_FILENAME = "albumdata_filename.pmp";
		#endregion

		#region Var
		private SynchronizationContext _syncContext = SynchronizationContext.Current;
		private string _picasaDBStoragePath;
		private string _albumPathPMPFileName;
		#endregion

		#region Private Property
		/// <summary>
		/// Gets the m_ sync context.
		/// </summary>
		/// <value>The m_ sync context.</value>
		private SynchronizationContext m_SyncContext
		{
			get
			{
				return _syncContext;
			}
		}

		/// <summary>
		/// Gets the m_ picasa DB storage path.
		/// </summary>
		/// <value>The m_ picasa DB storage path.</value>
		private string m_PicasaDBStoragePath
		{
			get
			{
				return _picasaDBStoragePath ??
					(_picasaDBStoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PICASA_DB_RELATIVED_STORAGE_PATH));
			}
		}

		/// <summary>
		/// Gets the name of the m_ album path PMP file.
		/// </summary>
		/// <value>The name of the m_ album path PMP file.</value>
		private string m_AlbumPathPMPFileName
		{
			get
			{
				return _albumPathPMPFileName ??
					(_albumPathPMPFileName = Path.Combine(m_PicasaDBStoragePath, ALBUM_PATH_PMP_FILENAME));
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="AutoImportDialog"/> class.
		/// </summary>
		public AutoImportDialog()
		{
			InitializeComponent();
		} 
		#endregion


		#region Private Method
		/// <summary>
		/// Sends the sync context.
		/// </summary>
		/// <param name="target">The target.</param>
		private void SendSyncContext(Action target)
		{
			m_SyncContext.Send((obj) => target(), null);
		}

		/// <summary>
		/// Sends the sync context.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">The target.</param>
		/// <param name="o">The o.</param>
		private void SendSyncContext<T>(Action<T> target, Object o)
		{
			m_SyncContext.Send((obj) => target((T)obj), o);
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the Click event of the btnImport control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnImport_Click(object sender, EventArgs e)
		{
			MethodInvoker mi = new MethodInvoker(() =>
			{
				var importer = new AutoImporter();
				importer.Import(ContentProviderType.Libraries);
			});

			btnImport.Enabled = false;
			button1.Enabled = false;
			button2.Enabled = false;
			progressBar1.Visible = true;
			mi.BeginInvoke((result) =>
			{
				SendSyncContext(() =>
				{
					this.DialogResult = DialogResult.OK;
					progressBar1.Visible = false;
				});
			}, null);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
				return;

			MethodInvoker mi = new MethodInvoker(() =>
			{
				var importer = new AutoImporter();
				importer.Import(folderBrowserDialog1.SelectedPath);
			});

			btnImport.Enabled = false;
			button1.Enabled = false;
			button2.Enabled = false;
			progressBar1.Visible = true;
			mi.BeginInvoke((result) =>
			{
				SendSyncContext(() =>
				{
					this.DialogResult = DialogResult.OK;
					progressBar1.Visible = false;
				});
			}, null);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			MethodInvoker mi = new MethodInvoker(() =>
			{
				var importer = new AutoImporter();
				importer.Import(ContentProviderType.Picasa);
			});

			btnImport.Enabled = false;
			button1.Enabled = false;
			button2.Enabled = false;
			progressBar1.Visible = true;
			mi.BeginInvoke((result) =>
			{
				SendSyncContext(() =>
				{
					this.DialogResult = DialogResult.OK;
					progressBar1.Visible = false;
				});
			}, null);
		}

		/// <summary>
		/// Handles the Load event of the AutoImportDialog control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void AutoImportDialog_Load(object sender, EventArgs e)
		{
			button2.Enabled = File.Exists(m_AlbumPathPMPFileName);
		}
		#endregion
	}
}

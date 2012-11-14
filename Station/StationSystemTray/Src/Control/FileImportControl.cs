using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace StationSystemTray
{
	public partial class FileImportControl : StepPageControl
	{
		private IPhotoSearch photoSearch;
		private SynchronizationContext mainSyncCtx;
		private CheckBox checkBox1;
		private HidableProgressingDialog _processDialog;


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="FileImportControl"/> class.
		/// </summary>
		public FileImportControl(IPhotoSearch search, SynchronizationContext mainSyncCtx)
		{
			InitializeComponent();
			this.photoSearch = search;
			this.mainSyncCtx = mainSyncCtx;
			this.PageTitle = "Import from folders";
		} 
		#endregion

		private HidableProgressingDialog m_ProcessingDialog
		{
			get
			{
				return _processDialog ?? (_processDialog = new HidableProgressingDialog
				{
					ProgressStyle = ProgressBarStyle.Marquee,
					StartPosition = FormStartPosition.CenterParent,
					Hidable = false
				});
			}
		}


		/// <summary>
		/// Processes the file import step.
		/// </summary>
		public override void OnEnteringStep(WizardParameters parameters)
		{
			base.OnEnteringStep(parameters);
			
			ClearInterestedPaths();
			AddInterestedPaths(photoSearch.InterestedPaths);
		}

		public override void OnLeavingStep(WizardParameters parameters)
		{
			base.OnLeavingStep(parameters);

			ImportSelectedPaths((string)parameters.Get("session_token"));
		}

		/// <summary>
		/// Imports the selected paths.
		/// </summary>
		private void ImportSelectedPaths(string session_token)
		{
			var selectedPaths = GetSelectedPaths();

			if (selectedPaths.Count() == 0)
				return;

			photoSearch.FileImported -= photoSearch_FileImported;
			photoSearch.FileImported += photoSearch_FileImported;
			photoSearch.MetadataUploaded -= photoSearch_MetadataUploaded;
			photoSearch.MetadataUploaded += photoSearch_MetadataUploaded; 
			photoSearch.ImportDone -= photoSearch_ImportDone;
			photoSearch.ImportDone += photoSearch_ImportDone;

			m_ProcessingDialog.ProcessMessage = "Indexing photos...";
			m_ProcessingDialog.ActionAfterShown = () => {
				photoSearch.ImportToStationAsync(selectedPaths, session_token);
			};
			m_ProcessingDialog.ShowDialog(this);
		}

		void photoSearch_ImportDone(object sender, Wammer.Station.ImportDoneEventArgs e)
		{
			try
			{
				if (m_ProcessingDialog.InvokeRequired)
				{
					m_ProcessingDialog.Invoke(new MethodInvoker(() =>
					{
						photoSearch_ImportDone(sender, e);
					}));
				}
				else
				{
					m_ProcessingDialog.DialogResult = DialogResult.OK;
				}
			}
			catch
			{
			}
		}

		void photoSearch_MetadataUploaded(object sender, Wammer.Station.MetadataUploadEventArgs e)
		{
			try
			{
				if (m_ProcessingDialog.InvokeRequired)
				{
					m_ProcessingDialog.Invoke(new MethodInvoker(() =>
					{
						photoSearch_MetadataUploaded(sender, e);
					}));
				}
				else
				{
					m_ProcessingDialog.Hidable = true;
				}
			}
			catch
			{
			}
		}

		void photoSearch_FileImported(object sender, Wammer.Station.FileImportedEventArgs e)
		{
			try
			{
				if (m_ProcessingDialog.InvokeRequired)
				{
					m_ProcessingDialog.Invoke(new MethodInvoker(() =>
					{
						photoSearch_FileImported(sender, e);
					}));
				}
				else
				{
					var path = e.FilePath;
					if (path.Length > 50)
					{
						path = path.Substring(0, 22) + "....." + path.Substring(path.Length - 22);
					}
					m_ProcessingDialog.ProcessMessage = path + " imported";
				}
			}
			catch
			{
			}
		}

		#region Public Method
		/// <summary>
		/// Clears the interested paths.
		/// </summary>
		public void ClearInterestedPaths()
		{
			dataGridView1.Rows.Clear();
		}

		/// <summary>
		/// Adds the interested paths.
		/// </summary>
		/// <param name="paths">The paths.</param>
		public void AddInterestedPaths(IEnumerable<PathAndPhotoCount> paths)
		{
			foreach (var path in paths)
			{
				dataGridView1.Rows.Add(true, path.path, path.photoCount);
			}
		}

		/// <summary>
		/// Gets the selected paths.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<String> GetSelectedPaths()
		{
			//return from path in clbInterestedFolders.CheckedItems.OfType<object>()
			//       select path.ToString();

			for(int i=0; i<dataGridView1.RowCount; i++)
			{
				if ((bool)dataGridView1[0, i].Value)
					yield return dataGridView1[1, i].Value as string;
			}
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the Click event of the button1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void button1_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
				return;

			var selectedPath = folderBrowserDialog1.SelectedPath;

			for (int i = 0; i < dataGridView1.RowCount; i++)
			{
				if (dataGridView1[1, i].Value.Equals(selectedPath))
					return;
			}

			Cursor.Current = Cursors.WaitCursor;
			photoSearch.Search(selectedPath, (path, count) => {
				dataGridView1.Rows.Add(true, path, count); 
			});
			Cursor.Current = Cursors.Default;
		}

		#endregion

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			for (int i = 0; i < dataGridView1.RowCount; i++)
			{
				dataGridView1[0, i].Value = checkBox1.Checked;
			}

			dataGridView1.EndEdit();
		}
		

		private void FileImportControl_Load(object sender, EventArgs e)
		{

			var rect = dataGridView1.GetCellDisplayRectangle(0, -1, true);
			checkBox1 = new CheckBox {
				Size = new Size(14, 15),
				Checked = true
			};

			var loc = rect.Location;
			loc.X = loc.X + rect.Width / 2  - checkBox1.Width / 2;
			loc.Y = loc.Y + rect.Height / 2 - checkBox1.Height / 2;

			checkBox1.Location = loc;

			checkBox1.CheckedChanged += checkBox1_CheckedChanged;
			dataGridView1.Controls.Add(checkBox1);
		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 0)
			{
				bool origValue = (bool)dataGridView1[e.ColumnIndex, e.RowIndex].Value;

				dataGridView1[e.ColumnIndex, e.RowIndex].Value = !origValue;

				dataGridView1.EndEdit();
			}
		}
	}


	public interface IPhotoSearch
	{
		event EventHandler<Wammer.Station.MetadataUploadEventArgs> MetadataUploaded;
		event EventHandler<Wammer.Station.FileImportedEventArgs> FileImported;
		event EventHandler<Wammer.Station.ImportDoneEventArgs> ImportDone;

		IEnumerable<PathAndPhotoCount> InterestedPaths { get; }
		void ImportToStationAsync(IEnumerable<string> paths, string session_token);
		void Search(string path, PhotoFolderFound folderFound);
	}

	public delegate void PhotoFolderFound(string path, int count);

	public class PathAndPhotoCount
	{
		public string path { get; set; }
		public int photoCount { get; set; }


		public PathAndPhotoCount(string path, int count)
		{
			this.path = path;
			this.photoCount = count;
		}
	}
}

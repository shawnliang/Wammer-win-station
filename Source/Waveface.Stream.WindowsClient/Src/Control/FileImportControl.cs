using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;

namespace Waveface.Stream.WindowsClient
{
	public partial class FileImportControl : UserControl
	{
		#region Var
		private IPhotoSearch _photoSearch;
		private CheckBox _checkBox1;
		private string _originalLocation;
		#endregion


		#region Private Property
		private CheckBox m_SelectAll
		{
			get
			{
				if (_checkBox1 == null)
				{
					InitSelectAllCheckBox();
				}
				return _checkBox1;
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="FileImportControl"/> class.
		/// </summary>
		public FileImportControl(IPhotoSearch search)
		{
			InitializeComponent();
			this._photoSearch = search;
		}

		public FileImportControl()
			: this(new PhotoSearch())
		{
		}
		#endregion


		#region Private Method
		private void InitSelectAllCheckBox()
		{
			if (_checkBox1 != null)
				return;

			_checkBox1 = new CheckBox()
			{
				Size = new Size(14, 15),
				Checked = true
			};

			_checkBox1.CheckedChanged += checkBox1_CheckedChanged;
			dataGridView1.Controls.Add(_checkBox1);

			var rect = dataGridView1.GetCellDisplayRectangle(0, -1, true);

			var loc = rect.Location;
			loc.X = loc.X + (rect.Width - _checkBox1.Width) / 2;
			loc.Y = loc.Y + (rect.Height - _checkBox1.Height) / 2;

			_checkBox1.Location = loc;
		}

		/// <summary>
		/// Gets the selected paths.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<String> GetSelectedPaths()
		{
			for (int i = 0; i < dataGridView1.RowCount; i++)
			{
				if ((bool)dataGridView1[0, i].Value)
					yield return dataGridView1[2, i].Value as string;
			}
		}
		#endregion



		/// <summary>
		/// Imports the selected paths.
		/// </summary>
		private void ImportSelectedPaths(string session_token)
		{
			var selectedPaths = GetSelectedPaths();

			if (!selectedPaths.Any())
				return;

			_photoSearch.ImportToStationAsync(selectedPaths, session_token, radioCopy.Checked);
		}

		#region Public Method
		public void ImportSelectedPaths()
		{
			if (!StreamClient.Instance.IsLogined)
				return;

			ImportSelectedPaths(StreamClient.Instance.LoginedUser.SessionToken);
		}

		public void ChangeLocation()
		{
			if (!_originalLocation.Equals(txtStoreLocation.Text, StringComparison.InvariantCultureIgnoreCase))
			{
				var progressing = new ProcessingDialog();

				var bgworker = new BackgroundWorker();
				bgworker.DoWork += (sender, arg) =>
				{
					StationAPI.MoveFolder(StreamClient.Instance.LoginedUser.UserID, txtStoreLocation.Text, StreamClient.Instance.LoginedUser.SessionToken);
				};

				bgworker.RunWorkerCompleted += (sender, arg) =>
				{
					progressing.Close();

					if (arg.Error != null)
					{
						MessageBox.Show(arg.Error.GetDisplayDescription(), "Unable to change AOStream folder location");
					}
				};
				bgworker.RunWorkerAsync();

				progressing.Text = "Moving AOStream folder...";
				progressing.StartPosition = FormStartPosition.CenterParent;
				progressing.ProgressStyle = ProgressBarStyle.Marquee;
				progressing.ShowDialog();
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
			using (var dialog = new FolderBrowserDialog())
			{
				if (dialog.ShowDialog() != DialogResult.OK)
					return;

				var selectedPath = dialog.SelectedPath;

				AddImportFolder(new string[] { selectedPath });
			}
		}

		private void AddImportFolder(IEnumerable<String> selectedPaths)
		{
			var bg = new BackgroundWorker();
			bg.WorkerSupportsCancellation = true;
			bg.DoWork += new DoWorkEventHandler(bg_DoWork);
			bg.RunWorkerAsync(new ImportArgs { paths = selectedPaths, bgworker = bg });

			var processingDialog = new WaitingBGWorkerDialog { Title = "Scanning photos", BackgroupWorker = bg };
			processingDialog.StartPosition = FormStartPosition.CenterParent;
			processingDialog.ShowDialog(this);

		}

		void bg_DoWork(object sender, DoWorkEventArgs e)
		{
			var arg = (ImportArgs)e.Argument;

			foreach (var selectedPath in arg.paths)
			{
				Cursor.Current = Cursors.WaitCursor;

				_photoSearch.Search(selectedPath, (p, c) =>
				{
					if (arg.bgworker.CancellationPending)
						return false;

					p = p.TrimEnd(Path.DirectorySeparatorChar);

					this.Invoke(new MethodInvoker(() =>
					{
						try
						{
							for (int i = 0; i < dataGridView1.RowCount; i++)
							{
								if (dataGridView1[2, i].Value.Equals(p))
									return;
							}
							dataGridView1.Rows.Add(true, Path.GetFileName(p), p);
						}
						catch (Exception ex)
						{
							log4net.LogManager.GetLogger(typeof(FileImportControl)).Warn("Unable to add path:" + p, ex);
						}
					}));

					return true;
				});
			}
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			AddImportFolder(WindowsLibraries.GetLibrariesFolders());
		}

		private void button2_Click(object sender, EventArgs e)
		{
			AddImportFolder(Picasa.GetAlbums());
		}
		#endregion

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			for (int i = 0; i < dataGridView1.RowCount; i++)
			{
				dataGridView1[0, i].Value = _checkBox1.Checked;
			}

			dataGridView1.EndEdit();
		}


		private void FileImportControl_Load(object sender, EventArgs e)
		{
			if (this.IsDesignMode())
				return;

			dataGridView1.Rows.Clear();
			InitSelectAllCheckBox();
			txtStoreLocation.Text = _originalLocation = _photoSearch.GetUserFolder(StreamClient.Instance.LoginedUser.UserID);
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

		private void changeButton_Click(object sender, EventArgs e)
		{
			ResourceFolder.Change(
				StreamClient.Instance.LoginedUser.UserID,
				StreamClient.Instance.LoginedUser.SessionToken,
				(newFolder) => { txtStoreLocation.Text = newFolder; }
			);
		}
	}


	public interface IPhotoSearch
	{
		IEnumerable<PathAndPhotoCount> InterestedPaths { get; }
		void ImportToStationAsync(IEnumerable<string> paths, string session_token, bool copyToStation = true);
		void Search(string path, PhotoFolderFound folderFound);
		string GetUserFolder(string user_id);
	}

	public delegate bool PhotoFolderFound(string path, int count);

	public class PathAndPhotoCount
	{
		public string path { get; set; }
		public int photoCount { get; set; }

		public PathAndPhotoCount(string path, int count)
		{
			this.path = path;
			this.photoCount = count;
		}

		public override int GetHashCode()
		{
			return path.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (obj is PathAndPhotoCount)
				return path.Equals(((PathAndPhotoCount)obj).path);
			else
				return false;
		}
	}

	class ImportArgs
	{
		public BackgroundWorker bgworker { get; set; }
		public IEnumerable<string> paths { get; set; }
	}
}

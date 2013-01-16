using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	public partial class FileImportControl : StepPageControl
	{
		#region Var
		private IPhotoSearch _photoSearch;
		private CheckBox _checkBox1;
		private HidableProgressingDialog _processDialog;
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="FileImportControl"/> class.
		/// </summary>
		public FileImportControl(IPhotoSearch search)
		{
			InitializeComponent();
			this._photoSearch = search;
			this.PageTitle = "Import from folders";
		}

		public FileImportControl()
		{
			InitializeComponent();
			this._photoSearch = new PhotoSearch();
			this.PageTitle = "Import from folders";
		}
		#endregion


		/// <summary>
		/// Processes the file import step.
		/// </summary>
		public override void OnEnteringStep(WizardParameters parameters)
		{
			base.OnEnteringStep(parameters);

			ClearInterestedPaths();
			AddInterestedPaths(_photoSearch.InterestedPaths);
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

			_photoSearch.ImportToStationAsync(selectedPaths, session_token);
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

			for (int i = 0; i < dataGridView1.RowCount; i++)
			{
				if ((bool)dataGridView1[0, i].Value)
					yield return dataGridView1[2, i].Value as string;
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

			AddImportFolder(new string[] { selectedPath });
		}

		private void AddImportFolder(IEnumerable<String> selectedPaths)
		{
			foreach (var selectedPath in selectedPaths)
			{
				Cursor.Current = Cursors.WaitCursor;

				_photoSearch.Search(selectedPath, (p, c) =>
				{
					try
					{
						p = p.TrimEnd(Path.DirectorySeparatorChar);
						for (int i = 0; i < dataGridView1.RowCount; i++)
						{
							if (dataGridView1[2, i].Value.Equals(p))
								return;
						}
						dataGridView1.Rows.Add(true, Path.GetFileName(p), p);
					}
					catch (Exception)
					{
					}
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
			var rect = dataGridView1.GetCellDisplayRectangle(0, -1, true);
			_checkBox1 = new CheckBox
			{
				Size = new Size(14, 15),
				Checked = true
			};

			var loc = rect.Location;
			loc.X = loc.X + (rect.Width - _checkBox1.Width) / 2;
			loc.Y = loc.Y + (rect.Height - _checkBox1.Height) / 2;

			_checkBox1.Location = loc;

			_checkBox1.CheckedChanged += checkBox1_CheckedChanged;
			dataGridView1.Controls.Add(_checkBox1);
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

		public override int GetHashCode()
		{
			return path.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (obj is PathAndPhotoCount)
				return path.Equals( ((PathAndPhotoCount)obj).path );
			else
				return false;
		}
	}
}

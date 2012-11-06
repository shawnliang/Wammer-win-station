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
		/// Sends the sync context.
		/// </summary>
		/// <param name="target">The target.</param>
		private void SendSyncContext(Action target)
		{
			mainSyncCtx.Send((obj) => target(), null);
		}

		/// <summary>
		/// Sends the sync context.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">The target.</param>
		/// <param name="o">The o.</param>
		private void SendSyncContext<T>(Action<T> target, Object o)
		{
			mainSyncCtx.Send((obj) => target((T)obj), o);
		}


		/// <summary>
		/// Imports the selected paths.
		/// </summary>
		private void ImportSelectedPaths(string session_token)
		{
			var selectedPaths = GetSelectedPaths();

			if (selectedPaths.Count() == 0)
				return;

			var dialog = new ProcessingDialog();
			var postID = Guid.NewGuid().ToString();
			
			MethodInvoker mi = new MethodInvoker(() => { photoSearch.ImportToStation(selectedPaths, session_token); });

			AutoResetEvent autoEvent = new AutoResetEvent(false);
			mi.BeginInvoke((result) =>
			{
				autoEvent.WaitOne();
				SendSyncContext(() =>
				{
					mi.EndInvoke(result);
					dialog.Dispose();
					dialog = null;
				});
			}, null);

			dialog.ProcessMessage = "Data importing";
			dialog.ProgressStyle = ProgressBarStyle.Marquee;
			dialog.StartPosition = FormStartPosition.CenterParent;

			autoEvent.Set();
			dialog.ShowDialog(this);
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
		public void AddInterestedPaths(IEnumerable<String> paths)
		{
			foreach (var path in paths)
			{
				dataGridView1.Rows.Add(true, path, "0");
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

			dataGridView1.Rows.Add(true, selectedPath, "1");
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
		IEnumerable<string> InterestedPaths { get; }
		void ImportToStation(IEnumerable<string> paths, string session_token);
	}
}

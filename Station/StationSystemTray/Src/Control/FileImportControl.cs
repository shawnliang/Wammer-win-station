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

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="FileImportControl"/> class.
		/// </summary>
		public FileImportControl(IPhotoSearch search, SynchronizationContext mainSyncCtx)
		{
			InitializeComponent();
			this.photoSearch = search;
			this.mainSyncCtx = mainSyncCtx;
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
			clbInterestedFolders.Items.Clear();
		}

		/// <summary>
		/// Adds the interested paths.
		/// </summary>
		/// <param name="paths">The paths.</param>
		public void AddInterestedPaths(IEnumerable<String> paths)
		{
			clbInterestedFolders.Items.AddRange(paths.OfType<object>().ToArray());
		}

		/// <summary>
		/// Gets the selected paths.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<String> GetSelectedPaths()
		{
			return from path in clbInterestedFolders.CheckedItems.OfType<object>()
				   select path.ToString();
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
			var linq = from item in clbInterestedFolders.CheckedItems.OfType<object>()
					   let path = item.ToString()
					   where path.Equals(selectedPath, StringComparison.CurrentCultureIgnoreCase)
					   select path;

			if (linq.Any())
				return;

			clbInterestedFolders.Items.Add(selectedPath);
		}

		/// <summary>
		/// Handles the Click event of the button2 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void button2_Click(object sender, EventArgs e)
		{
			for (int idx = 0; idx < clbInterestedFolders.Items.Count; ++idx)
			{
				clbInterestedFolders.SetItemChecked(idx, true);
			}
		}
		#endregion
	}


	public interface IPhotoSearch
	{
		IEnumerable<string> InterestedPaths { get; }
		void ImportToStation(IEnumerable<string> paths, string session_token);
	}
}

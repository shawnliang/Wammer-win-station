using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace StationSystemTray
{
	public partial class FileImportControl : UserControl
	{
		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="FileImportControl"/> class.
		/// </summary>
		public FileImportControl()
		{
			InitializeComponent();
		} 
		#endregion


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
}

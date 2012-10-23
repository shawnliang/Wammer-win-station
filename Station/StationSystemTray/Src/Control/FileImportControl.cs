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
			clbInterestedFolders.Items.AddRange(paths.OfType<Object>().ToArray());
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the clbInterestedFolders control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void clbInterestedFolders_SelectedIndexChanged(object sender, EventArgs e)
		{
			//if(clbInterestedFolders.SelectedItem == null)
			//    return;

			//var path = clbInterestedFolders.SelectedItem.ToString();
			//var files = (new DirectoryInfo(path)).EnumerateFiles("*.*");

			//listView1.Items.
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Waveface.Stream.Core;

namespace Waveface.Stream.WindowsClient
{
	public partial class DocImportControl : Control
	{
		private CheckBox checkBox1;
		private HidableProgressingDialog _processDialog;

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="FileImportControl"/> class.
		/// </summary>
		public DocImportControl()
		{
			InitializeComponent();
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

		private void importSelectedPaths(string session_token)
		{
			List<string> docPaths = new List<string>();

			for (int i = 0; i < dataGridView1.RowCount; i++)
			{
				if ((bool)dataGridView1[0, i].Value)
					docPaths.Add(dataGridView1[1, i].Value as string);
			}

			StationAPI.ImportDoc(session_token, docPaths);
		}


		#region Public Method

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

			dataGridView1.Rows.Add(true, selectedPath);
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
			checkBox1 = new CheckBox
			{
				Size = new Size(14, 15),
				Checked = true
			};

			var loc = rect.Location;
			loc.X = loc.X + rect.Width / 2 - checkBox1.Width / 2;
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

}

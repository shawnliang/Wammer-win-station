using System;
using System.Linq;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public partial class LogMessageComponent : UserControl, IDebugComponent
	{
		public LogMessageComponent()
		{
			InitializeComponent();
		}

		private void toolStripLabel1_Click(object sender, EventArgs e)
		{
			logMessageListBox1.Items.Clear();
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var selectedItems = logMessageListBox1.SelectedItems;

			if (selectedItems == null || selectedItems.Count == 0)
				return;

			Clipboard.SetText(string.Join(Environment.NewLine, selectedItems.OfType<String>().ToArray()));
		}

		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			logMessageListBox1.SelectAll();
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StationSystemTray.Properties;
using System.IO;

namespace StationSystemTray
{
	public partial class AddDeviceDialog : Form
	{
		public AddDeviceDialog()
		{
			InitializeComponent();
			Icon = Resources.Icon;
		}

		private void AddDeviceDialog_Load(object sender, EventArgs e)
		{
			var rtfBytes = Encoding.UTF8.GetBytes(Resources.addDeviceInstruction);
			using (var rtf = new MemoryStream(rtfBytes))
			{
				richTextBox1.LoadFile(rtf, RichTextBoxStreamType.RichText);
			}
		}

		private void doneButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}

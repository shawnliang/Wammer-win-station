using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Waveface.Common;
using Waveface.Stream.WindowsClient.Properties;
using System.Reflection;

namespace Waveface.Stream.WindowsClient.Src.Control
{
	public partial class AboutControl : UserControl
	{
		public AboutControl()
		{
			InitializeComponent();
		}

		private void CheckUpdateButton_Click(object sender, EventArgs e)
		{
			var update = new AutoUpdate(false);
			if (update.IsUpdateRequired())
				update.ShowUpdateNeededUI();
			else
				MessageBox.Show(Resources.ALREAD_UPDATED);
		}

		private void AboutControl_Load(object sender, EventArgs e)
		{
			lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}
	}
}

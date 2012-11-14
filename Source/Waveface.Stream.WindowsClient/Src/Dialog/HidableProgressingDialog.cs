using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public partial class HidableProgressingDialog : ProcessingDialog
	{
		private BackgroundWorker bgworker;

		public Action ActionAfterShown { get; set; }
		
		public HidableProgressingDialog()
		{
			InitializeComponent();
		}

		private void goBackgroundButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		public bool Hidable
		{
			get { return goBackgroundButton.Enabled; }
			set { goBackgroundButton.Enabled = value; }
		}

		private void HidableProgressingDialog_Shown(object sender, EventArgs e)
		{
			if (ActionAfterShown != null)
			{
				bgworker = new BackgroundWorker();
				bgworker.DoWork += (s, args) => { ActionAfterShown(); };
				bgworker.RunWorkerAsync();
			}
		}
	}
}

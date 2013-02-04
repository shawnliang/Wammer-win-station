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
	public partial class WaitingBGWorkerDialog : Form
	{
		BackgroundWorker bgWorker;
		Timer timer = new Timer();

		public WaitingBGWorkerDialog()
		{
			InitializeComponent();

			timer.Tick += checkBgWorkerBusy;
			timer.Interval = 500;
		}

		public string Title
		{
			get { return this.Text; }
			set { this.Text = value; }
		}

		public BackgroundWorker BackgroupWorker
		{
			set { bgWorker = value; }
		}

		private void cancelBtn_Click(object sender, EventArgs e)
		{
			bgWorker.CancelAsync();

			cancelBtn.Text = "Stopping...";
			cancelBtn.Enabled = false;
		}

		private void checkBgWorkerBusy(object sender, EventArgs args)
		{
			if (!bgWorker.IsBusy)
			{
				Close();
				timer.Stop();
			}
		}

		private void WaitingBGWorkerDialog_Load(object sender, EventArgs e)
		{
			timer.Enabled = true;
			timer.Start();
		}
	}
}

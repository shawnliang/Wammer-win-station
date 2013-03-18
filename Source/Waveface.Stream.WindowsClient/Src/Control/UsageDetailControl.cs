using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient.Src.Control
{
	public partial class UsageDetailControl : UserControl
	{
		#region Public Property
		public String ResourcePath
		{
			get
			{
				return linkLabel3.Text;
			}
			set
			{
				linkLabel3.Text = value;
			}
		}

		public string CloudTotalUsage
		{
			get
			{
				return lblCloudTotalUsage.Text;
			}
			set
			{
				lblCloudTotalUsage.Text = value;
			}
		}

		public long TotalPhoto
		{
			get
			{
				return long.Parse(lblTotalPhoto.Text);
			}
			set
			{
				lblTotalPhoto.Text = value.ToString("N0");
			}
		}

		public string LocalPhoto
		{
			get
			{
				return lblLocalPhoto.Text;
			}
			set
			{
				lblLocalPhoto.Text = value;
			}
		}

		public string LocalDocument
		{
			get
			{
				return lblLocalDocument.Text;
			}
			set
			{
				lblLocalDocument.Text = value;
			}
		}

		public string CloudPhoto
		{
			get
			{
				return lblCloudPhoto.Text;
			}
			set
			{
				lblCloudPhoto.Text = value;
			}
		}

		public string CloudDocument
		{
			get
			{
				return lblCloudDocument.Text;
			}
			set
			{
				lblCloudDocument.Text = value;
			}
		}


		public long TotalWeb
		{
			get
			{
				return long.Parse(lblTotalWeb.Text);
			}
			set
			{
				lblTotalWeb.Text = value.ToString("N0");
			}
		}

		public long TotalDocument
		{
			get
			{
				return long.Parse(lblTotalDocument.Text);
			}
			set
			{
				lblTotalDocument.Text = value.ToString("N0");
			}
		}
		#endregion


		#region Event
		public event EventHandler ChangeResourcePathButtonClick;
		#endregion


		#region Constructor
		public UsageDetailControl()
		{
			InitializeComponent();
		}
		#endregion


		#region Protected Method
		protected void OnChangeResourcePathButtonClick(EventArgs e)
		{
			this.RaiseEvent(ChangeResourcePathButtonClick, e);
		}
		#endregion


		#region Event Process
		private void btnChangeResourceFolder_Click(object sender, EventArgs e)
		{
			OnChangeResourcePathButtonClick(EventArgs.Empty);
		}

		private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Directory.CreateDirectory(linkLabel3.Text);
				Process.Start(linkLabel3.Text);
			}
			catch (Exception)
			{
			}
		}
		#endregion
	}
}

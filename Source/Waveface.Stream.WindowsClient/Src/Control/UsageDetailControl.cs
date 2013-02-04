using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
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
				return lblResourcePath.Text;
			}
			set
			{
				lblResourcePath.Text = value;
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
				lblTotalPhoto.Text = value.ToString();
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


		public long TotalWeb
		{
			get
			{
				return long.Parse(lblTotalWeb.Text);
			}
			set
			{
				lblTotalWeb.Text = value.ToString();
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
				lblTotalDocument.Text = value.ToString();
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
		#endregion
	}
}

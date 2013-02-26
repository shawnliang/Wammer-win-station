using System;
using System.Windows.Forms;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public partial class LoginInputBox : UserControl
	{
		#region Property
		/// <summary>
		/// Gets the email.
		/// </summary>
		/// <value>The email.</value>
		public String Email
		{
			get
			{
				return tbxEMail.Text;
			}
		}

		/// <summary>
		/// Gets the password.
		/// </summary>
		/// <value>The password.</value>
		public String Password
		{
			get
			{
				return tbxPassword.Text;
			}
		}

		/// <summary>
		/// Gets or sets the enable drop down.
		/// </summary>
		/// <value>
		/// The enable drop down.
		/// </value>
		public Boolean EnableDropDown
		{
			get
			{
				return button1.Visible;
			}
			set
			{
				button1.Visible = value;

				AdjustLayout();
			}
		}
		#endregion


		#region Constructor
		public LoginInputBox()
		{
			InitializeComponent();

			cmbEmail.DataBindings.Add("Visible", button1, "Visible");
			AdjustLayout();
		}
		#endregion

		#region Private Method
		private void AdjustLayout()
		{
			var halfHeight = Resources.input_box_2.Height / 2;
			var halfControlHeight = tbxEMail.Height / 2;
			var emailTop = (halfHeight - halfControlHeight) / 2;
			var passwordTop = halfHeight + emailTop;

			cmbEmail.Top = emailTop;

			tbxEMail.Top = emailTop;
			tbxEMail.Width = EnableDropDown ? button1.Left - tbxEMail.Left : tbxPassword.Width;

			tbxPassword.Top = passwordTop;
		}

		private void LoginInputBox_SizeChanged(object sender, EventArgs e)
		{
			AdjustLayout();
		}
		#endregion
	}
}

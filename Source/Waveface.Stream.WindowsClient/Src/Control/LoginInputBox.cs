using System;
using System.Windows.Forms;

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
		}
		#endregion

		#region Private Method
		private void AdjustLayout()
		{
			tbxEMail.Width = EnableDropDown ? button1.Left - tbxEMail.Left : tbxPassword.Width;
		}
		#endregion
	}
}

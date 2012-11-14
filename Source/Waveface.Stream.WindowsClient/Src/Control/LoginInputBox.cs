using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
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
		#endregion


		#region Constructor
		public LoginInputBox()
		{
			InitializeComponent();
		} 
		#endregion
	}
}

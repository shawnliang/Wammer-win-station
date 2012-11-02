using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StationSystemTray.Properties;
using System.Threading;

namespace StationSystemTray.Dialog
{
	public partial class SignUpDialog : Form
	{
		#region Public Property
		/// <summary>
		/// Gets the browser.
		/// </summary>
		/// <value>The browser.</value>
		public WebBrowser Browser
		{
			get
			{
				return webBrowser1;
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="SignUpDialog"/> class.
		/// </summary>
		public SignUpDialog()
		{
			InitializeComponent();
		} 
		#endregion


		#region Protected Method
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			//prevent ctrl+tab to switch signin pages
			if (keyData == (Keys.Control | Keys.Tab))
			{
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		#endregion


		#region Public Method
		public void ShowSignUpPage()
		{
			tabControlEx1.SelectedTab = tabPage1;
		}
		#endregion
	}
}

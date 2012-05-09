using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray.Control
{
	/// <summary>
	/// 
	/// </summary>
	public partial class FBLoginButton : UserControl
	{
		#region Property
		/// <summary>
		/// </summary>
		/// <value></value>
		/// <returns>The text associated with this control.</returns>
		[Localizable(true)]
		public string DisplayText
		{
			get { return lblLoginMsg.Text; }
			set { lblLoginMsg.Text = value; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="FBLoginButton"/> class.
		/// </summary>
		public FBLoginButton()
		{
			InitializeComponent();
		} 
		#endregion

		#region Event Process
		/// <summary>
		/// Handles the Click event of the lblLoginMsg control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void lblLoginMsg_Click(object sender, EventArgs e)
		{
			OnClick(EventArgs.Empty);
		}

		/// <summary>
		/// Handles the Click event of the picFBIcon control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void picFBIcon_Click(object sender, EventArgs e)
		{
			OnClick(EventArgs.Empty);
		} 
		#endregion
	}
}

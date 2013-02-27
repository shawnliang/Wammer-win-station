using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
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
	}
}

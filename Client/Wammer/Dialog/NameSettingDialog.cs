using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Waveface
{
	public partial class NameSettingDialog : Form
	{
		#region Property
		public String UserName
		{
			get
			{
				return textBox2.Text;
			}
			set
			{
				textBox2.Text = value;
			}
		}
		#endregion

		public NameSettingDialog()
		{
			InitializeComponent();
		}
	}
}

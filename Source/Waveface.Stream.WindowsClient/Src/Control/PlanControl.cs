using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Waveface.Stream.WindowsClient
{
	public partial class PlanControl : UserControl
	{
		#region Public Property
		public String HeaderIconText
		{
			get
			{
				return btnPlanHeaderIcon.Text;
			}
			set
			{
				btnPlanHeaderIcon.Text = value;
			}
		}

		public Boolean HeaderVisible
		{
			get
			{
				return lblPlanHeader.Visible;
			}
			set
			{
				lblPlanHeader.Visible = value;
			}
		}

		[Localizable(true)]
		public String HeaderText
		{
			get
			{
				return lblPlanHeader.Text;
			}
			set
			{
				lblPlanHeader.Text = value;
			}
		}


		public Font DescriptionFont
		{
			get
			{
				return rtbxPlanDescription.Font;
			}
			set
			{
				rtbxPlanDescription.Font = value;
			}
		} 

		public Color DescriptionColor
		{
			get
			{
				return rtbxPlanDescription.ForeColor;
			}
			set
			{
				rtbxPlanDescription.ForeColor = value;
			}
		}

		[Localizable(true)]
		public String Description
		{
			get
			{
				return rtbxPlanDescription.Text;
			}
			set
			{
				rtbxPlanDescription.Text = value;
			}
		}

		[Localizable(true)]
		public string RTFDescription
		{
			get 
			{
				return rtbxPlanDescription.Rtf;
			}
			set 
			{
				rtbxPlanDescription.Rtf = value;
			}
		}
		#endregion


		#region Private Method
		private Color GetBackColor(System.Windows.Forms.Control control)
		{
			if (control.BackColor != Color.Transparent)
				return control.BackColor;

			return SystemColors.Control;
		}

		#endregion


		#region Event Process
		public PlanControl()
		{
			InitializeComponent();
		}

		private void PlanBox_BackColorChanged(object sender, EventArgs e)
		{
			rtbxPlanDescription.BackColor = GetBackColor(this);
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray
{

	public partial class ChoosePlanControl : StepPageControl
	{
		public ChoosePlanControl()
		{
			InitializeComponent();
			PageTitle = "Upgrade";
			CustomSize = new Size(710, 437);
		}

		public override bool RunOnce
		{
			get
			{
				return true;
			}
		}
	}
}

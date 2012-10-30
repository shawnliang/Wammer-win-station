using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace StationSystemTray
{
	public class StepPageControl : UserControl
	{
		public WizardControl WizardControl { get; set; }

		public virtual void OnEnteringStep(WizardParameters parameters)
		{
		}

		public virtual void OnLeavingStep(WizardParameters parameters)
		{
		}


		/// <summary>
		/// If true, wizard will provice "next" and "prev" buttons on this page
		/// </summary>
		public virtual bool HasPrevAndBack
		{
			get { return true; }
		}

		/// <summary>
		/// If true, user cannot go back to this step. Default is false.
		/// </summary>
		public virtual bool RunOnce
		{
			get { return false; }
		}
	}
}

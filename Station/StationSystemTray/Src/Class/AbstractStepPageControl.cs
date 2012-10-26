using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray
{
	public abstract class AbstractStepPageControl : UserControl
	{
		public virtual void OnEnteringStep()
		{
		}

		public virtual void OnLeavingStep()
		{
		}
	}
}

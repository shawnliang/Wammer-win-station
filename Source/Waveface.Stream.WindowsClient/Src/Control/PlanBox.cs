using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public class PlanBox : Control
	{
		#region Enum
		public enum PlanType
		{
 			Free
		}
		#endregion

		#region Var
		private PlanControl _planControl;
		private PlanType _type;
		#endregion

		#region Private Property
		private PlanControl m_PlanControl
		{
			get
			{
				return _planControl ?? (_planControl = new PlanControl()
				{
					HeaderVisible = false
				});
			}
		}
		#endregion


		#region Public Property
		public PlanType Type
		{
			get
			{
				return _type;
			}
			set
			{
				if (_type == value)
					return;

				_type = value;
				UpdateUI();
			}
		}
		#endregion


		#region Constructor
		public PlanBox()
		{
			m_PlanControl.Dock = DockStyle.Fill;

			this.Controls.Add(m_PlanControl);

			UpdateUI();
		}
		#endregion


		#region Private Method
		private void UpdateUI()
		{
			switch (this.Type)
			{
				case PlanType.Free:
					m_PlanControl.HeaderIconText = "Free";
					m_PlanControl.RTFDescription = Resources.FreePlan;
					break;
				default:
					break;
			}
		}
		#endregion
	}
}
